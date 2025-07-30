using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterPrettyGirl : Monster
{
	public override void InitFSM()
	{
		monsterFSM = new MonsterFSM();

		List<MonsterStateBase> states = new List<MonsterStateBase>();
		states.Add(new MonsterStateSearch());
		monsterFSM.Init(this, states, typeof(MonsterStateSearch).ToString());
	}

	public override void Awake()
	{
		base.Awake();
	}

	public override void Start()
	{
		base.Start();
		InitFSM();
	}

	public override void Update()
	{
		base.Update();
	}
}