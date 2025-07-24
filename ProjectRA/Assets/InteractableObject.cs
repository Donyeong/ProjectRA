using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
	Outline outline = null;
	public void SetOutline(bool state)
	{
		if(outline == null)
		{
			outline = gameObject.GetComponent<Outline>();
		}
		outline.enabled = state;
	}
	public virtual void OnInteract()
	{
		// This method can be overridden by derived classes to define specific interaction behavior.
		Debug.Log("Interacted with " + gameObject.name);
	}

	public virtual void OnInAim()
	{
		SetOutline(true);
	}

	public virtual void OnOutAim()
	{
		SetOutline(false);
	}

	public virtual string GetInteractText()
	{
		return "상호작용";
	}
	public virtual string GetInteractKey()
	{
		return "E";
	}
}
