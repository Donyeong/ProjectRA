// UnityEditor ���ӽ����̽� �ʿ�
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
		GUILayout.Label("���� ���� ����", EditorStyles.boldLabel);

		monster = (Monster)EditorGUILayout.ObjectField("Monster", monster, typeof(Monster), true);

		if (monster != null)
		{
			if (monster.monsterFSM != null) {
				EditorGUILayout.LabelField("����", monster.monsterFSM.currentState == null ? "None" : monster.monsterFSM.currentState.GetType().ToString());
			}
			// �ʿ��� ���� �߰�
		}
	}
}