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
		SetRole();
		MapManager.Instance.GameMapStart(e);
		CGameManager.Instance.localPlayer.playerMovement.Warp(Vector3.zero);

		CGameManager.Instance.gameState = GameState.Game;
	}

	public void SetRole()
	{
		int maxUser = CGameManager.Instance.gameUsers.Count;
		//유저중 한명은 Role을 범인으로
		int randomIndex = Random.Range(0, maxUser);
		for (int i = 0; i < maxUser; i++)
		{
			CGameUser gameUser = CGameManager.Instance.gameUsers[i];
			gameUser.raplayer.roleType = eRoleType.Citizen; //기본은 시민
			if(randomIndex == i)
			{
				gameUser.raplayer.roleType = eRoleType.Bumin;
			}
		}
	}
}
