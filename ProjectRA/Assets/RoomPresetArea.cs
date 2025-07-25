using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPresetArea : MonoBehaviour
{
#if UNITY_EDITOR
	public static bool DrawGizmoArea;
	void OnDrawGizmos()
	{
		if (!DrawGizmoArea)
		{
			return;
		}
		Gizmos.color = new Color(0, 1, 1, 0.2f); // �þȻ�, 30% ����
		Gizmos.DrawCube(transform.position, transform.rotation * transform.localScale);
	}

#endif
}
