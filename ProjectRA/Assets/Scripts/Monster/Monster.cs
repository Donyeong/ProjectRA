using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class Monster : Actor
{
	public MonsterFSM monsterFSM;
	public float wanderRadius = 10f;
	public float wanderInterval = 2f;

	private NavMeshAgent agent;
	private float timer;

	public Animator animator;

	private void Awake()
	{
		animator = GetComponentInChildren<Animator>();
	}

	void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		timer = wanderInterval;
		MoveToRandomPosition();
	}

	void Update()
	{
		timer += Time.deltaTime;

		if (animator != null)
		{
			animator.SetBool("param_idletowalk", true);
		}

		if (!agent.pathPending && agent.remainingDistance < 0.5f && timer >= wanderInterval)
		{
			MoveToRandomPosition();
			timer = 0f;
		}
	}

	void MoveToRandomPosition()
	{
		Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
		randomDirection += transform.position;
		NavMeshHit hit;
		if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
		{
			agent.SetDestination(hit.position);
		}
	}
}