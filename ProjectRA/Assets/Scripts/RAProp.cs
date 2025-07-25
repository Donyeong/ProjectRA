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
	public int hp = 100;
	public int maxHp = 100;
	public int maxPrice;
	public int price;
	public float weight = 1;
	public float minImpactToDamage = 10f;  // �̺��� ���� �浹�� ����
	public float damageMultiplier = 10f;  // ������ ���� ���ط� ���
	public int breakLevel = 5;

	public Rigidbody rb;
	public RAPropCollider propCollider;
	[SyncVar]
	public int prop_id = -1;

	public bool isInit = false;

	public ePropState propState = ePropState.Normal;

	public List<RAPlayer> pickPlayers;

	public bool selected = false; // ī�޶�� ������ �ٶ�(�ƿ����� �뵵)
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
	void OnCollisionEnter(Collision collision)
	{
		float impact = collision.relativeVelocity.magnitude;

		// �浹 ǥ���� ���� ����(ù ��° ������ ����)
		Vector3 normal = collision.contacts[0].normal;
		// ��� �ӵ� ����(�ݴ� ��������)
		Vector3 relVelDir = -collision.relativeVelocity.normalized;

		// ���� ������: 1(����) ~ 0(���� �񽺵�)
		float angleFactor = Mathf.Clamp01(Vector3.Dot(normal, relVelDir));

		// ���� ���� ����
		float adjustedImpact = impact * angleFactor * damageMultiplier;

		if (impact >= minImpactToDamage)
		{
			float damage = (impact - minImpactToDamage);
			hp -= (int)damage;
			int prevBreakLevel = breakLevel;
			breakLevel = hp % 20;

			if(prevBreakLevel != breakLevel)
			{
				ParticleManager.Instance.PlayParticle(eParticleType.PropDamage, transform.position, Quaternion.LookRotation(rb.velocity));
				if (hp <= 0)
				{
					price = 0;
				}
				else
				{
					price = (int)((hp / (float)maxHp) * maxPrice);
				}
				GameRoomEvent_OnDamageProp ev = new GameRoomEvent_OnDamageProp();
				ev.targetProp = this;
				CGameManager.Instance.roomEventBus.Publish(ev);
			}
			

			Debug.Log($"{gameObject.name} ������� {damage} ���� �� ���� ����: {price}, ���� HP : {hp}");

			if (price <= 0f)
			{
				BreakProp();
			}
		}
	}

	void BreakProp()
	{
		ParticleManager.Instance.PlayParticle(eParticleType.PropBreak, transform.position, Quaternion.LookRotation(rb.velocity));
		Destroy(gameObject);
	}
}
