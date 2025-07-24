using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGameManager : SingletonMono<CGameManager>
{
	public RAPlayer localPlayer => RANetworkManager.instance.localPlayer;
	public MapManager mapManager;
	public EventBus<RoomEvent> roomEventBus = new EventBus<RoomEvent>();
	public EventBus<GameEvent> gameEventBus = new EventBus<GameEvent>();


	public void Awake()
	{
		roomEventBus.AddListner<GameRoomEvent_RoomCreated>(OnRoomCreated);
		mapManager = GetComponent<MapManager>();
	}

	public void OnRoomCreated(GameRoomEvent_RoomCreated e)
	{
		//mapManager.Generate();
	}
}
