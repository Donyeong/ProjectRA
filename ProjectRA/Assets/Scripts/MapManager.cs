using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using JetBrains.Annotations;
using System.Linq;
using System;
using UnityEngine.Experimental.Rendering;
using ReferenceTable;

[Serializable]
public class DoorInfo
{
	public Vector3 position;
	public Vector3 direction;
}
[Serializable]
public class RoomInfo
{
	public string preset_key;
	public List<Rect> rect = new List<Rect>();

	public List<DoorInfo> doors = new List<DoorInfo>();
}

public class Corridor
{
	public Vector2 start;
	public Vector2 end;
	public Corridor(Vector2 s, Vector2 e) { start = s; end = e; }
}

public class RoomGenerator
{
	public List<RoomInfo> roomPresets = new List<RoomInfo>();
	public Vector2 mapSize = new Vector2(100, 100);
	public int roomCount = 5;

	public List<RoomInfo> rooms = new List<RoomInfo>();
	public List<Rect> rects = new List<Rect>();
	public Queue<DoorInfo> openedDoors = new Queue<DoorInfo>();


	public void LoadPresetData(int mapId)
	{
		List<RefMap> refMaps = RefDataManager.Instance.GetRefDatas<RefMap>();
		List<RefMap> maps = refMaps.FindAll(x => x.map_id == mapId);
		roomPresets.Clear();
		foreach (var map in maps)
		{
			string preset_key = $"RoomPreset/{map.room_preset}";
			RoomInfo preset = JsonLoader.LoadJsonFromResources<RoomInfo>(preset_key);
			roomPresets.Add(preset);
		}
	}

	public void GenerateRooms(int mapId)
	{
		LoadPresetData(mapId);
		rooms.Clear();
		RoomInfo defaultRoom = roomPresets.First();
		AddRoom(defaultRoom);
		int maxLoop = 10000;
		int loopCount = 0;
		while(rooms.Count < roomCount)
		{
			if(loopCount > maxLoop)
			{
				Debug.LogWarning("방 생성 실패: 최대 반복 횟수 초과");
				break;
			}
			loopCount++;

			RoomInfo randomRoom = roomPresets[UnityEngine.Random.Range(0, roomPresets.Count)];

			AddRoom(randomRoom);
		}
	}

	public bool AddRoom(RoomInfo _roomInfo)
	{
		if(IsOverlapping(_roomInfo))
		{
			return false;
		}
		rooms.Add(_roomInfo);
		rects.AddRange(_roomInfo.rect);
		return true;
	}

	bool IsOverlapping(RoomInfo room)
	{
		foreach (var addedRect in rects)
		{
			foreach (var roomRect in room.rect)
			{
				if (roomRect.Overlaps(addedRect))
					return true;
			}
		}
		return false;
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
public class MapManager : MonoBehaviour
{
	public List<GameObject> RoomPresetPrefabs = new List<GameObject>();
	RoomGenerator generator = new RoomGenerator();
	public int mapId = 1;

	void Start()
	{
		generator.GenerateRooms(mapId);
	}

	public void GenerateRoomsInEditor()
	{
		generator.GenerateRooms(mapId);
	}


	void OnDrawGizmos()
	{
		if (generator == null || generator.rooms == null) return;

		Gizmos.color = Color.green;
		foreach (var rect in generator.rects)
		{
			Vector3 pos = new Vector3(rect.x + rect.width / 2, 0, rect.y + rect.height / 2);
			Vector3 size = new Vector3(rect.width, 0.1f, rect.height);
			Gizmos.DrawWireCube(pos, size);
		}
	}
}
[CustomEditor(typeof(MapManager))]
public class MapManagerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		MapManager mapManager = (MapManager)target;
		if (GUILayout.Button("방 생성"))
		{
			mapManager.GenerateRoomsInEditor();
		}
	}
}