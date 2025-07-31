#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Text;
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

			StringBuilder resSB = new StringBuilder();

			var summary = report.summary;
			if (summary.result == BuildResult.Succeeded)
			{
				resSB.Append("[BuildResult] Succeeded!\n");
			}
			else if (summary.result == BuildResult.Failed)
			{
				resSB.Append("[BuildResult] Failed!\n");
				foreach (var step in report.steps)
				{
					foreach (var message in step.messages)
					{
						if (message.type == LogType.Error)
						{
							resSB.Append($"[BuildResult Error][{step.name}] {message.content}\n");
						}
						if (message.type == LogType.Exception)
						{
							resSB.Append($"[BuildResult Exception][{step.name}] {message.content}\n");
						}
					}
					foreach (var message in step.messages)
					{
						if (message.type == LogType.Error || message.type == LogType.Exception)
						{
							continue;
						}
						resSB.Append($"[BuildResult {message.type}][{step.name}] {message.content}\n");
					}
				}
			}
			else if (summary.result == BuildResult.Cancelled)
			{
				resSB.Append("[BuildResult] Cancelled!\n");
			}
			else
			{ // Unknown           
				resSB.Append("[BuildResult] Unknown!\n");
			}

			Debug.Log(resSB.ToString());

			//resultLogPath에 빌드 결과를 저장합니다.
			var resultLogPath = GetArg("-result_log_path");
			if (!string.IsNullOrEmpty(resultLogPath))
			{
				Debug.Log("Build result log saved to: " + resultLogPath);
				System.IO.File.WriteAllText(resultLogPath, resSB.ToString());
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

#endif