using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAnimController : NetworkBehaviour
{
    public Animator animator;
	private void Awake()
	{
		animator = GetComponentInChildren<Animator>();
	}

	public void SetMove(bool isMove)
	{
		CmdSetBool("bMove", isMove);
	}

	public void SetRun(bool isRun)
	{
		CmdSetBool("bRun", isRun);
	}
	public void SetDead()
	{
		CmdSetBool("bDead", true);
		CmdSetTrigger("tDead");
	}

	[Command]
	public void CmdSetBool(string key, bool state)
	{
		animator.SetBool(key, state);
		RpcSetBool(key, state);
	}

	[ClientRpc]
	public void RpcSetBool(string key, bool state)
	{
		animator.SetBool(key, state);
	}

	[Command]
	public void CmdSetTrigger(string key)
	{
		animator.SetTrigger(key);
		RpcSetTrigger(key);
	}

	[ClientRpc]
	public void RpcSetTrigger(string key)
	{
		animator.SetTrigger(key);
	}
}
