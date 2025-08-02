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
		public float cartStrength = 3000;
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

			if(isOnAimInteractable && Input.GetKeyDown(KeyCode.E))
			{
				// interactableObject의 NetworkIdentity에서 netId를 얻어서 넘김
				var netIdentity = interactableObject.GetComponent<NetworkIdentity>();
				if (netIdentity != null)
				{
					CmdInteractObject(netIdentity.netId);
				}
				//interactableObject.OnInteract();
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
			Vector3 aimTarget = player.playerMovement.head.transform.position  + (player.viewDir.normalized) * grapRange;
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
			if(grabedProp.propType == ePropType.Cart)
			{
				power = cartStrength;
				grabedProp.rb.AddForceAtPosition(forceDirection * power, grabPoint, ForceMode.Acceleration);
			} else
			{
				grabedProp.rb.AddForceAtPosition(forceDirection * power, grabPoint);
			}

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
		public void CmdInteractObject(uint objectNetId)
		{
			if (NetworkServer.spawned.TryGetValue(objectNetId, out NetworkIdentity objId))
			{
				InteractableObject obj = objId.GetComponent<InteractableObject>();
				if (obj != null)
					obj.OnInteract();
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