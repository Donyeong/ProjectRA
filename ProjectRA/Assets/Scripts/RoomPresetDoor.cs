using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RoomPresetDoor : MonoBehaviour
{
	public int doorId;
	public GameObject door;
	public GameObject wall;
	public float spawnChance = 1f;
	//Door가 바라보는 방향을 GIzmo로 표시
#if UNITY_EDITOR
	public static bool DrawGizmoDoor;
	void OnDrawGizmos()
	{
		if (!DrawGizmoDoor)
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
		Vector3 direction = transform.forward * 2f; // 2 유닛 길이
		Vector3 end = start + direction;
		Gizmos.DrawLine(transform.position, start);

		Gizmos.DrawLine(start, end);
		// 화살표 머리
		Gizmos.DrawLine(end, end + Quaternion.Euler(0, 150, 0) * direction * 0.3f);
		Gizmos.DrawLine(end, end + Quaternion.Euler(0, -150, 0) * direction * 0.3f);
	}
#endif


	public void RotateDoor(float _v)
	{
		transform.Rotate(0, _v, 0, Space.World);
	}

	public void SetDoorMode(bool isDoor)
	{
		if(isDoor)
		{
			if(door != null)
				door.SetActive(true);
			if (wall != null)
				wall.SetActive(false);
		} else
		{
			if (wall != null)
				wall.SetActive(true);
			if (door != null)
				door.SetActive(false);
		}
	}

}
#if UNITY_EDITOR
[CustomEditor(typeof(RoomPresetDoor))]
public class RoomPresetDoorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		RoomPresetDoor door = (RoomPresetDoor)target;
		if (GUILayout.Button("+90도"))
		{
			Undo.RecordObject(door.transform, "D+90도");
			door.RotateDoor(90);
		}
		if (GUILayout.Button("-90도"))
		{
			Undo.RecordObject(door.transform, "D-90도");
			door.RotateDoor(-90);
		}
		if (GUILayout.Button("SetDoorMode"))
		{
			Undo.RecordObject(door.transform, "D-SetDoorMode");
			door.SetDoorMode(true);
		}
		if (GUILayout.Button("SetWallMode"))
		{
			Undo.RecordObject(door.transform, "D-SetWallMode");
			door.SetDoorMode(false);
		}
	}
}
#endif