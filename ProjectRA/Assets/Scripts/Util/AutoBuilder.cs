using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.CrashReportHandler;

namespace JENKINS
{
    public class AutoBuilder : ScriptableObject
    {
        static string[] SCENES;

        // Use real app name here        
        /* Anyway the App will have the name as configured within the Unity-Editor
         * This Appname is just for the Folder in which to Build */

        static string APP_NAME;
        static string TARGET_DIR;
        [MenuItem("Custom/CI/Windows Mixed Reality Build (UWP)")]
        public static void Build()
        {
            SCENES = FindEnabledEditorScenes();

            string oldDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
            string defineSetting = GetArg("-defineSetting");
            Debug.Log("defineSetting = " + defineSetting);
            if (!string.IsNullOrEmpty(defineSetting))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defineSetting);
            }

            TARGET_DIR = GetArg("-buildFolder");
            //앱이름 = 날짜
			APP_NAME = "APP_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".apk";
			Debug.Log("Jenkins-Build: APP_NAME: " + APP_NAME + " TARGET_DIR: " + TARGET_DIR);
            GenericBuild(SCENES, TARGET_DIR + "/" + APP_NAME, BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64, BuildOptions.None);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, oldDefines);
        }

        private static string[] FindEnabledEditorScenes()
        {
            List<string> EditorScenes = new List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (!scene.enabled) continue;
                EditorScenes.Add(scene.path);
            }
            return EditorScenes.ToArray();
        }


        private static void GenericBuild(string[] scenes, string app_target, BuildTargetGroup build_target_group, BuildTarget build_target, BuildOptions build_options)
        {
            string[] arguments = System.Environment.GetCommandLineArgs();
            //foreach (string arg in arguments)
            //{
            //    Debug.Log("Jenkins-Build: arg: " + arg);
            //}

/*            string keyaliasPass = GetArg("-keyaliasPass");
            string keystorePass = GetArg("-keystorePass");
            //EditorUserBuildSettings.SwitchActiveBuildTarget(build_target_group, BuildTarget.Android);      
            PlayerSettings.keyaliasPass = keyaliasPass;
            PlayerSettings.keystorePass = keystorePass;
            PlayerSettings.Android.keystoreName = GetArg("-keystoreName");

            string bundleVersionCode = GetArg("-BundleVersionCode");
            if (null != bundleVersionCode)
            {
                PlayerSettings.Android.bundleVersionCode = int.Parse(bundleVersionCode);
                PlayerSettings.Android.useAPKExpansionFiles = true;//split apk
                CrashReportHandler.enableCaptureExceptions = true;//CrashReport On
                EditorUserBuildSettings.androidCreateSymbols = AndroidCreateSymbols.Public;//심볼
            }

            Debug.Log("**** keystoreName : " + PlayerSettings.Android.keystoreName);
            Debug.Log("**** keyaliasPass : " + keyaliasPass);
            Debug.Log("**** keystorePass : " + keystorePass);*/

            /*string buildAppBundle = GetArg("-buildAppBundle");
            bool bbuildAB = bool.Parse(buildAppBundle);

            EditorUserBuildSettings.buildAppBundle = bbuildAB;*/

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = scenes;
            buildPlayerOptions.locationPathName = app_target;
            buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
            buildPlayerOptions.options = BuildOptions.None;
            Debug.LogFormat("**** app_target : {0}", app_target);

            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            Debug.LogFormat("**** player : {0}", report);

            var summary = report.summary;
            Debug.LogFormat("**** summary.result : {0}", summary.result);

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("**** Succeeded!");
            }
            else if (summary.result == BuildResult.Failed)
            {
                Debug.Log("**** Failed!");
                foreach (var step in report.steps)
                {
                    foreach (var message in step.messages)
                    {
                        Debug.Log("****" + message.content);
                    }
                }
			}
            else if (summary.result == BuildResult.Cancelled)
            {
                Debug.Log("**** Cancelled!");
            }
            else
            { // Unknown           
                Debug.Log("**** Unknown!");
            }
        }

        /**         
         * * Get Arguments from the command line by name         
         */
        private static string GetArg(string name)
        {
            var args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == name && args.Length > i + 1)
                {
                    return args[i + 1];
                }
            }
            return null;
        }
	}
}
