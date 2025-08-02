using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSettingBtnObject : InteractableObject
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
		CGameManager.Instance.roomEventBus.Publish(new GameRoomEvent_OnRoomSettingButtonClick());
		UIPanelRoomOption uIPanelRoomOption = UIPanelManager.Instance.GetPanel<UIPanelRoomOption>();
		UIPanelManager.Instance.ShowPanel(uIPanelRoomOption);
	}
	public override string GetInteractText()
	{
		return "¹æ ¼³Á¤";
	}
}
