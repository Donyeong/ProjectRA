using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using JetBrains.Annotations;
using System.Linq;
using System;
using UnityEngine.Experimental.Rendering;
using ReferenceTable;
using UnityEditor.Presets;

[Serializable]
public class PropSpawnerInfo
{
	public eItemType itemType = eItemType.small;
	public int spawnChance = 100; // 0~100
	public int spawnCount = 1; // 한번에 생성할 개수
	public Vector3 positionOffset = Vector3.zero;
	public Vector3 rotationOffset = Vector3.zero;
	public Vector3 scaleOffset = Vector3.one;
}

[Serializable]
public class DoorInfo
{
	public Vector3 position;
	public Vector3 direction;
}
[Serializable]
public class RoomPresetInfo
{
	public string preset_key;
	public List<Bounds> bounds = new List<Bounds>();

	public List<DoorInfo> doors = new List<DoorInfo>();

	public List<PropSpawnerInfo> spawner = new List<PropSpawnerInfo>();
}

public class RoomInfo : RoomPresetInfo
{
	public Vector3 position;
	public Quaternion rotation;
	public void CopyFrom(RoomPresetInfo preset)
	{
		preset_key = preset.preset_key;
		bounds = new List<Bounds>(preset.bounds);
		doors = new List<DoorInfo>();
		foreach(var d in preset.doors)
		{
			doors.Add(new DoorInfo
			{
				position = d.position,
				direction = d.direction
			});
		}
		position = Vector3.zero;
		rotation = Quaternion.identity;
	}
	public static void RotateRoomAroundY(RoomInfo room, float angleInDegrees)
	{
		Quaternion rotation = Quaternion.Euler(0, angleInDegrees, 0);

		// 1. 문 위치 및 방향 회전
		foreach (var door in room.doors)
		{
			door.position = rotation * door.position;
			door.direction = rotation * door.direction;
		}

		// 2. Bounds 회전
		for (int i = 0; i < room.bounds.Count; i++)
		{
			Bounds original = room.bounds[i];

			Bounds rotatedBounds = RotateBoundsY(original, angleInDegrees);

			room.bounds[i] = rotatedBounds;
		}
		room.rotation = Quaternion.Euler(0, angleInDegrees, 0) * room.rotation;
	}
	public static Bounds RotateBoundsY(Bounds bounds, float angleInDegrees)
	{
		Quaternion rotation = Quaternion.Euler(0, angleInDegrees, 0);

		Vector3 center = bounds.center;
		Vector3 extents = bounds.extents;

		// Bounds 꼭짓점 8개 계산
		Vector3[] corners = new Vector3[8];
		int i = 0;
		for (int x = -1; x <= 1; x += 2)
		{
			for (int y = -1; y <= 1; y += 2)
			{
				for (int z = -1; z <= 1; z += 2)
				{
					Vector3 corner = new Vector3(
						center.x + extents.x * x,
						center.y + extents.y * y,
						center.z + extents.z * z
					);
					corners[i++] = rotation * corner; // (0,0,0)을 기준으로 회전
				}
			}
		}

		// 회전된 꼭짓점들로 새로운 Bounds 생성
		Bounds rotatedBounds = new Bounds(corners[0], Vector3.zero);
		for (int j = 1; j < 8; j++)
		{
			rotatedBounds.Encapsulate(corners[j]);
		}

		return rotatedBounds;
	}
}

public class OpenedDoorInfo
{
	public RoomInfo roomInfo;
	public DoorInfo doorInfo;
}
public class RoomGenerator
{
	public List<RoomPresetInfo> roomPresets = new List<RoomPresetInfo>();
	public Vector2 mapSize = new Vector2(100, 100);
	public int roomCount = 50;

	public List<RoomInfo> rooms = new List<RoomInfo>();
	public List<Bounds> bounds = new List<Bounds>();
	public Queue<OpenedDoorInfo> openedDoors = new Queue<OpenedDoorInfo>();


	public void LoadPresetData(int mapId)
	{
		List<RefMap> refMaps = RefDataManager.Instance.GetRefDatas<RefMap>();
		List<RefMap> maps = refMaps.FindAll(x => x.map_id == mapId);
		roomPresets.Clear();
		foreach (var map in maps)
		{
			string preset_key = $"RoomPresets/{map.room_preset}";
			RoomPresetInfo preset = JsonLoader.LoadJsonFromResources<RoomPresetInfo>(preset_key);
			roomPresets.Add(preset);
		}
	}

	public void GenerateRooms(int mapId, int mapSize)
	{
		roomCount = mapSize;
		LoadPresetData(mapId);
		rooms.Clear();
		bounds.Clear();
		RoomPresetInfo defaultRoomPreset = roomPresets.First();
		openedDoors.Clear();
		RoomInfo defalutRoom = new RoomInfo();
		defalutRoom.CopyFrom(defaultRoomPreset);
		AddRoom(defalutRoom);
		int maxLoop = 3000;
		int loopCount = 0;
		while (rooms.Count < roomCount)
		{
			if(loopCount > maxLoop)
			{
				Debug.LogWarning("방 생성 실패: 최대 반복 횟수 초과");
				break;
			}
			loopCount++;
			if(openedDoors.Count == 0)
			{
				Debug.LogWarning("방 생성 실패: 문이 달린 방이 없음");
				break;
			}

			OpenedDoorInfo targetDoor = openedDoors.Dequeue();

			RoomPresetInfo randomPreset = roomPresets[UnityEngine.Random.Range(0, roomPresets.Count)];

			RoomInfo roomInfo = new RoomInfo();
			roomInfo.CopyFrom(randomPreset);
			List<DoorInfo> randomDoors = new List<DoorInfo>(randomPreset.doors);
			for (int i = 0; i < randomDoors.Count; i++)
			{
				int rnd = UnityEngine.Random.Range(i, randomDoors.Count);
				(randomDoors[i], randomDoors[rnd]) = (randomDoors[rnd], randomDoors[i]);
			}
			bool added = false;
			RoomInfo.RotateRoomAroundY(roomInfo, UnityEngine.Random.Range(0, 4) * 90f);
			foreach (var door in roomInfo.doors) {
				roomInfo.position = targetDoor.roomInfo.position + targetDoor.doorInfo.position - door.position;

				if (AddRoom(roomInfo))
				{
					Debug.Log($"Add Room {randomPreset.preset_key}");
					added = true;
					break;
				}
				else
				{
					Debug.LogWarning($"방 생성 실패: {randomPreset.preset_key}가 겹침");
				}
			}
			if (!added)
			{
				openedDoors.Enqueue(targetDoor);
			}
		}
	}

