using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RAPlayer : Actor
{
	public Transform camera_position;

	/// <summary>
	/// The Sessions ID for the current server.
	/// </summary>
	[SyncVar]
	public string sessionId = "";


	/// <summary>
	/// Player name.
	/// </summary>
	public string username;

	public string ip;

	/// <summary>
	/// Platform the user is on.
	/// </summary>
	public string platform;
	public float power = 5f;

	public Vector3 viewDir = Vector3.forward;

	private void Awake()
	{
		username = SystemInfo.deviceName;
		platform = Application.platform.ToString();
		ip = NetworkManager.singleton.networkAddress;
	}

	private void Start()
	{
	}

	/// <summary>
	/// Called after player has spawned in the scene.
	/// </summary>
	public override void OnStartServer()
	{
		Debug.Log("Player has been spawned on the server!");
	}
}
