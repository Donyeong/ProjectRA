using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGameManager : SingletonMono<CGameManager>
{
	public GameMapGenerator mapGenerator;
	public EventBus<RoomEvent> roomEventBus = new EventBus<RoomEvent>();
	public EventBus<GameEvent> gameEventBus = new EventBus<GameEvent>();


	public void Awake()
	{
		roomEventBus.AddListner<GameRoomEvent_RoomCreated>(OnRoomCreated);
		mapGenerator = GetComponent<GameMapGenerator>();
	}

	public void OnRoomCreated(GameRoomEvent_RoomCreated e)
	{
		mapGenerator.Generate();
	}
}
