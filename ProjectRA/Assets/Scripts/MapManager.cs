using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using JetBrains.Annotations;
using System.Linq;
using System;

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
	public List<RoomPreset> RoomPresetPrefabs = new List<RoomPreset>();
	public Vector2 mapSize = new Vector2(100, 100);
	public int roomCount = 5;

	public List<RoomInfo> rooms = new List<RoomInfo>();
	public List<Rect> rects = new List<Rect>();

	public void GenerateRooms()
	{

		rooms.Clear();
		RoomPreset defaultRoom = RoomPresetPrefabs.First();

		//RoomInfo roomInfo = defaultRoom.GenerateRoomInfo();
		//AddRoom(roomInfo);
	}

	public void AddRoom(RoomInfo _roomInfo)
	{
		rooms.Add(_roomInfo);
		rects.AddRange(_roomInfo.rect);
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

public class MapManager : MonoBehaviour
{
	public List<GameObject> RoomPresetPrefabs = new List<GameObject>();
	RoomGenerator generator = new RoomGenerator();

	void Start()
	{
		generator.RoomPresetPrefabs.Clear();
		foreach (var prefab in RoomPresetPrefabs)
		{
			RoomPreset roomPreset = prefab.GetComponent<RoomPreset>();
			if (roomPreset != null)
			{
				generator.RoomPresetPrefabs.Add(roomPreset);
			}
		}
		generator.GenerateRooms();
	}

	public void GenerateRoomsInEditor()
	{
		generator.GenerateRooms();
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
		if (GUILayout.Button("¹æ »ý¼º"))
		{
			mapManager.GenerateRoomsInEditor();
		}
	}
}