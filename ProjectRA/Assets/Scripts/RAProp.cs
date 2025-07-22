using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ePropState
{
	Normal,
	PickedUp,
}

public class RAProp : NetworkBehaviour
{
	public int hp;
	public int maxHp;
	public int price;
	public float weight = 1;

	public Rigidbody rb;
	public RAPropCollider propCollider;
	[SyncVar]
	public int prop_id = -1;

	public bool isInit = false;

	public ePropState propState = ePropState.Normal;

	public List<RAPlayer> pickPlayers;

	public bool selected = false; // 카메라로 아이템 바라봄(아웃라인 용도)
	public Outline[] outlines = null;

	public void Awake()
	{
		rb = GetComponent<Rigidbody>();
		if (rb == null)
		{
			Debug.LogError("RAProp requires a Rigidbody component.");
		}
	}

	public void PickUp()
	{
	}
	public void PickDown()
	{
	}

	public void Select(bool itemSelected)
	{
		if(outlines == null)
		{
			outlines = GetComponentsInChildren<Outline>();
		} else
		{
			if(outlines.Length == 0)
			{
				outlines = GetComponentsInChildren<Outline>();
			}
		}
		selected = itemSelected;
		if (outlines != null)
		{
			foreach (Outline outline in outlines)
			{
				outline.enabled = itemSelected;
			}
		}
	}

	public void Update()
	{
		if(!isInit && prop_id != -1)
		{
			SetProp();
			isInit = true;
		}
	}


	[Server]
	public void ServerSetProp(int _prop_id)
	{
		prop_id = _prop_id;
	}

	public void SetProp()
	{
		GameObject go = ResourceManager.Instance.LoadResource< GameObject>($"Props/Prop_{prop_id}");
		GameObject model = Instantiate(go, transform);
		propCollider = model.GetComponent<RAPropCollider>();
		propCollider.prop = this;
		Select(false);
	}
}
