using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using Unity.Services.Relay;
using UnityEngine;

public class LobbyManager : SingletonMono<LobbyManager>
{
	public async void CreateRoom(string joinCode)
	{
		var lobbyOptions = new CreateLobbyOptions
		{
			Data = new Dictionary<string, DataObject>
			{
				{ "joinCode", new DataObject(DataObject.VisibilityOptions.Public, joinCode) }
			}
		};

		Lobby createdLobby = await LobbyService.Instance.CreateLobbyAsync("My Room", 4, lobbyOptions);
	}
}
