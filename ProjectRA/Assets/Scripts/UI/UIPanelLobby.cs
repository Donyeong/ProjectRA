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
	public Button btnSetting;
	public UILobbyRoomList uILobbyRoomList;
	public void Start()
	{
		btnHost.onClick.AddListener(OnClickHost);
		btnRefresh.onClick.AddListener(OnClickRefresh);
		btnSetting.onClick.AddListener(OnClickSetting);
	}
	public void OnClickHost()
	{
		CMain.Instance.StartHost();
	}

	public void OnClickRefresh()
	{
		uILobbyRoomList.Refresh();
		//Game.Instance.StartClient();
	}
	public void OnClickSetting()
	{
		var panel = UIPanelManager.Instance.GetPanel<CUIPanelOption>();
		UIPanelManager.Instance.ShowPanel(panel);
	}
}
