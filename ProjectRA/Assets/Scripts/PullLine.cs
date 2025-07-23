using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullLine : MonoBehaviour
{
	public GameObject pointEffect;
	public Vector3 point1;
	public Vector3 point2;
	public Vector3 point3;

	[Min(1)][SerializeField] int vertexResolution = 12;

	LineRenderer lineRenderer;

	void Start() => lineRenderer = GetComponent<LineRenderer>();

	// Update is called once per frame
	void Update()
	{
		List<Vector3> pointPositions = new List<Vector3>();
		for (float i = 0; i <= 1; i += 1f / vertexResolution)
		{
			Vector3 tangentLineA = Vector3.Lerp(point1, point2, i);
			Vector3 tangentLineB = Vector3.Lerp(point2, point3, i);
			// Each point along the curve
			Vector3 bezierPoint = Vector3.Lerp(tangentLineA, tangentLineB, i);

			// Checks if the current vertex is the first or the last, which makes both ends of the lightning stay still.
			if (i >= 1f / vertexResolution && i <= 1 - 1f / vertexResolution)
			{

				// Changes the point's position randomly based on the noise.
				Vector3 point = new Vector3(bezierPoint.x, bezierPoint.y, bezierPoint.z);
				pointPositions.Add(point);
			}
			else
			{
				pointPositions.Add(bezierPoint);
			}
		}


	lineRenderer.positionCount = pointPositions.Count;
		lineRenderer.SetPositions(pointPositions.ToArray());
		pointEffect.transform.position = point3;
	}
}
