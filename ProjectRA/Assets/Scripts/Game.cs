using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : SingletonMono<Game>
{
    public EventBus<GameEvent> eventBus = new EventBus<GameEvent>();
    // Start is called before the first frame update
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
