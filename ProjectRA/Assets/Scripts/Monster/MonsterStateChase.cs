using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class MonsterStateChase : MonsterStateBase
{

	public override void OnEnter()
	{
	}
	// Update is called once per frame
	public override void OnUpdate()
	{
		RAPlayer targetPlayer = owner.GetAggroPlayer();

		if (targetPlayer == null)
		{
			fsm.ChangeState<MonsterStateSearch>();
			return;
		}

		owner.UpdateAggroDecrease(10 * Time.deltaTime);

		if (owner.animator != null)
		{
		}
		if (owner.agent.pathPending || owner.agent.remainingDistance < 0.3f)
		{
			//애니메이션 상태 변경
			if (owner.animator != null)
			{
				owner.animator.SetBool("param_idletorunning", false);
			}
		} else 
		{
			if (owner.animator != null)
			{
				owner.animator.SetBool("param_idletorunning", true);
			}
		}

		if (Vector3.Distance(owner.transform.position, targetPlayer.transform.position) > owner.attackRange)
		{
			owner.ToRotation(owner.agent.destination, 15 * Time.deltaTime);
			UnityEngine.AI.NavMeshHit hit;
			if (UnityEngine.AI.NavMesh.SamplePosition(targetPlayer.transform.position, out hit, 100, UnityEngine.AI.NavMesh.AllAreas))
			{
				owner.agent.speed = owner.runSpeed;
				owner.agent.SetDestination(hit.position);
			}
		}
		else
		{
			owner.ToRotation(targetPlayer.transform.position, 5 * Time.deltaTime);
			owner.agent.SetDestination(owner.transform.position);
			//거리가 가까울경우
			if (owner.attackCooldown <= 0)
			{
				owner.attackTarget = targetPlayer;
				fsm.ChangeState<MonsterStateGirlAttack>();
				return;
			}
		}
	}

	public override void OnExit()
	{
		base.OnExit();

		if (owner.animator != null)
		{
			owner.animator.SetBool("param_idletorunning", false);
		}
	}

	void MoveToRandomPosition()
	{
	}
}
