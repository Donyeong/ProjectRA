#if UNITY_2021_2_OR_NEWER
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

[Overlay(typeof(SceneView), "MapToolOverlay" , true)]
public class MapToolOverlay : Overlay
{
	public override VisualElement CreatePanelContent()
	{
		var root = new VisualElement();

		// 버튼을 가로로 배치할 컨테이너
		var buttonRow = new VisualElement();
		buttonRow.style.flexDirection = FlexDirection.Row;
		buttonRow.style.justifyContent = Justify.Center;
		buttonRow.style.alignItems = Align.Center;
		buttonRow.style.marginTop = 4;
		buttonRow.style.marginBottom = 4;

		var rotateButton90p = new Button(() =>
		{
			var go = Selection.activeGameObject;
			if (go != null)
			{
				Undo.RecordObject(go.transform, "Rotate 90 p");
				go.transform.Rotate(0, 90, 0, Space.Self);
			}
			else
			{
				Debug.Log("not found selected objects.");
			}
		})
		{
			text = "90+"
		};
		rotateButton90p.style.width = 60;
		rotateButton90p.style.height = 28;
		rotateButton90p.style.marginRight = 4;

		var rotateButton90m = new Button(() =>
		{
			var go = Selection.activeGameObject;
			if (go != null)
			{
				Undo.RecordObject(go.transform, "Rotate 90 m");
				go.transform.Rotate(0, -90, 0, Space.Self);
			}
			else
			{
				Debug.Log("not found selected objects.");
			}
		})
		{
			text = "90-"
		};
		rotateButton90m.style.width = 60;
		rotateButton90m.style.height = 28;

		buttonRow.Add(rotateButton90p);
		buttonRow.Add(rotateButton90m);

		root.Add(buttonRow);

		return root;
	}
}
#endif

/*[InitializeOnLoad]
public static class SceneViewGUIExample
{
	static SceneViewGUIExample()
	{
		SceneView.duringSceneGui += OnSceneGUI;
	}

	static void OnSceneGUI(SceneView sceneView)
	{
		Handles.BeginGUI();
		GUILayout.BeginArea(new Rect(10, 10, 100, 40), "Tool", GUI.skin.box);
		if (GUILayout.Button("MapTool"))
		{
			Overlay ov;
			sceneView.TryGetOverlay("MapToolOverlay",out ov);
			if (ov != null)
			{
				ov.displayed = !ov.displayed;
			}
		}
		GUILayout.EndArea();
		Handles.EndGUI();
	}
}
#endif
*/