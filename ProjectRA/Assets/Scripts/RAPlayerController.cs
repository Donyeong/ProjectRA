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
		public RAProp grabedProp = null;
		public RAProp onAimProp = null;

		public InteractableObject interactableObject = null;

		public Vector3 m_grabPointLocal = new Vector3();

		public float grapPowerRangeMax = 1f;
		public float grapPowerRangeMin = 0f;
		public float grapRange = 1f;
		public float strength = 300;
		public float grapMaxRange = 2f;

		public bool isGrabbed => grabedProp != null;
		public bool isOnAimProp => onAimProp != null;
		public bool isOnAimInteractable => interactableObject != null;

		public LayerMask grabMask;

		public void Awake()
		{
			player = GetComponent<RAPlayer>();
			characterController = GetComponent<Rigidbody>();
		}

		public void CameraRaycast(out RaycastHit hit)
		{
			if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, grapMaxRange, grabMask))
			{
				if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Interactable"))
				{
					OnHitInteractableObject(hit);
				} else
				{
					if (interactableObject != null)
					{
						interactableObject.OnOutAim();
						GameRoomEvent_OnInteractAimOut ev = new GameRoomEvent_OnInteractAimOut();
						CGameManager.Instance.roomEventBus.Publish(ev);
						interactableObject = null;
					}
				}

				if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("PropCollider"))
				{
					OnHitPropCollider(hit);
				} else
				{
					if (onAimProp != null)
					{
						Debug.Log("false");
						onAimProp.Select(false);
						onAimProp = null;
					}
				}
			} else
			{
				if (interactableObject != null)
				{
					interactableObject.OnOutAim();
					GameRoomEvent_OnInteractAimOut ev = new GameRoomEvent_OnInteractAimOut();
					CGameManager.Instance.roomEventBus.Publish(ev);
					interactableObject = null;
				}
				if (onAimProp != null)
				{
					Debug.Log("false");
					onAimProp.Select(false);
					onAimProp = null;
				}
			}
		}

		public void OnHitPropCollider(RaycastHit hit)
		{
			RAPropCollider propCol = hit.collider.GetComponent<RAPropCollider>();
			if (propCol != null)
			{
				RAProp prop = propCol.prop;
				ChangeAimProp(prop);
			}
		}

		public void ChangeAimProp(RAProp newProp)
		{
			if (isOnAimProp)
			{
				if(onAimProp != newProp)
				{
					Debug.Log("false2");
					onAimProp.Select(false);
					onAimProp = newProp;
					onAimProp.Select(true);
				}
			}
			else
			{
				onAimProp = newProp;
				onAimProp.Select(true);
			}
		}

		public void OnHitInteractableObject(RaycastHit hit)
		{
			InteractableObject intObj = hit.collider.GetComponent<InteractableObject>();
			if (intObj != null)
			{
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
			}
		}

		public void PlayerInput()
		{
			CameraRaycast(out RaycastHit hit);
			/*RaycastHit hit;
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
			}*/

			if(isOnAimInteractable && Input.GetKeyDown(KeyCode.E))
			{
				interactableObject.OnInteract();
			}

			if(isOnAimProp && Input.GetKeyDown(KeyCode.Mouse0) && !isGrabbed)
			{
				Vector3 grabPointLocal = onAimProp.transform.InverseTransformPoint(hit.point);
				CmdPickUpProp(onAimProp.GetComponent<NetworkIdentity>(), grabPointLocal);
				GameRoomEvent_OnGrabProp ev = new GameRoomEvent_OnGrabProp();
				ev.targetProp = onAimProp;
				CGameManager.Instance.roomEventBus.Publish(ev);
			}

			if(Input.GetKey(KeyCode.Mouse0) == false && isGrabbed)
			{
				GameRoomEvent_OnDropProp ev = new GameRoomEvent_OnDropProp();
				CGameManager.Instance.roomEventBus.Publish(ev);
				CmdDropProp();
			}
		}

		Vector3 GetPickDest()
		{
			Vector3 aimTarget = Camera.main.transform.position + Camera.main.transform.forward * grapRange;
			return aimTarget;
		}

		public Vector3 GetGrabPointWorldPosition()
		{
			if (grabedProp != null)
			{
				return grabedProp.transform.TransformPoint(m_grabPointLocal);
			}
			return Vector3.zero;
		}

		public void Update()
		{
			if (player.isLocalPlayer)
			{
				PlayerInput();
			}
		}

		public void LateUpdate()
		{
			if (grabedProp != null)
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
			if (grabedProp != null)
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
			if(forceDirection.magnitude > grapPowerRangeMax) {
				forceDirection = forceDirection.normalized * grapPowerRangeMax;
			} else if (forceDirection.magnitude < grapPowerRangeMin) {
				forceDirection = forceDirection.normalized * grapPowerRangeMin;
			}
			float power = strength;

			grabedProp.rb.AddForceAtPosition(forceDirection * power, grabPoint);
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
				grabedProp = propIdentity.GetComponent<RAProp>();
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
			grabedProp = null;
			RpcDropProp();
		}

		[ClientRpc]
		public void RpcDropProp()
		{
			grabedProp = null;
		}
	}
}