using ECM2;
using Mirror;
using System;
using UnityEngine;

public class RAPlayerMovement : NetworkBehaviour
{
	[Serializable]
	public struct TransformSapshot
	{
		public float remoteTime;

		public Vector3 position;

		public Vector3 velocity;

		public bool moving;

		public Quaternion rotation;

		public Vector3 aimP;

		public void Init(Transform tr)
		{
			remoteTime = 0.0f;
			position = tr.position;
			velocity = Vector3.zero;
			moving = false;
			rotation = tr.rotation;
			aimP = Vector3.forward;
		}

		public void SetFrom(TransformSapshot other)
		{
			remoteTime = other.remoteTime;
			position = other.position;
			velocity = other.velocity;
			moving = other.moving;
			rotation = other.rotation;
			aimP = other.aimP;
		}
	}

	RAPlayer player;

	public GameObject camPos;
	public GameObject head;
	public GameObject model;

	Vector3 moveVelocity;
	public float crouchedSpeed = 0.7f;
	public float moveSpeed = 1.5f;
	public float runMoveSeped = 3f;

/*	// 위치와 회전 동기화
	[SyncVar] Vector3 syncPosition;
	[SyncVar] Vector3 syncRotation;*/

	public TransformSapshot targetTransform;

	public Character character;

	public float targetHeadHeight;


	void Awake()
	{
		player = GetComponent<RAPlayer>();
		character = GetComponent<Character>();
	}

	public void Warp(Vector3 position)
	{
		character.SetPosition(position);
	}

	void Update()
	{
		if (isLocalPlayer)
		{
			HandleMovement();
		} else
		{
			RemoteSimulateTransform();
		}
		if(character.IsCrouched())
		{
			targetHeadHeight = character.crouchedHeight;
		} else
		{
			targetHeadHeight = character.height;
		}

		head.transform.position = Vector3.Lerp(head.transform.position, model.transform.position + Vector3.up * targetHeadHeight, 10 * Time.deltaTime);
		camPos.transform.position = head.transform.position;

	}

	void HandleMovement()
	{
		if (isLocalPlayer)
		{
			Vector3 input = Vector3.zero;
			if (RAInputManager.Instance.GetKey(eInputContentType.KeyboardMoveForward)) input += Vector3.forward;
			if (RAInputManager.Instance.GetKey(eInputContentType.KeyboardMoveBack)) input += Vector3.back;
			if (RAInputManager.Instance.GetKey(eInputContentType.KeyboardMoveLeft)) input += Vector3.left;
			if (RAInputManager.Instance.GetKey(eInputContentType.KeyboardMoveRight)) input += Vector3.right;

			input = CameraController.Instance.viewRot * input;
			input.y = 0;
			input = input.normalized;
			//moveVelocity += input * Time.deltaTime;

			character.SetMovementDirection(input);


			if (RAInputManager.Instance.GetKeyDown(eInputContentType.Crouch))
				character.Crouch();
			else if (RAInputManager.Instance.GetKeyUp(eInputContentType.Crouch))
				character.UnCrouch();

			if (RAInputManager.Instance.GetKeyDown(eInputContentType.Jump))
				character.Jump();
			else if (RAInputManager.Instance.GetKeyUp(eInputContentType.Jump))
				character.StopJumping();
			//if (Input.GetKeyDown(KeyCode.Space)) moveVelocity += Vector3.up * 2f;

			//transform.position = transform.position + (moveVelocity * moveSpeed * Time.deltaTime);
			model.transform.rotation = Quaternion.LookRotation(CameraController.Instance.viewDIrXZ, Vector3.up);
			//moveVelocity = Vector3.Lerp(moveVelocity, Vector3.zero, 0.1f);
			player.viewDir = CameraController.Instance.viewDir;
			head.transform.rotation = Quaternion.Lerp(head.transform.rotation, Quaternion.LookRotation(CameraController.Instance.viewDir, Vector3.up), 20 * Time.deltaTime);
			

			targetTransform.position = transform.position;
			targetTransform.rotation = model.transform.rotation;
			targetTransform.aimP = CameraController.Instance.viewDir;
			CmdMovePosition(targetTransform.position, targetTransform.rotation, targetTransform.aimP);

			bool isMove = input.magnitude > 0.001f;
			player.playerAnimController.SetMove(isMove);
			player.playerAnimController.SetRun(true);

			bool isRun = false;
			if(input != Vector3.zero && RAInputManager.Instance.GetKey(eInputContentType.Sprint))
			{
				if(player.UseStamina(Time.deltaTime * 15))
				{
					isRun = true;
				}
			}
			if (isRun)
			{
				character.maxWalkSpeed = runMoveSeped;
			}
			else
			{
				character.maxWalkSpeed = moveSpeed;
			}
			character.maxWalkSpeedCrouched = crouchedSpeed;
		}
	}

	public void Knockback(Vector3 dir, float power)
	{
		//Debug.Log(dir.normalized * power);

		character.PauseGroundConstraint();
		character.SetMovementMode(Character.MovementMode.Falling);
		character.LaunchCharacter(dir.normalized * power, true, false);	
	}

	void RemoteSimulateTransform()
	{
		transform.position = targetTransform.position;
		model.transform.rotation = targetTransform.rotation;
		player.viewDir = targetTransform.aimP;

		Vector3 headDir = targetTransform.aimP;
		head.transform.rotation = Quaternion.Lerp(head.transform.rotation, Quaternion.LookRotation(headDir, Vector3.up), 20 * Time.deltaTime);
	}
	// 위치/회전 서버에 전송
	[Command]
	void CmdMovePosition(Vector3 _position, Quaternion _rotation, Vector3 aim)
	{
		targetTransform.position = _position;
		targetTransform.rotation = _rotation;
		targetTransform.aimP = aim;
		RpcSyncTransform(_position, _rotation,aim);
	}
	// 위치/회전 서버에 전송
	[ClientRpc]
	void RpcSyncTransform(Vector3 _position, Quaternion _rotation, Vector3 aim)
	{
		targetTransform.position = _position;
		targetTransform.rotation = _rotation;
		targetTransform.aimP = aim;
	}
}