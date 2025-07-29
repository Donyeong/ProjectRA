using ReferenceTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropSpawner : MonoBehaviour
{
	public eItemType itemType = eItemType.small;
#if UNITY_EDITOR
	public static bool DrawGizmoProp;
	void OnDrawGizmos()
	{
		if ( !DrawGizmoProp)
		{
			return;
		}
		Gizmos.color = new Color(1, 0, 0, 0.6f);

		Gizmos.DrawCube(transform.position + Vector3.up * (transform.localScale.y / 2), transform.rotation * transform.localScale);
	}
#endif
}
