using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStateSearch : MonsterStateBase
{
	public float wanderRadius = 10f;
	public float wanderInterval = 2f;

	public override void Start()
    {
        
    }
    // Update is called once per frame
    public override void Update()
    {

    }



	void MoveToRandomPosition()
	{
/*		Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
		randomDirection += transform.position;
		UnityEngine.AI.NavMeshHit hit;
		if (UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, UnityEngine.AI.NavMesh.AllAreas))
		{
			agent.SetDestination(hit.position);
		}*/
	}
}
