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
        UIPanelManager.Instance.ShowPanel<UIPanelLoading>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartHost()
	{
		UIPanelManager.Instance.ShowPanel<UIPanelLoading>();
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
