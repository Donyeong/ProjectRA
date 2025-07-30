using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStateGirlAttack : MonsterStateBase
{
	Vector3 targetPosition;
	public float attackTime = 0.6f; // ���� �ִϸ��̼� ��� �ð�
	public bool isHIt = false;
	public float hitTime = 0.1f; // ���� ��Ʈ ���� �ð�
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
		// ���� �ִϸ��̼��� �����ٸ�
		// ���� ��Ʈ ����
		if (timer > hitTime)
		{
			if (!isHIt)
			{
				isHIt = true;
				//���� 90�� �̳� �ִ� �÷��̾� ��� ����
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
						if (angle < 45.0f) // 90�� �̳�
						{
							AttackInfo attackInfo = new AttackInfo();
							attackInfo.damage = owner.attackPower;
							attackInfo.attacker = owner;
							attackInfo.attackType = eAttackType.MonsterAttack;
							attackInfo.direction = directionToTarget + Vector3.up * owner.knockbackPowerY;
							attackInfo.knockbackPower = owner.knockbackPower; // Knockback power ����
							targetPlayer.TakeDamage(attackInfo); // ������ ó��
						}
					}
				}
			}
		}

		if (timer > attackTime)
		{
			// ��Ʈ ���� �ð� ���Ŀ��� �ٽ� ���¸� ����
			fsm.ChangeState<MonsterStateChase>();
		}
	}

	public override void OnExit()
	{
		base.OnExit();
		owner.attackCooldown = 2f;
	}
}
