#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ForceRecompile : EditorWindow
{
	[MenuItem("Tools/Force Recompile")]
	static void Recompile()
	{
		AssetDatabase.Refresh();
		UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
		Debug.Log("Forced script recompilation triggered.");
	}
}
#endif