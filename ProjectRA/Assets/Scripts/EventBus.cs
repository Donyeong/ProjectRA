using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EventBus<B>
{
	Dictionary<Type, List<Delegate>> event_actions;
	List<Delegate> pre_proc_actions;
	List<Delegate> post_proc_actions;
	public EventBus()
	{
		event_actions = new Dictionary<Type, List<Delegate>>();
		pre_proc_actions = new List<Delegate>();
		post_proc_actions = new List<Delegate>();
	}

	public void AddListner<T>(Action<T> action) where T : B
	{
		//Debug.Log($" AddListner {action.GetType().Name}");
		if (!event_actions.ContainsKey(typeof(T)))
		{
			event_actions.Add(typeof(T), new List<Delegate>());
			event_actions[typeof(T)].Add(action as Delegate);
		}
		else
		{
			event_actions[typeof(T)].Add(action as Delegate);
		}
	}
	public void AddPostAction(Action<B> action)
	{
		// Debug.Log($" AddPostAction");
		post_proc_actions.Add(action);
	}
	public void AddPreAction<B>(Action<B> action)
	{
		//Debug.Log($" AddPostAction");
		pre_proc_actions.Add(action);
	}

	public void RemovePostAction<B>(Action<B> action)
	{
		//Debug.Log($" AddPostAction");
		post_proc_actions.Remove(action);
	}
	public void RemovePreAction<B>(Action<B> action)
	{
		//Debug.Log($" AddPostAction");
		pre_proc_actions.Remove(action);
	}

	public void RemoveListner<T>(Action<T> action) where T : B
	{
		//Debug.Log($" RemoveListner {action.GetType().Name}");
		event_actions[typeof(T)].Remove(action);
	}
	public void Publish<T>(T ev) where T : B
	{
		foreach (var i in pre_proc_actions)
		{
			(i as Action<B>).Invoke(ev);
		}
		//Debug.Log($" Publish {ev.GetType().Name}");
		if (event_actions.ContainsKey(typeof(T)))
		{
			foreach (var i in event_actions[typeof(T)])
			{
				(i as Action<T>).Invoke(ev);
			}
		}
		foreach (var i in post_proc_actions)
		{
			(i as Action<B>).Invoke(ev);
		}
	}
}