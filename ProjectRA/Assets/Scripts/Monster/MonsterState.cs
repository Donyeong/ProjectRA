using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStateBase
{
    public Monster owner;
    public MonsterFSM fsm;
	// Start is called before the first frame update
	public virtual void OnEnter()
	{
	}
	public virtual void OnExit()
	{

	}

	// Update is called once per frame
	public virtual void OnUpdate()
    {
        
    }
}
