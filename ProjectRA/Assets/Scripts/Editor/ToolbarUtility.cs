using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
#else
  using UnityEngine.Experimental.UIElements;
#endif

[Serializable]
public enum ToolbarZone
{
	ToolbarZoneRightAlign,
	ToolbarZoneLeftAlign
}

[InitializeOnLoad]
public static class ToolbarUtility
{
	private static ScriptableObject _toolbar;
	private static string[] _scenePaths;
	private static string[] _sceneNames;
	
	private static string[] _prefabPaths;
	private static string[] _prefabNames;

	public static bool DrawGizmoDoor;
	public static bool DrawGizmoProp;

	static ToolbarUtility()
	{
		EditorApplication.delayCall += () => {
			EditorApplication.update -= Update;
			EditorApplication.update += Update;
		};
	}

	private static void Update()
	{
		if (_prefabPaths == null)
		{
			List<string> prefabPaths = new List<string>();
			List<string> prefabNames = new List<string>();
			string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Resources" });
			foreach (string guid in guids)
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);
				string resourcePath = path;
				if (resourcePath.StartsWith("Assets/Resources/"))
					resourcePath = resourcePath.Substring("Assets/Resources/".Length);
				resourcePath = System.IO.Path.ChangeExtension(resourcePath, null); // 확장자 제거
				prefabNames.Add(resourcePath);
				prefabPaths.Add(path);
			}
			_prefabPaths = prefabPaths.ToArray();
			_prefabNames = prefabNames.ToArray();
		}
		if (_toolbar == null)
		{
			Assembly editorAssembly = typeof(UnityEditor.Editor).Assembly;

			UnityEngine.Object[] toolbars = UnityEngine.Resources.FindObjectsOfTypeAll(editorAssembly.GetType("UnityEditor.Toolbar"));
			_toolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
			if (_toolbar != null)
			{
				var _root = _toolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
				var rawRoot = _root.GetValue(_toolbar);
				var mRoot = rawRoot as VisualElement;
				RegisterCallback(ToolbarZone.ToolbarZoneLeftAlign.ToString(), OnGUILeft);
				RegisterCallback(ToolbarZone.ToolbarZoneRightAlign.ToString(), OnGUIRight);

				void RegisterCallback(string root, Action cb)
				{
					var toolbarZone = mRoot.Q(root);
					if (toolbarZone != null)
					{
						var parent = new VisualElement()
						{
							style = {
			flexGrow = 1,
			flexDirection = FlexDirection.Row,
			}
						};
						var container = new IMGUIContainer();
						container.onGUIHandler += () => {
							cb?.Invoke();
						};
						parent.Add(container);
						toolbarZone.Add(parent);
					}
				}
			}
		}

		if (_scenePaths == null || _scenePaths.Length != EditorBuildSettings.scenes.Length)
		{
			List<string> scenePaths = new List<string>();
			List<string> sceneNames = new List<string>();

			foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
			{
				if (scene.path == null || scene.path.StartsWith("Assets") == false)
					continue;

				string scenePath = Application.dataPath + scene.path.Substring(6);

				scenePaths.Add(scenePath);
				sceneNames.Add(Path.GetFileNameWithoutExtension(scenePath));
			}

			_scenePaths = scenePaths.ToArray();
			_sceneNames = sceneNames.ToArray();
		}
	}

	private static void OnGUILeft()
	{
		EditorGUILayout.BeginHorizontal();
		using (new EditorGUI.DisabledScope(Application.isPlaying))
		{
			string sceneName = EditorSceneManager.GetActiveScene().name;
			int sceneIndex = -1;

			for (int i = 0; i < _sceneNames.Length; ++i)
			{
				if (sceneName == _sceneNames[i])
				{
					sceneIndex = i;
					break;
				}
			}

			int newSceneIndex = EditorGUILayout.Popup(sceneIndex, _sceneNames, GUILayout.Width(100.0f));
			if (newSceneIndex != sceneIndex)
			{
				if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
				{
					EditorSceneManager.OpenScene(_scenePaths[newSceneIndex], OpenSceneMode.Single);
				}
			}
		}

/*		if (GUILayout.Button("Intro", GUILayout.Width(50)))
		{
			EditorSceneManager.OpenScene("Assets/Scenes/Intro.unity");
		}

		if (GUILayout.Button("Lobby", GUILayout.Width(50)))
		{
			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				EditorSceneManager.OpenScene("Assets/Scenes/Lobby.unity");
			}
		}

		if (GUILayout.Button("Game", GUILayout.Width(50)))
		{
			EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");
		}*/
		if (GUILayout.Button("Map", GUILayout.Width(50)))
		{
			EditorSceneManager.OpenScene("Assets/Scenes/MapGenerate.unity");
		}
		if (GUILayout.Button("TestRoom", GUILayout.Width(80)))
		{
			EditorSceneManager.OpenScene("Assets/Scenes/MapGenerate.unity");
		}
		// Prefab 빠른 열기
		if (_prefabNames != null && _prefabNames.Length > 0)
		{
			GUILayout.Space(10);
			EditorGUILayout.LabelField("Prefabs", GUILayout.Width(60));
			int prefabIndex = -1;
			int newPrefabIndex = EditorGUILayout.Popup(prefabIndex, _prefabNames, GUILayout.Width(100.0f));
			if (newPrefabIndex >= 0)
			{
				UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(_prefabPaths[newPrefabIndex]);
				if (prefab != null)
				{
					AssetDatabase.OpenAsset(prefab);
				}
			}
		}

		RoomPresetArea.DrawGizmoArea = GUILayout.Toggle(RoomPresetArea.DrawGizmoArea, "Area", "Button", GUILayout.Width(50));
		RoomPresetDoor.DrawGizmoDoor = GUILayout.Toggle(RoomPresetDoor.DrawGizmoDoor, "Door", "Button", GUILayout.Width(50));
		PropSpawner.DrawGizmoProp = GUILayout.Toggle(PropSpawner.DrawGizmoProp, "Prop", "Button", GUILayout.Width(50));

		EditorGUILayout.EndHorizontal();
	}



	private static void OnGUIRight()
	{
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("GenerateJsonMaps", GUILayout.Width(150)))
		{
		}
		if (GUILayout.Button("Player", GUILayout.Width(50)))
		{
		}

		EditorGUILayout.EndHorizontal();
	}
}