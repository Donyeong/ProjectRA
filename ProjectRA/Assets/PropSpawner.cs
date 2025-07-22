using ReferenceTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropSpawner : MonoBehaviour
{
	public eItemType itemType = eItemType.small;
	void OnDrawGizmos()
	{
		MapManager mapManager = MapManager.Instance;
		if (mapManager != null && !mapManager.drawPropGizmos)
		{
			return;
		}
		Gizmos.color = new Color(1, 0, 0, 0.6f);

		Gizmos.DrawCube(transform.position, transform.rotation * transform.localScale);
	}
}
