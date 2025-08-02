using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    // Update is called once per frame
    void Update()
   {
        if(Input.GetKeyDown(KeyCode.O)) { 
			var panel = UIPanelManager.Instance.GetPanel<CUIPanelOption>();
            if(panel.IsShow())
			{
				UIPanelManager.Instance.HidePanel(panel);
			} else
			{
				UIPanelManager.Instance.ShowPanel(panel);
			}
        }
    }
}
