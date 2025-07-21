using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyRoomSlot : MonoBehaviour
{
    private Lobby lobby;
	private string joinCode = string.Empty;

    public TMP_Text textJoinCode;
	public Button btnJoin;

	public void Awake()
	{
		btnJoin.onClick.AddListener(OnClickJoin);
	}


	public void SetLobbyData(Lobby _lobby)
	{
        lobby = _lobby;
		joinCode = lobby.Data["joinCode"].Value;
		textJoinCode.text = "Join Code: " + joinCode;
	}

    public void OnClickJoin()
    {
        if (lobby == null) return;
        Debug.Log("Join Lobby: " + lobby.Name + " with join code: " + joinCode);
        Game.Instance.StartClient(joinCode);

    }
}
