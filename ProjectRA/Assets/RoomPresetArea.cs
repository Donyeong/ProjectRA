using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPresetArea : MonoBehaviour
{
	void OnDrawGizmos()
	{
		MapGenerator mapManager = MapGenerator.Instance;
		if(mapManager != null && !mapManager.drawMapGizmos)
		{
			return;
		}
		Gizmos.color = new Color(0, 1, 1, 0.2f); // �þȻ�, 30% ����
		Gizmos.DrawCube(transform.position, transform.rotation * transform.localScale);
	}
}
