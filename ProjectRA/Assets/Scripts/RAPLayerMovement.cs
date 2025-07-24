using ECM2;
using Mirror;
using System;
using UnityEngine;

public class RAPLayerMovement : NetworkBehaviour
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

	public GameObject head;
	public GameObject model;

	Vector3 moveVelocity;
	public float moveSpeed = 1;

/*	// 위치와 회전 동기화
	[SyncVar] Vector3 syncPosition;
	[SyncVar] Vector3 syncRotation;*/

	public TransformSapshot targetTransform;

	public Character character;

	void Awake()
	{
		player = GetComponent<RAPlayer>();
		character = GetComponent<Character>();
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
	}

	void HandleMovement()
	{
		if (isLocalPlayer)
		{
			Vector3 input = Vector3.zero;
			if (Input.GetKey(KeyCode.W)) input += Vector3.forward;
			if (Input.GetKey(KeyCode.S)) input += Vector3.back;
			if (Input.GetKey(KeyCode.A)) input += Vector3.left;
			if (Input.GetKey(KeyCode.D)) input += Vector3.right;

			input = CameraController.Instance.viewRot * input;
			input.y = 0;
			input = input.normalized;
			//moveVelocity += input * Time.deltaTime;

			character.SetMovementDirection(input);


			if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C))
				character.Crouch();
			else if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.C))
				character.UnCrouch();

			if (Input.GetButtonDown("Jump"))
				character.Jump();
			else if (Input.GetButtonUp("Jump"))
				character.StopJumping();
			//if (Input.GetKeyDown(KeyCode.Space)) moveVelocity += Vector3.up * 2f;

			//transform.position = transform.position + (moveVelocity * moveSpeed * Time.deltaTime);
			model.transform.rotation = Quaternion.LookRotation(CameraController.Instance.viewDIrXZ, Vector3.up);
			//moveVelocity = Vector3.Lerp(moveVelocity, Vector3.zero, 0.1f);
			player.viewDir = CameraController.Instance.viewDir;
			head.transform.rotation = Quaternion.LookRotation(CameraController.Instance.viewDir, Vector3.up);

			targetTransform.position = transform.position;
			targetTransform.rotation = model.transform.rotation;
			targetTransform.aimP = CameraController.Instance.viewDir;
			CmdMovePosition(targetTransform.position, targetTransform.rotation, targetTransform.aimP);

			bool isMove = input.magnitude > 0.001f;
			Debug.Log(isMove);
			player.playerAnimController.SetMove(isMove);
			player.playerAnimController.SetRun(true);

		}
	}

	void RemoteSimulateTransform()
	{
		transform.position = targetTransform.position;
		model.transform.rotation = targetTransform.rotation;
		player.viewDir = targetTransform.aimP;

		Vector3 headDir = targetTransform.aimP;
		head.transform.rotation = Quaternion.LookRotation(headDir, Vector3.up);
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