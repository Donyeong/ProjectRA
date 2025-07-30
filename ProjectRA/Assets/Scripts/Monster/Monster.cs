using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.GridLayoutGroup;

public class Monster : Actor
{
	public float walkSpeed = 1.0f;
	public float runSpeed = 3.0f;
	public float attackRange = 1.0f;
	public RAPlayer attackTarget = null;
	public float attackCooldown = 0;

	public float attackPower = 50f;
	public float knockbackPower = 50;
	public float knockbackPowerY = 5;
	public float headHeight = 1f;

	public MonsterFSM monsterFSM;

	public NavMeshAgent agent;

	public Animator animator;

	public Dictionary<RAPlayer, float> aggro = new Dictionary<RAPlayer, float>();

	public virtual void InitFSM()
	{

	}


	public RAPlayer GetAggroPlayer()
	{
		if (aggro.Count == 0)
			return null;

		RAPlayer maxAggroPlayer = null;
		float maxAggroValue = float.MinValue;

		foreach (var kvp in aggro)
		{
			if (kvp.Value > maxAggroValue)
			{
				maxAggroValue = kvp.Value;
				maxAggroPlayer = kvp.Key;
			}
		}

		return maxAggroPlayer;
	}

	public void UpdateAggroDecrease(float aggroDecrease)
	{
		// 키 목록을 복사해서 순회
		foreach (var key in new List<RAPlayer>(aggro.Keys))
		{
			aggro[key] -= aggroDecrease;
			if (aggro[key] <= 0)
			{
				aggro.Remove(key);
			}
		}
	}

	public void AddAggro(RAPlayer player, float aggro)
	{
		if (this.aggro.ContainsKey(player))
		{
			this.aggro[player] += aggro;
		}
		else
		{
			this.aggro.Add(player, aggro);
		}
	}

	public virtual void Awake()
	{
		animator = GetComponentInChildren<Animator>();
	}

	public virtual void Start()
	{
		agent = GetComponent<NavMeshAgent>();
	}

	public virtual void Update()
	{
		attackCooldown -= Time.deltaTime;
		if (monsterFSM != null)
		{
			monsterFSM.Update();
		}
	}



	public void ToRotation(Vector3 target, float t)
	{
		Vector3 direction = (target - transform.position).normalized;
		direction.y = 0; // 수평면에서 회전
		if (direction != Vector3.zero)
		{
			Quaternion lookRotation = Quaternion.LookRotation(direction);
			transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, t);
		}
	}

	public bool IsCanView(RAPlayer targetPlayer)
	{
		float eyeRange = 8f;
		float eyeAngle = 60f;
		if (targetPlayer.isCrouched)
		{
			eyeRange = 3f;
			eyeAngle = 25f;
		}
		Vector3 eye = transform.position + Vector3.up * headHeight;
		Vector3 directionToTarget = (targetPlayer.transform.position - eye).normalized;

		//시야각에 들어오는지
		Vector3 directionToTargetHorizontal = (targetPlayer.transform.position - transform.position).normalized;
		directionToTargetHorizontal.y = 0; // 수평면에서만 비교
		float angle = Vector3.Angle(transform.forward, directionToTargetHorizontal);
		Debug.Log(angle);
		if (angle < eyeAngle) // 90도 이내
		{
			RaycastHit hit;
			if (Physics.Raycast(eye, directionToTarget, out hit, eyeRange, CGameManager.Instance.monsterSearchMask))
			{
				Debug.Log(hit.collider.name);
				if (hit.collider.gameObject == targetPlayer.gameObject)
				{
					return true;
				}
			}
		}
		
		return false;
	}
}