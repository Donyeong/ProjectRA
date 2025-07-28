using Edgegap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : SingletonMono<DungeonManager>
{
	// Start is called before the first frame update
	void Start()
	{
		CGameManager.Instance.roomEventBus.AddListner<GameRoomEvent_OnStartButtonClick>(OnStart);
	}

	public void OnStart(GameRoomEvent_OnStartButtonClick e)
	{
		MapManager.Instance.GameMapStart(e);
		CGameManager.Instance.localPlayer.playerMovement.Warp(Vector3.zero);

		CGameManager.Instance.gameState = GameState.Game;
	}
}
