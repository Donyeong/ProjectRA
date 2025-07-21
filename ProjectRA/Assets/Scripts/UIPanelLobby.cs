using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIPanelLobby : MonoBehaviour
{
	public Button btnHost;
	public Button btnRefresh;
	public UILobbyRoomList uILobbyRoomList;
	public void Start()
	{
		btnHost.onClick.AddListener(OnClickHost);
		btnRefresh.onClick.AddListener(OnClickRefresh);
	}
	public void OnClickHost()
	{
		Game.Instance.StartHost();
	}

	public void OnClickRefresh()
	{
		uILobbyRoomList.Refresh();
		//Game.Instance.StartClient();
	}
}
