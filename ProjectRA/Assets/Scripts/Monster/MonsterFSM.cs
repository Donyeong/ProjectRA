using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFSM
{
    Monster monster;

    public MonsterStateBase currentState = null;

    public void Init(Monster _monster,MonsterStateBase _defaultState)
    {
        monster = _monster;
        currentState = _defaultState;
        currentState.owner = monster;
        currentState.fsm = this;
	}

    // Update is called once per frame
    public void Update()
    {
        
    }
}
