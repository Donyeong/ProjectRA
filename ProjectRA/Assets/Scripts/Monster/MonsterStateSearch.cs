using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStateSearch : MonsterStateBase
{
	private float timer;
	public float wanderRadius = 10f;
	public float wanderInterval = 2f;
	private float searchTimer = 0f;

	public float searchInterval = 1f;

	public override void OnEnter()
	{
		timer = wanderInterval;
		searchTimer = searchInterval;
		MoveToRandomPosition();
	}
    // Update is called once per frame
    public override void OnUpdate()
	{
		timer += Time.deltaTime;

		if (owner.animator != null)
		{
			owner.animator.SetBool("param_idletowalk", true);
		}

		if (!owner.agent.pathPending && owner.agent.remainingDistance < 0.5f && timer >= wanderInterval)
		{
			MoveToRandomPosition();
			timer = 0f;
		}

		searchTimer+= Time.deltaTime;

		if (searchTimer >= searchInterval)
		{
			//주변 플레이어를 탐색
			Collider[] hitColliders = Physics.OverlapSphere(owner.transform.position, wanderRadius);
			foreach (Collider collider in hitColliders)
			{
				RAPlayer targetPlayer = collider.GetComponent<RAPlayer>();
				if (targetPlayer != null)
				{
					owner.AddAggro(targetPlayer, 100);
					// 플레이어가 발견되면 추적 상태로 전환
					fsm.ChangeState<MonsterStateChase>();
					return;
				}
			}
			searchTimer = 0f;
		}
	}

	public override void OnExit()
	{
		base.OnExit();

		if (owner.animator != null)
		{
			owner.animator.SetBool("param_idletowalk", false);
		}
	}

	void MoveToRandomPosition()
	{
		Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
		randomDirection += owner.transform.position;
		UnityEngine.AI.NavMeshHit hit;
		if (UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, UnityEngine.AI.NavMesh.AllAreas))
		{
			owner.agent.speed = owner.walkSpeed;
			owner.agent.SetDestination(hit.position);
		}
	}
}
