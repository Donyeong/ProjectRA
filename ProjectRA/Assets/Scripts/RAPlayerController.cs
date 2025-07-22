using JetBrains.Annotations;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RA {
	public class RAPlayerController : NetworkBehaviour
	{
		public PullLine pullLine;
		public RAPlayer player;
		public Rigidbody characterController;
		public RAProp targetProp = null;

		public RAProp viewProp = null;

		public float propRange = 1;

		public void Awake()
		{
			characterController = GetComponent<Rigidbody>();
			player = GetComponent<RAPlayer>();
		}

		public void PlayerInput()
		{
			RaycastHit hit;
			bool isSelected = false;
			if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 10f))
			{
				if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("PropCollider"))
				{
					RAPropCollider propCol = hit.collider.GetComponent<RAPropCollider>();
					if (propCol != null)
					{
						isSelected = true;
						RAProp prop = propCol.prop;
						if (viewProp != prop)
						{
							if (viewProp != null)
							{
								viewProp.Select(false);
							}
							viewProp = prop;
							viewProp.Select(true);
						}
						// Prop 레이어에 있는 오브젝트를 클릭했을 때
						if (Input.GetKeyDown(KeyCode.Mouse0))
						{
							CmdPullOn(prop.GetComponent<NetworkIdentity>());
						}
					}
				}
			}
			if(!isSelected)
			{
				if (viewProp != null)
				{
					viewProp.Select(false);
					viewProp = null;
				}
			}
			if (Input.GetKeyUp(KeyCode.Mouse0))
			{
				CmdPullOff();
			}
		}
		public void Update()
		{
			if(player.isLocalPlayer)
			{
				PlayerInput();
			}
			//카메라에서 raycast쏴서 Prop레이어 체크
			

			if (targetProp != null)
			{
				Vector3 headPosition = gameObject.transform.position + Vector3.up * 0.3f;
				Vector3 targetPosition = headPosition + player.viewDir.normalized * propRange;
				Vector3 direction = (targetPosition - targetProp.transform.position);
				float power;
				if (targetProp.weight >= 1)
				{
					power = player.power / targetProp.weight;
				}
				else
				{
					power = player.power / 1;
				}
				// 목표 방향 및 거리 계산
				Vector3 toTarget = targetPosition - targetProp.transform.position;
				float distance = toTarget.magnitude;
				Vector3 dirNorm = toTarget.normalized;

				// 현재 속도를 방향에 투영
				float currentSpeedInDir = Vector3.Dot(targetProp.rb.velocity.normalized, dirNorm);

				// 거리 기반 목표 속도 계산 (예: power * distance)
				float desiredSpeed = power * distance;

				// 추가해야 할 속도 계산
				float speedToAdd = desiredSpeed * 1+currentSpeedInDir;

				// 힘으로 변환해서 적용
				Vector3 force = dirNorm * speedToAdd * targetProp.rb.mass * Time.deltaTime;
				targetProp.rb.AddForce(force, ForceMode.Impulse);
				Debug.Log($"Pulled {targetProp.name} with weight {targetProp.weight}");

				if(pullLine == null)
				{
					GameObject pullLinePrefab =  Resources.Load("PullLine") as GameObject;
					GameObject newPullLine = Instantiate(pullLinePrefab, Vector3.zero, Quaternion.identity);
					pullLine = newPullLine.GetComponent<PullLine>();
				}
				pullLine.point1 = transform.position + Vector3.up * 0.5f;
				pullLine.point2 = Vector3.Lerp(pullLine.point1, targetPosition, 0.5f);
				pullLine.point3 = targetProp.transform.position;
			} else
			{
				if(pullLine != null)
				{
					Destroy(pullLine.gameObject);
				}
			}
		}

		[Command]
		public void CmdPullOn(NetworkIdentity propIdentity)
		{
			if (propIdentity != null)
			{
				targetProp = propIdentity.GetComponent<RAProp>();
				RpcPullOn(propIdentity);
			}
		}
		[ClientRpc]
		public void RpcPullOn(NetworkIdentity propIdentity)
		{
			if (propIdentity != null)
			{
				targetProp = propIdentity.GetComponent<RAProp>();
			}
		}

		[Command]
		public void CmdPullOff()
		{
			targetProp = null;
			RpcPullOff();
		}

		[ClientRpc]
		public void RpcPullOff()
		{
			targetProp = null;
		}
	}
}