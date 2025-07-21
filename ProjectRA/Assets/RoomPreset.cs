using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class RoomPreset : MonoBehaviour
{
	public string preset_key;
	[TextArea(5, 20)]
	public string jsonInfo;
	public string GenerateRoomInfo()
	{
		RoomInfo roomInfo = new RoomInfo();

		RoomPresetDoor[] doors = GetComponentsInChildren<RoomPresetDoor>();

		foreach (RoomPresetDoor door in doors)
		{
			if (door.spawnChance > 0)
			{
				Vector3 doorPosition = door.transform.position;
				Vector3 doorDirection = door.transform.forward;

				DoorInfo doorInfo = new DoorInfo
				{
					position = doorPosition,
					direction = doorDirection
				};

				roomInfo.doors.Add(doorInfo);
			}
		}

		RoomPresetArea[] area = GetComponentsInChildren<RoomPresetArea>();
		foreach (RoomPresetArea areaItem in area)
		{
			Rect rect = new Rect(
				areaItem.transform.position.x - areaItem.transform.localScale.x / 2,
				areaItem.transform.position.z - areaItem.transform.localScale.z / 2,
				areaItem.transform.localScale.x,
				areaItem.transform.localScale.z
			);
			roomInfo.rect.Add(rect);
		}
		roomInfo.preset_key = preset_key;

		string jsonRoomInfo = JsonUtility.ToJson(roomInfo, true);

		return jsonRoomInfo;
	}
}

[CustomEditor(typeof(RoomPreset))]
public class RoomPresetEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		RoomPreset roomPreset = (RoomPreset)target;
		if (GUILayout.Button("Generate Data"))
		{
			roomPreset.jsonInfo = roomPreset.GenerateRoomInfo();

			string folderPath = "Assets/Resources/RoomPresets";
			string fileName = roomPreset.preset_key + ".json";
			string fullPath = Path.Combine(folderPath, fileName);

			// 폴더가 없으면 생성
			if (!Directory.Exists(folderPath))
				Directory.CreateDirectory(folderPath);

			File.WriteAllText(fullPath, roomPreset.jsonInfo);
			AssetDatabase.Refresh(); // 에셋 갱신
		}
	}

}



public class RoomPresetJsonGenerator
{
	[MenuItem("Assets/Generate RoomPreset JSON", true)]
	static bool ValidateGenerateJson()
	{
		// 여러 개 선택 가능, 프리팹+RoomPreset만 허용
		foreach (var obj in Selection.objects)
		{
			var go = obj as GameObject;
			if (go != null && PrefabUtility.GetPrefabAssetType(go) != PrefabAssetType.NotAPrefab
				&& go.GetComponent<RoomPreset>() != null)
				return true;
		}
		return false;
	}

	[MenuItem("Assets/Generate RoomPreset JSON")]
	static void GenerateJson()
	{
		int count = 0;
		foreach (var obj in Selection.objects)
		{
			var go = obj as GameObject;
			if (go == null) continue;
			if (PrefabUtility.GetPrefabAssetType(go) == PrefabAssetType.NotAPrefab) continue;

			var roomPreset = go.GetComponent<RoomPreset>();
			if (roomPreset == null) continue;

			string jsonRoomInfo = roomPreset.GenerateRoomInfo();
			string folderPath = "Assets/Resources/RoomPresets";
			string fileName = roomPreset.preset_key + ".json";
			string fullPath = Path.Combine(folderPath, fileName);

			if (!Directory.Exists(folderPath))
				Directory.CreateDirectory(folderPath);

			File.WriteAllText(fullPath, jsonRoomInfo);
			count++;
		}
		AssetDatabase.Refresh();
	}
}