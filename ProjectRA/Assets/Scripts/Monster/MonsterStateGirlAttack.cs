using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStateGirlAttack : MonsterStateBase
{
	Vector3 targetPosition;
	public float attackTime = 0.6f; // 공격 애니메이션 재생 시간
	public bool isHIt = false;
	public float hitTime = 0.1f; // 공격 히트 판정 시간
	public float timer = 0;
	public override void OnEnter()
	{
		targetPosition = owner.attackTarget.transform.position;
		if (owner.animator != null)
		{
			owner.animator.SetTrigger("param_attack");
		}
		isHIt = false;
		timer = 0;
	}
	// Update is called once per frame
	public override void OnUpdate()
	{
		owner.ToRotation(targetPosition, 30 * Time.deltaTime);
		timer += Time.deltaTime;
		// 공격 애니메이션이 끝났다면
		// 공격 히트 판정
		if (timer > hitTime)
		{
			if (!isHIt)
			{
				isHIt = true;
				//정면 90도 이내 있는 플레이어 모두 감지
				Collider[] hitColliders = Physics.OverlapSphere(owner.transform.position, owner.attackRange);
				foreach (Collider collider in hitColliders)
				{
					RAPlayer targetPlayer = collider.GetComponent<RAPlayer>();
					if (targetPlayer != null)
					{
						Vector3 directionToTarget = (targetPlayer.transform.position - owner.transform.position);
						directionToTarget.y = 0;
						directionToTarget.Normalize();
						float angle = Vector3.Angle(owner.transform.forward, directionToTarget);
						if (angle < 45.0f) // 90도 이내
						{
							AttackInfo attackInfo = new AttackInfo();
							attackInfo.damage = owner.attackPower;
							attackInfo.attacker = owner;
							attackInfo.attackType = eAttackType.MonsterAttack;
							attackInfo.direction = directionToTarget + Vector3.up * owner.knockbackPowerY;
							attackInfo.knockbackPower = owner.knockbackPower; // Knockback power 설정
							targetPlayer.TakeDamage(attackInfo); // 데미지 처리
						}
					}
				}
			}
		}

		if (timer > attackTime)
		{
			// 히트 판정 시간 이후에는 다시 상태를 변경
			fsm.ChangeState<MonsterStateChase>();
		}
	}

	public override void OnExit()
	{
		base.OnExit();
		owner.attackCooldown = 2f;
	}
}
