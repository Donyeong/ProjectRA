using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public class MapManager : MonoBehaviour
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
		CGameManager.Instance.roomEventBus.AddListner<GameRoomEvent_OnStartButtonClick>(OnStart);
	}

	public void OnStart(GameRoomEvent_OnStartButtonClick e)
	{
		Generate();
		lobbyMap.SetActive(false);
		Debug.Log("MapManager Start ");
	}

	public void Generate()
	{
		mapGenerator.GenerateRooms();
		List<PropSpawnerInfo> spawnerInfos = mapGenerator.GetAllSpawner();

		GameObject itemPrefab = ResourceManager.Instance.LoadResource<GameObject>("Props/Prop");
		RANetworkManager.instance.spawnPrefabs.Add(itemPrefab);
		Debug.Log("Spawner Count" + spawnerInfos.Count);
		foreach(PropSpawnerInfo spawnerInfo in spawnerInfos)
		{
			SpawnItem(itemPrefab, spawnerInfo.positionOffset, Random.RandomRange(0, 3));
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
