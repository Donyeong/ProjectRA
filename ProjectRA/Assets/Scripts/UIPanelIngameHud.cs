using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPanelIngameHud : MonoBehaviour
{
    public TMP_Text price;
    // Start is called before the first frame update
    void Start()
    {
        CGameManager.Instance.roomEventBus.AddListner<GameRoomEvent_SellPlaceUpdate>(OnSellPlaceUpdate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSellPlaceUpdate(GameRoomEvent_SellPlaceUpdate e)
	{
        price.SetText(e.sellPlace.GetTotalPrice().ToString());
	}
}
