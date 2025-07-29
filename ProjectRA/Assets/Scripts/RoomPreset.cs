using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class RoomPreset : MonoBehaviour
{
	[TextArea(5, 20)]
	public string jsonInfo;

	public void SetDoorId()
	{
		RoomPresetDoor[] doors = GetComponentsInChildren<RoomPresetDoor>();

		int uniqueId = 0;
		foreach (RoomPresetDoor door in doors)
		{
			door.doorId = uniqueId++;
		}
	}

	public RoomPresetDoor[] GetDoors()
	{
		return GetComponentsInChildren<RoomPresetDoor>();
	}

	public string GenerateRoomInfo()
	{
		SetDoorId();
		RoomPresetInfo roomInfo = new RoomPresetInfo();

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
					direction = doorDirection,
					doorId = door.doorId,
				};

				roomInfo.doors.Add(doorInfo);
			}
		}
		PropSpawner[] spawners = GetComponentsInChildren<PropSpawner>();

		foreach (PropSpawner spawner in spawners)
		{
			PropSpawnerInfo spawnerInfo = new PropSpawnerInfo
			{
				itemType = spawner.itemType,
				spawnChance = 1,
				spawnCount = 1,
				positionOffset = spawner.transform.position,
				rotationOffset = spawner.transform.rotation.eulerAngles,
				scaleOffset = spawner.transform.localScale
			};
			roomInfo.spawner.Add(spawnerInfo);
		}


		RoomPresetArea[] area = GetComponentsInChildren<RoomPresetArea>();
		foreach (RoomPresetArea areaItem in area)
		{
			Vector3 center = new Vector3(
				areaItem.transform.position.x,
				areaItem.transform.position.y,
				areaItem.transform.position.z
			);
			Vector3 size = new Vector3(
				areaItem.transform.localScale.x,
				areaItem.transform.localScale.y,
				areaItem.transform.localScale.z
			);
			Bounds bound = new Bounds(center, size);
			roomInfo.bounds.Add(bound);
		}
		roomInfo.preset_key = gameObject.name;
		string jsonRoomInfo = JsonUtility.ToJson(roomInfo, true);

		return jsonRoomInfo;
	}
}
#if UNITY_EDITOR

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
			string fileName = roomPreset.gameObject.name + ".json";
			string fullPath = Path.Combine(folderPath, fileName);

			// 폴더가 없으면 생성
			if (!Directory.Exists(folderPath))
				Directory.CreateDirectory(folderPath);

			File.WriteAllText(fullPath, roomPreset.jsonInfo);
			AssetDatabase.Refresh(); // 에셋 갱신
		}
		if (GUILayout.Button("Set Door UniqueID"))
		{
			roomPreset.SetDoorId();
			// 프리팹 저장
			GameObject go = roomPreset.gameObject;
			var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
			if (prefabStage != null)
			{
				// Prefab Stage에서 작업 중일 때 자동 저장됨
				EditorUtility.SetDirty(go);
			}
			else
			{
				// 인스턴스에서 프리팹으로 변경사항 적용
				var prefab = PrefabUtility.GetCorrespondingObjectFromSource(go);
				if (prefab != null)
				{
					PrefabUtility.ApplyPrefabInstance(go, InteractionMode.UserAction);
					EditorUtility.SetDirty(prefab);
					AssetDatabase.SaveAssets();
				}
			}
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
			string fileName = obj.name + ".json";
			string fullPath = Path.Combine(folderPath, fileName);

			if (!Directory.Exists(folderPath))
				Directory.CreateDirectory(folderPath);

			File.WriteAllText(fullPath, jsonRoomInfo);
			count++;
		}
		AssetDatabase.Refresh();
	}
}

#endif