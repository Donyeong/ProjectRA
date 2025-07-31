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


		[MenuItem("Custom/CI/Build")]
		public static void Build()
		{
			string oldDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
			string defineSetting = GetArg("-defineSetting");
			Debug.Log("-defineSetting = " + defineSetting);
			if (!string.IsNullOrEmpty(defineSetting))
			{
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defineSetting);
			}


			var _SCENES = FindEnabledEditorScenes();
			var buildFolder = GetArg("-buildFolder");// ;
			GenericBuild(_SCENES, buildFolder);

			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, oldDefines);
		}

		private static void GenericBuild(string[] scenes, string app_target)
		{
			var group = BuildTargetGroup.Standalone;
			if (PlayerSettings.GetScriptingBackend(group) != ScriptingImplementation.Mono2x)
			{
				PlayerSettings.SetScriptingBackend(group, ScriptingImplementation.Mono2x);
			}

			BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
			buildPlayerOptions.scenes = scenes;
			buildPlayerOptions.locationPathName = app_target;
			buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
			buildPlayerOptions.targetGroup = BuildTargetGroup.Standalone;
			buildPlayerOptions.subtarget = (int)StandaloneBuildSubtarget.Player;
			buildPlayerOptions.options = BuildOptions.Development;

			var report = BuildPipeline.BuildPlayer(buildPlayerOptions);

			var summary = report.summary;
			if (summary.result == BuildResult.Succeeded)
			{
				Debug.Log("[BuildResult] Succeeded!");
			}
			else if (summary.result == BuildResult.Failed)
			{
				Debug.Log("[BuildResult] Failed"); foreach (var step in report.steps)
				{
					foreach (var message in step.messages)
					{
						if (message.type == LogType.Error)
						{
							Debug.Log($"[BuildResult Error] {message.content}");
						}
						if (message.type == LogType.Exception)
						{
							Debug.Log($"[BuildResult Exception] {message.content}");
						}
					}
					foreach (var message in step.messages)
					{
						Debug.Log($"[BuildResult {message.type}] {message.content}");
					}
				}
			}
			else if (summary.result == BuildResult.Cancelled)
			{
				Debug.Log("[BuildResult] Cancelled!");
			}
			else
			{ // Unknown           
				Debug.Log("[BuildResult] Unknown!");
			}

			//resultLogPath에 빌드 결과를 저장합니다.
			var resultLogPath = GetArg("-result_log_path");
			if (!string.IsNullOrEmpty(resultLogPath))
			{
				System.IO.File.WriteAllText(resultLogPath, report.ToString());
				Debug.Log("Build result log saved to: " + resultLogPath);
			}
			else
			{
				Debug.LogWarning("No result log path specified. Build result log not saved.");
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
	}
}
