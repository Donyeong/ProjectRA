using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButtonObject : InteractableObject
{

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnInteract()
	{
		base.OnInteract();
		CGameManager.Instance.roomEventBus.Publish(new GameRoomEvent_OnStartButtonClick());
        Debug.Log("Interacted with Start Button");
	}
	public override string GetInteractText()
	{
        return "게임 시작";
	}
}

