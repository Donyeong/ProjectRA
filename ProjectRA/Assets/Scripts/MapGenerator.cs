using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public class MapGenerator : MonoBehaviour
{

	public void Generate()
	{
		GameObject itemPrefab = ResourceManager.Instance.LoadResource<GameObject>("Props/Prop");
		RANetworkManager.instance.spawnPrefabs.Add(itemPrefab);
		for (int i = 0; i < 10; i++)
		{
			SpawnItem(itemPrefab, new Vector3(Random.Range(-2f, 2f), 1, Random.Range(-2f, 2f)), Random.RandomRange(0, 3));
		}
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