	public bool AddRoom(RoomInfo _roomInfo)
	{
		if(IsOverlapping(_roomInfo))
		{
			return false;
		}
		rooms.Add(_roomInfo);
		foreach(var roomBounds in _roomInfo.bounds) {
			Bounds tempBounds = roomBounds;
			tempBounds.center += _roomInfo.position;
			bounds.Add(tempBounds);
		}

		foreach(var door in _roomInfo.doors)
		{
			OpenedDoorInfo openedDoor = new OpenedDoorInfo();
			openedDoor.roomInfo = _roomInfo;
			openedDoor.doorInfo = door;
			openedDoors.Enqueue(openedDoor);
		}
		return true;
	}

	bool IsOverlapping(RoomInfo room)
	{
		foreach (var addedBounds in bounds)
		{
			foreach (var roomBounds in room.bounds)
			{
				Bounds tempBoundstA = roomBounds;
				Bounds tempBoundsB = addedBounds;
				tempBoundstA.center += room.position;
				if (IsStrictlyOverlapping(tempBoundstA,tempBoundsB))
					return true;
			}
		}
		return false;
	}
	bool IsStrictlyOverlapping(Bounds a, Bounds b)
	{
		// 각 축별로 "완전히 겹치는" 경우만 true
		// 면이 딱 맞닿은 경우(=, ==)는 false
		return
			a.min.x < b.max.x && a.max.x > b.min.x &&
			a.min.y < b.max.y && a.max.y > b.min.y &&
			a.min.z < b.max.z && a.max.z > b.min.z;
	}
}
public class JsonLoader
{
	public static T LoadJsonFromResources<T>(string resourcePath)
	{
		// 확장자(.json)는 생략
		TextAsset jsonText = Resources.Load<TextAsset>(resourcePath);
		if (jsonText == null)
		{
			Debug.LogError($"리소스에서 {resourcePath}를 찾을 수 없습니다.");
			return default;
		}
		return JsonUtility.FromJson<T>(jsonText.text);
	}
}
public class MapGenerator : SingletonMono<MapGenerator>
{
	public bool drawPropGizmos = true; // Gizmos를 그릴지 여부
	public bool drawMapGizmos = true; // Gizmos를 그릴지 여부
	public List<GameObject> RoomPresetPrefabs = new List<GameObject>();
	RoomGenerator generator = new RoomGenerator();
	public int mapId = 1;
	public int mapSize = 50;

	void Start()
	{
		generator.GenerateRooms(mapId, mapSize);
	}

	public void GenerateRooms()
	{
		generator.GenerateRooms(mapId, mapSize);
		GenerateRoomObject();
	}

	public List<PropSpawnerInfo> GetAllSpawner()
	{
		List<PropSpawnerInfo> res = new List<PropSpawnerInfo>();
		foreach (var room in generator.rooms)
		{
			foreach (var spawner in room.spawner)
			{
				//if (spawner.spawnCount <= 0 || spawner.spawnChance <= 0) continue;
				res.Add(spawner);
			}
		}
		return res;
	}

	public void GenerateRoomObject()
	{
		GameObject existingMap = GameObject.Find("Map");
		if (existingMap != null)
		{
			DestroyImmediate(existingMap);
		}
		
		GameObject mapHolder = new GameObject("Map");
		if (generator.rooms == null || generator.rooms.Count == 0)
		{
			Debug.LogWarning("방 정보가 없습니다. 먼저 방을 생성하세요.");
			return;
		}

		foreach (var roomInfo in generator.rooms)
		{
			GameObject roomPrefab= ResourceManager.Instance.LoadResource<GameObject>($"RoomPresetPrefabs/{roomInfo.preset_key}");
			GameObject roomObject = Instantiate(roomPrefab, roomInfo.position, roomInfo.rotation, mapHolder.transform);
			roomObject.transform.localScale = Vector3.one; // 스케일 조정
			roomObject.name = roomInfo.preset_key;
		}
		Debug.Log("방 오브젝트 생성 완료");
	}


	void OnDrawGizmos()
	{
/*		if (generator == null || generator.rooms == null) return;

		Gizmos.color = Color.green;
		foreach (var bound in generator.bounds)
		{
			Vector3 pos = bound.center;
			Vector3 size = bound.size;
			Gizmos.DrawWireCube(pos, size);
		}*/
	}
}
[CustomEditor(typeof(MapGenerator))]
public class MapManagerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		MapGenerator mapManager = (MapGenerator)target;
		if (GUILayout.Button("방 생성"))
		{
			mapManager.GenerateRooms();
		}
		if (GUILayout.Button("엑셀 데이터 Reload"))
		{
			RefDataManager.Instance.LoadRefData();
		}
	}
}