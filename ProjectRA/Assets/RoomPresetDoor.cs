using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomPresetDoor : MonoBehaviour
{
	public float spawnChance = 1f;
	//Door�� �ٶ󺸴� ������ GIzmo�� ǥ��

	void OnDrawGizmos()
	{
		MapGenerator mapManager = MapGenerator.Instance;
		if (mapManager != null && !mapManager.drawMapGizmos)
		{
			return;
		}
		Color gizmoColor;
		if (spawnChance >= 0.7f)
			gizmoColor = Color.green;
		else if (spawnChance >= 0.3f)
			gizmoColor = Color.yellow;
		else
			gizmoColor = Color.red;

		Gizmos.color = gizmoColor;
		Vector3 start = transform.position + Vector3.up;
		Vector3 direction = transform.forward * 2f; // 2 ���� ����
		Vector3 end = start + direction;
		Gizmos.DrawLine(transform.position, start);

		Gizmos.DrawLine(start, end);
		// ȭ��ǥ �Ӹ�
		Gizmos.DrawLine(end, end + Quaternion.Euler(0, 150, 0) * direction * 0.3f);
		Gizmos.DrawLine(end, end + Quaternion.Euler(0, -150, 0) * direction * 0.3f);
	}



	public void RotateDoor(float _v)
	{
		transform.Rotate(0, _v, 0, Space.World);
	}


}

[CustomEditor(typeof(RoomPresetDoor))]
public class RoomPresetDoorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		RoomPresetDoor door = (RoomPresetDoor)target;
		if (GUILayout.Button("+90��"))
		{
			Undo.RecordObject(door.transform, "D+90��");
			door.RotateDoor(90);
		}
		if (GUILayout.Button("-90��"))
		{
			Undo.RecordObject(door.transform, "D-90��");
			door.RotateDoor(-90);
		}
	}
}