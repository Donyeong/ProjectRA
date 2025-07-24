using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPanelIngameHud : MonoBehaviour
{
    public TMP_Text price;
    public GameObject interactInfo;
	public TMP_Text interactInfoText;
	public GameObject propPrice;
	public TMP_Text propPriceText;
	public RAProp target = null;
	// Start is called before the first frame update
	void Start()
    {
        CGameManager.Instance.roomEventBus.AddListner<GameRoomEvent_SellPlaceUpdate>(OnSellPlaceUpdate);
		CGameManager.Instance.roomEventBus.AddListner<GameRoomEvent_OnInteractAimIn>(OnInteractAimIn);
		CGameManager.Instance.roomEventBus.AddListner<GameRoomEvent_OnInteractAimOut>(OnInteractAimOut);
		CGameManager.Instance.roomEventBus.AddListner<GameRoomEvent_OnGrabProp>(OnGrabProp);
		CGameManager.Instance.roomEventBus.AddListner<GameRoomEvent_OnDropProp>(OnDropProp);
		CGameManager.Instance.roomEventBus.AddListner<GameRoomEvent_OnDamageProp>(OnDamageProp);

	}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSellPlaceUpdate(GameRoomEvent_SellPlaceUpdate e)
	{
        price.SetText(e.sellPlace.GetTotalPrice().ToString());
	}

	public void OnInteractAimIn(GameRoomEvent_OnInteractAimIn e)
	{
        interactInfo.SetActive(true);
		interactInfoText.SetText(e.interactableObject.GetInteractText());
	}

	public void OnInteractAimOut(GameRoomEvent_OnInteractAimOut e)
	{
		interactInfo.SetActive(false);
	}

	public void OnGrabProp(GameRoomEvent_OnGrabProp e)
	{
		propPrice.SetActive(true);
		propPriceText.SetText(e.targetProp.price.ToString());
		target = e.targetProp;
	}
	public void OnDropProp(GameRoomEvent_OnDropProp e)
	{
		propPrice.SetActive(false);
		target = null;
	}

	public void OnDamageProp(GameRoomEvent_OnDamageProp e)
	{
		if(propPrice.activeInHierarchy)
		{
			if (e.targetProp == target)
			{
				propPriceText.SetText(e.targetProp.price.ToString());
			}
		}
	}
}
