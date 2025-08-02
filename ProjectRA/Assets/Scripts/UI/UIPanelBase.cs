using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelBase : MonoBehaviour
{
	public int uiLayer = 0;
	public bool IsShow()
	{
		return gameObject.activeSelf;
	}
	public virtual void OnShowPanel()
	{
	}
	public virtual void OnHidePanel()
	{
	}
}
