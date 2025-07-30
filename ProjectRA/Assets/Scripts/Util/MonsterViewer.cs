// UnityEditor 네임스페이스 필요
using UnityEngine;
using UnityEditor;

public class MonsterViewer : EditorWindow
{
	Monster monster;

	[MenuItem("Window/Monster Info")]
	public static void ShowWindow()
	{
		GetWindow<MonsterViewer>("Monster Info");
	}

	void OnGUI()
	{
		GUILayout.Label("몬스터 정보 보기", EditorStyles.boldLabel);

		monster = (Monster)EditorGUILayout.ObjectField("Monster", monster, typeof(Monster), true);

		if (monster != null)
		{
			if (monster.monsterFSM != null) {
				EditorGUILayout.LabelField("상태", monster.monsterFSM.currentState == null ? "None" : monster.monsterFSM.currentState.GetType().ToString());
			}
			// 필요한 정보 추가
		}
	}
}