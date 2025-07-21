using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RAProp : NetworkBehaviour
{
	public int price;
	public float weight = 1;

	public Rigidbody rb;
	public RAPropCollider propCollider;
	[SyncVar]
	public int prop_id = -1;

	public bool isInit = false;

	public void Awake()
	{
		rb = GetComponent<Rigidbody>();
		if (rb == null)
		{
			Debug.LogError("RAProp requires a Rigidbody component.");
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
	}
}
