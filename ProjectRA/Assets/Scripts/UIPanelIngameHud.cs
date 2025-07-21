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
        DungeonManager.Instance.eventBus.AddListner<DungeonEvent_SellPlaceUpdate>(OnSellPlaceUpdate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSellPlaceUpdate(DungeonEvent_SellPlaceUpdate e)
	{
        price.SetText(e.sellPlace.GetTotalPrice().ToString());
	}
}
