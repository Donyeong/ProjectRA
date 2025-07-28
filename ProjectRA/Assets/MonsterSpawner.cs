using ReferenceTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
#if UNITY_EDITOR
	public static bool DrawGizmoProp;
	void OnDrawGizmos()
	{
		Gizmos.color = new Color(0, 1, 0, 0.6f);

		Gizmos.DrawCube(transform.position + Vector3.up * (transform.localScale.y/2), transform.rotation * transform.localScale);
	}
#endif
}
