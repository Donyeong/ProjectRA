using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelManager : SingletonMono<UIPanelManager>
{
	public Transform canvas;
	public Dictionary<string, UIPanelBase> dictUI = new Dictionary<string, UIPanelBase>();
	public void ShowPanel<T>() where T : UIPanelBase
	{
		string uiName = typeof(T).Name;
		if(!dictUI.ContainsKey(uiName))
		{
			GameObject panelPrefab = ResourceManager.Instance.LoadResource<GameObject>($"UI/{uiName}");
			GameObject ui = Instantiate(panelPrefab, canvas);
			UIPanelBase uIPanelBase = ui.GetComponent<UIPanelBase>();
			dictUI.Add(uiName, uIPanelBase);
		}

		if(dictUI.TryGetValue(uiName, out UIPanelBase panel) && panel != null)
		{
			panel.gameObject.SetActive(true);
			panel.OnShowPanel();
		}
	}
	public void HidePanel<T>() where T : UIPanelBase
	{
		string uiName = typeof(T).Name;
		if (!dictUI.ContainsKey(uiName))
		{
			return;
		}

		if (dictUI.TryGetValue(uiName, out UIPanelBase panel) && panel != null)
		{
			panel.gameObject.SetActive(false);
			panel.OnHidePanel();
		}
	}
}
