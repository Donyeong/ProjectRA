using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIPanelManager : SingletonMono<UIPanelManager>
{
	public Transform canvas;
	public Dictionary<string, UIPanelBase> dictUI = new Dictionary<string, UIPanelBase>();

	public List<UIPanelBase> opnedUI = new List<UIPanelBase>();
	public bool isUILock => opnedUI.Count > 0;
	public T GetPanel<T>() where T : UIPanelBase
	{
		string uiName = typeof(T).Name;
		if (!dictUI.ContainsKey(uiName))
		{
			GameObject panelPrefab = ResourceManager.Instance.LoadResource<GameObject>($"UI/{uiName}");
			GameObject ui = Instantiate(panelPrefab, canvas);
			UIPanelBase uIPanelBase = ui.GetComponent<UIPanelBase>();
			dictUI.Add(uiName, uIPanelBase);
			ui.SetActive(false);
		}
		return dictUI[uiName] as T;
	}
	public void ShowPanel(UIPanelBase panel)
	{
		panel.gameObject.SetActive(true);
		panel.OnShowPanel();
		if(!opnedUI.Any(i => i == panel))
		{
			opnedUI.Add(panel);
		}
	}
	public void HidePanel(UIPanelBase panel)
	{
		panel.gameObject.SetActive(false);
		panel.OnHidePanel();
		if (opnedUI.Any(i => i == panel))
		{
			opnedUI.Remove(panel);
		}
	}
}
