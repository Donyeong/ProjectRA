using Mirror;
using System;
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

public class CGameUser
{
	public RAPlayer raplayer;
	public string userId;
	public string userName;
	public bool joinedVoice = false;
	public CGameUser()
	{
	}
}

[Serializable]
public class RoomOption
{
	public int buminCount = 1;
	public int mapSize = 10;
}


public class CGameManager : SingletonMono<CGameManager>
{
	public GameState gameState = GameState.Lobby;
	public RAPlayer localPlayer => RANetworkManager.instance.localPlayer;
	public EventBus<RoomEvent> roomEventBus = new EventBus<RoomEvent>();
	public EventBus<GameEvent> gameEventBus = new EventBus<GameEvent>();

	public LayerMask monsterSearchMask;

	public List<CGameUser> gameUsers = new List<CGameUser>();

	public int goalPrice = 0;

	public RoomOption roomOption = new RoomOption();


	public void Awake()
	{
		base.Awake();
		roomEventBus.AddListner<GameRoomEvent_RoomCreated>(OnRoomCreated);
	}

	public void OnRoomCreated(GameRoomEvent_RoomCreated e)
	{
		//mapManager.Generate();
	}
}
