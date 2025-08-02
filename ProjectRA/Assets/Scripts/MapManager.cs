using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.UIElements;

public class MapManager : SingletonNetworkBehaviour<MapManager>
{
	public GameObject lobbyMap;
	public MapGenerator mapGenerator;
	public NavMeshSurface navMeshSurface;

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

	float spawnCool = 0;

	public void Update()
	{
		if (isServer)
		{
			if (CGameManager.Instance.gameState == GameState.Game)
			{
				spawnCool -= Time.deltaTime;
				if (spawnCool < 0)
				{
					spawnCool = Random.RandomRange(90, 120);
					SpawnMonster();
				}
			}
		}
	}

	public void GameMapStart(GameRoomEvent_OnStartButtonClick e)
	{
		Debug.Log("CmdGameMapStart");
		Generate();
		mapGenerator.GenerateRoomObject();
		lobbyMap.SetActive(false);
		//SpawnMonster();
		RpcGameMapStart(mapGenerator.generator.rooms);
	}

	[ClientRpc]
	public void RpcGameMapStart(List<RoomInfo> roomInfos)
	{
		Debug.Log("RpcGameMapStart");
		mapGenerator.generator.rooms = roomInfos;
		if (!isServer)
		{
			lobbyMap.SetActive(false);
			mapGenerator.GenerateRoomObject();
		}
		navMeshSurface.BuildNavMesh();
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
				SpawnItem(itemPrefab, room.position + room.rotation * spawner.positionOffset, Random.RandomRange(0, 3));
			}
			
		}

		foreach(CGameUser user in CGameManager.Instance.gameUsers)
		{
			user.raplayer.transform.position = Vector3.zero;
		}

		GameObject cart = ResourceManager.Instance.LoadResource<GameObject>("Props/Cart");
		SpawnItem(cart, Vector3.up + Vector3.right);
	}

	public void SpawnMonster()
	{
		GameObject mob = ResourceManager.Instance.LoadResource<GameObject>("Monster/mob");
		CmdSpawnMonster(mob, GetRandomSpawnPoint());
	}

	public Vector3 GetRandomSpawnPoint()
	{
		MonsterSpawner[] spawners = FindObjectsOfType<MonsterSpawner>();
		if (spawners.Length == 0)
		{
			Debug.LogWarning("No MonsterSpawner found in the scene.");
			return Vector3.zero;
		}
		int randomIndex = Random.Range(0, spawners.Length);
		Vector3 spawnPosition = spawners[randomIndex].transform.position;
		Debug.Log($"Random spawn position: {spawnPosition}");
		return spawnPosition;
	}

	[Server]
	public void SpawnItem(GameObject _prefab, Vector3 position, int propId = -1)
	{
		GameObject item = Instantiate(_prefab, position, Quaternion.identity);
		RAProp prop = item.GetComponent<RAProp>();
		if (propId != -1) {
			prop.ServerSetProp(propId);
		} else
		{
			prop.isInit = true;
		}
		NetworkServer.Spawn(item);
	}

	[Server]
	public void CmdSpawnMonster(GameObject _prefab, Vector3 position)
	{
		GameObject item = Instantiate(_prefab, position, Quaternion.identity);
		NetworkServer.Spawn(item);
	}
}
