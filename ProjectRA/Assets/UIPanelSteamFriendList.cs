using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelSteamFriendList : MonoBehaviour
{
	public GameObject origin;
	public List<PanelInviteItemController> InviteItems = new List<PanelInviteItemController>();
	public void Start()
	{
		SetList();
	}

	public void SetList()
	{
		origin.SetActive(false);
		Clear();
		var friends = SteamManager.Instance.GetFriendList();
		foreach (var friend in friends)
		{
			var inviteItem = Instantiate(origin, origin.transform.parent);
			inviteItem.SetActive(true);
			PanelInviteItemController controller = inviteItem.GetComponent<PanelInviteItemController>();
			controller.PlayerName = SteamManager.Instance.GetSteamUserName((Steamworks.CSteamID)friend.m_SteamID);
			controller.PlayerSteamID = friend.m_SteamID;
			controller.Init();

			InviteItems.Add(controller);
		}
	}

	public void Clear()
	{
		foreach (var item in InviteItems)
		{
			if (item != null)
			{
				Destroy(item.gameObject);
			}
		}
		InviteItems.Clear();
	}
}
