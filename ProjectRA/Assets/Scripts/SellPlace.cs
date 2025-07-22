using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellPlace : MonoBehaviour
{
	public Collider sellCollider;
    void Start()
    {
    }

    void Update()
    {
        
    }

	public int GetTotalPrice()
	{
		//sellCollider�� �����ִ� prop ���
		Collider[] hitColliders = Physics.OverlapBox(sellCollider.bounds.center, sellCollider.bounds.extents, Quaternion.identity, LayerMask.GetMask("PropCollider"));
		int totalPrice = 0;
		foreach (Collider hitCollider in hitColliders)
		{
			RAPropCollider prop = hitCollider.GetComponent<RAPropCollider>();
			if (prop != null)
			{
				totalPrice += prop.prop.price;
			}
		}

		return totalPrice;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("PropCollider"))
		{
			// Prop ���̾ �ִ� ������Ʈ�� ������ ��
			RAPropCollider prop = other.gameObject.GetComponent<RAPropCollider>();
			if (prop != null)
			{
				DungeonEvent_SellPlaceUpdate sellPlaceUpdate = new DungeonEvent_SellPlaceUpdate();
				sellPlaceUpdate.sellPlace = this;

				CGameManager.Instance.gameEventBus.Publish(sellPlaceUpdate);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("PropCollider"))
		{
			// Prop ���̾ �ִ� ������Ʈ�� ������ ��
			RAPropCollider prop = other.gameObject.GetComponent<RAPropCollider>();
			if (prop != null)
			{
				DungeonEvent_SellPlaceUpdate sellPlaceUpdate = new DungeonEvent_SellPlaceUpdate();
				sellPlaceUpdate.sellPlace = this;

				CGameManager.Instance.gameEventBus.Publish(sellPlaceUpdate);
			}
		}
	}
}
