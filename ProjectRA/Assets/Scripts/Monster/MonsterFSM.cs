using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFSM
{
    Monster monster;

    public MonsterStateBase currentState = null;

    public Dictionary<string,MonsterStateBase> states = new Dictionary<string , MonsterStateBase>();

    public void Init(Monster _monster,List<MonsterStateBase> states, string defaultState)
	{
		monster = _monster;
		foreach (MonsterStateBase state in states)
		{
			state.owner = monster;
			state.fsm = this;
			RegisterState(state);
		}
		ChangeState(defaultState);
	}

    public void RegisterState(MonsterStateBase state)
	{
		if (state == null || states.ContainsKey(state.GetType().ToString()))
		{
			Debug.LogError("State is null or already registered: " + state);
			return;
		}
		states.Add(state.GetType().ToString(), state);
	}

	public void ChangeState<T>() where T : MonsterStateBase, new()
	{
		if(states.ContainsKey(typeof(T).ToString()) == false)
		{
			T state = Activator.CreateInstance<T>();
			state.owner = monster;
			state.fsm = this;
			RegisterState(state);
		}
		ChangeState(typeof(T).ToString());
	}

	public void ChangeState(string stateKey)
    {
        MonsterStateBase nextState = states[stateKey];
        if(currentState != null)
        {
            currentState.OnExit();
		}
        currentState = nextState;
		nextState.OnEnter();
	}
    

    // Update is called once per frame
    public void Update()
    {
		if (currentState != null)
		{
			currentState.OnUpdate();
		}
    }
}
