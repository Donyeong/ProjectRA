using JetBrains.Annotations;
using RA;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public enum eCameraMode
{
	FollowPlayer,
	FreeCamera
}

public class CameraController : SingletonMono<CameraController>
{
    public RAPlayer target;
    public Camera mainCamera;
	public eCameraMode cameraMode = eCameraMode.FreeCamera;

	public Vector3 viewEluar;
	public Quaternion viewRot
	{
		get { return Quaternion.Euler(viewEluar); }
	}

	public Vector3 viewDir
	{
		get { return viewRot * Vector3.forward; }
	}

	public Vector3 viewDIrXZ
	{
		get {
			Vector3 res = viewDir;
			return new Vector3(res.x, 0, res.z).normalized;
		}
	}

	public Vector3 viewDIrY
	{
		get
		{
			Vector3 res = viewDir;
			return new Vector3(0, res.y, 0).normalized;
		}
	}


	void HandleRotation()
	{
		float rotationHorizontal = Input.GetAxis("Mouse X");
		float rotationVertical = -Input.GetAxis("Mouse Y");
		viewEluar.x += rotationVertical;
		viewEluar.y += rotationHorizontal;
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(viewEluar), 0.8f);
	}


	public void SetPlayer(RAPlayer _target)
    {
        target = _target;
		cameraMode = eCameraMode.FollowPlayer;
		Cursor.lockState = CursorLockMode.Locked;
	}
    void Awake()
    {
        
    }

	private void Update()
	{
		if (!UIPanelManager.Instance.isUILock)
		{
			HandleRotation();
		}
		if (Input.GetKeyDown(KeyCode.C))
		{
			if (cameraMode == eCameraMode.FollowPlayer)
			{
				cameraMode = eCameraMode.FreeCamera;
			}
			else
			{
				cameraMode = eCameraMode.FollowPlayer;
			}
		}
		if(UIPanelManager.Instance.isUILock)
		{
			Cursor.lockState = CursorLockMode.None;
		} else
		{
			Cursor.lockState = cameraMode == eCameraMode.FollowPlayer ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}

	// Update is called once per frame
	void LateUpdate()
    {
        UpdateCamera();
    }

    void UpdateCamera()
	{
		if (target == null)
		{
			return;
		}
		gameObject.transform.position = target.camera_position.position;
	}
}
