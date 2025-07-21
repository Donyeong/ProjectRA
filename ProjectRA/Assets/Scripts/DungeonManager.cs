using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : SingletonMono<DungeonManager>
{
	public MapGenerator mapGenerator;
	public EventBus<DungeonEvent> eventBus = new EventBus<DungeonEvent>();


	public void Awake()
	{
		eventBus.AddListner<DungeonEvent_DungeonStart>(OnDungeonStart);
		mapGenerator = GetComponent<MapGenerator>();
	}

	public void OnDungeonStart(DungeonEvent_DungeonStart e)
	{
		mapGenerator.Generate();
	}
}
