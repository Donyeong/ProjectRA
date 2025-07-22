using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CMain : SingletonMono<CMain>
{
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartHost()
	{
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
