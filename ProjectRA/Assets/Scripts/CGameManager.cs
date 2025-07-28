using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
	Lobby,
	Room,
	Game,
	Result
}
public class CGameManager : SingletonMono<CGameManager>
{
	public GameState gameState = GameState.Lobby;
	public RAPlayer localPlayer => RANetworkManager.instance.localPlayer;
	public EventBus<RoomEvent> roomEventBus = new EventBus<RoomEvent>();
	public EventBus<GameEvent> gameEventBus = new EventBus<GameEvent>();


	public void Awake()
	{
		roomEventBus.AddListner<GameRoomEvent_RoomCreated>(OnRoomCreated);
	}

	public void OnRoomCreated(GameRoomEvent_RoomCreated e)
	{
		//mapManager.Generate();
	}
}
