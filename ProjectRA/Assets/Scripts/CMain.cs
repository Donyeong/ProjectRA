using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CMain : SingletonMono<CMain>
{
	protected virtual void Awake()
	{
		base.Awake();
	}

	void Start()
    {
		var panel = UIPanelManager.Instance.GetPanel<UIPanelLoading>();
		UIPanelManager.Instance.ShowPanel(panel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartHost()
	{
		var panel = UIPanelManager.Instance.GetPanel<UIPanelLoading>();
		UIPanelManager.Instance.ShowPanel(panel);
		SceneManager.LoadScene("Game");
		RANetworkManager.instance.StartHostGame(8);
	}

	public void StartClient(string joinCode)
	{
		SceneManager.LoadScene("Game");
		RANetworkManager.instance.JoinRelayServer(joinCode);
	}

    public void OnStartGame()
    {

    }
}
