using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public class MapManager : SingletonMono<MapManager>
{
	public GameObject lobbyMap;
	public MapGenerator mapGenerator;

	public void Awake()
	{
		if (mapGenerator == null)
		{
			mapGenerator = GetComponent<MapGenerator>();
		}
	}

	public void Start()
	{
	}

	public void GameMapStart(GameRoomEvent_OnStartButtonClick e)
	{
		Generate();
		lobbyMap.SetActive(false);
		Debug.Log("MapManager Start ");
	}

	public void Generate()
	{
		mapGenerator.GenerateRooms();

		GameObject itemPrefab = ResourceManager.Instance.LoadResource<GameObject>("Props/Prop");
		RANetworkManager.instance.spawnPrefabs.Add(itemPrefab);
		foreach (var room in mapGenerator.generator.rooms)
		{
			foreach (var spawner in room.spawner)
			{
				SpawnItem(itemPrefab, room.position + spawner.positionOffset, Random.RandomRange(0, 3));
			}
		}

		RANetworkManager.instance.localPlayer.transform.position = Vector3.zero;


	}

	[Server]
	public void SpawnItem(GameObject _prefab, Vector3 position, int propId)
	{
		GameObject item = Instantiate(_prefab, position, Quaternion.identity);
		RAProp prop = item.GetComponent<RAProp>();
		prop.ServerSetProp(propId);
		NetworkServer.Spawn(item);
	}
}
