using JetBrains.Annotations;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
namespace RA {
	public class RAPlayerController : NetworkBehaviour
	{
		public PullLine pullLine;
		public RAPlayer player;
		public Rigidbody characterController;
		public RAProp pickedProp = null;
		public RAProp viewTargetProp = null;

		public InteractableObject interactableObject = null;

		public Vector3 m_grabPointLocal = new Vector3(); 

		public float grapRange = 1;
		public float strength = 300;

		public void Awake()
		{
			player = GetComponent<RAPlayer>();
			characterController = GetComponent<Rigidbody>();
		}

		public void PlayerInput()
		{
			RaycastHit hit;
			bool isSelected = false;
			bool intObjSelected = false;
			if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 10f))
			{
				if(hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Interactable")) {
					InteractableObject intObj = hit.collider.GetComponent<InteractableObject>();
					intObjSelected = true;
					if (interactableObject != intObj)
					{
						if (interactableObject != null)
						{
							interactableObject.OnOutAim();
							GameRoomEvent_OnInteractAimOut ev = new GameRoomEvent_OnInteractAimOut();
							CGameManager.Instance.roomEventBus.Publish(ev);
						}
						interactableObject = intObj;
						interactableObject.OnInAim();
						GameRoomEvent_OnInteractAimIn e = new GameRoomEvent_OnInteractAimIn() { interactableObject = interactableObject };
						CGameManager.Instance.roomEventBus.Publish(e);
					}

					if (Input.GetKeyDown(KeyCode.E))
					{
						interactableObject.OnInteract();
					}
				}
				if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("PropCollider"))
				{
					RAPropCollider propCol = hit.collider.GetComponent<RAPropCollider>();
					if (propCol != null)
					{
						isSelected = true;
						RAProp prop = propCol.prop;
						if (viewTargetProp != prop)
						{
							if (viewTargetProp != null)
							{
								viewTargetProp.Select(false);
							}
							viewTargetProp = prop;
							viewTargetProp.Select(true);
						}
						// Prop ���̾ �ִ� ������Ʈ�� Ŭ������ ��
						if (Input.GetKeyDown(KeyCode.Mouse0))
						{
							Vector3 grabPointLocal = propCol.prop.transform.InverseTransformPoint(hit.point);
							CmdPickUpProp(prop.GetComponent<NetworkIdentity>(), grabPointLocal);

							GameRoomEvent_OnGrabProp ev = new GameRoomEvent_OnGrabProp();
							ev.targetProp = prop;
							CGameManager.Instance.roomEventBus.Publish(ev);
						}
					}
				}
			}
			if(!isSelected)
			{
				if (viewTargetProp != null)
				{
					viewTargetProp.Select(false);
					viewTargetProp = null;
				}
			}
			if (!intObjSelected)
			{
				if (interactableObject != null)
				{
					interactableObject.OnOutAim();
					GameRoomEvent_OnInteractAimOut ev = new GameRoomEvent_OnInteractAimOut();
					CGameManager.Instance.roomEventBus.Publish(ev);
					interactableObject = null;
				}
			}
			if (Input.GetKeyUp(KeyCode.Mouse0))
			{
				GameRoomEvent_OnDropProp ev = new GameRoomEvent_OnDropProp();
				CGameManager.Instance.roomEventBus.Publish(ev);
				CmdDropProp();
			}
		}

		public void UpdatePickedProp()
		{
		}

		Vector3 GetPickDest()
		{
			Vector3 aimTarget = Camera.main.transform.position + Camera.main.transform.forward * grapRange;
			return aimTarget;
		}

		public Vector3 GetGrabPointWorldPosition()
		{
			if (pickedProp != null)
			{
				return pickedProp.transform.TransformPoint(m_grabPointLocal);
			}
			return Vector3.zero;
		}

		public void Update()
		{
			if (pickedProp != null)
			{
				UpdatePickupLineEffect();
			}
			else
			{
				if (pullLine != null)
				{
					Destroy(pullLine.gameObject);
				}
			}
		}

		public void FixedUpdate()
		{
			if(player.isLocalPlayer)
			{
				PlayerInput();
			}
			if (pickedProp != null)
			{
				UpdateGrabedProp();
			}
		}

		public void UpdateGrabedProp()
		{
			Vector3 grabPoint = GetGrabPointWorldPosition();

			Vector3 pickDest = GetPickDest();
			Vector3 direction = (pickDest - grabPoint);

			Gizmos.color = Color.red;
			Debug.DrawLine(grabPoint, pickDest);

			Vector3 forceDirection = direction;
			float power = strength;

			pickedProp.rb.AddForceAtPosition(forceDirection * power, grabPoint);
		}

		public void UpdatePickupLineEffect()
		{
			Vector3 pickDest = GetPickDest();
			Vector3 grabPoint = GetGrabPointWorldPosition();
			if (pullLine == null)
			{
				GameObject pullLinePrefab = Resources.Load("PullLine") as GameObject;
				GameObject newPullLine = Instantiate(pullLinePrefab, Vector3.zero, Quaternion.identity);
				pullLine = newPullLine.GetComponent<PullLine>();
			}
			pullLine.point1 = Camera.main.transform.position + Vector3.down*0.3f + (CameraController.Instance.viewRot * Vector3.right * 0.2f);
			pullLine.point2 = pickDest;//Vector3.Lerp(pullLine.point1, pickDest, 0.5f);
			pullLine.point3 = grabPoint;
		}

		public void PickUpProp(NetworkIdentity propIdentity, Vector3 _grabPointLocal)
		{
			if (propIdentity != null)
			{
				pickedProp = propIdentity.GetComponent<RAProp>();
				m_grabPointLocal = _grabPointLocal;
			}
		}

		[Command]
		public void CmdPickUpProp(NetworkIdentity propIdentity, Vector3 _grabPointLocal)
		{
			PickUpProp(propIdentity, _grabPointLocal);
			RpcPickUpProp(propIdentity, _grabPointLocal);
		}
		[ClientRpc]
		public void RpcPickUpProp(NetworkIdentity propIdentity, Vector3 _grabPointLocal)
		{
			PickUpProp(propIdentity, _grabPointLocal);
		}

		[Command]
		public void CmdDropProp()
		{
			pickedProp = null;
			RpcDropProp();
		}

		[ClientRpc]
		public void RpcDropProp()
		{
			pickedProp = null;
		}
	}
}