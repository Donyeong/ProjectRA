using Mirror;
using ResourceModule;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;
using UnityEngine;
using Utp;

public class RANetworkManager : NetworkManager
{
	public TMP_Text textRoomCode;
	public static RANetworkManager _instance;
	public static RANetworkManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<RANetworkManager>();
			}
			return _instance;
		}
	}
	/// <summary>
	/// The local player object that spawns in.
	/// </summary>
	public RAPlayer localPlayer;
	private string m_SessionId = "";
	private string m_Username;
	private string m_UserId;

	/// <summary>
	/// Flag to determine if the user is logged into the backend.
	/// </summary>
	public bool isLoggedIn = false;

	private UtpTransport utpTransport;

	public string relayJoinCode { get; private set; }

	public PacketHandler packetHandler_s2c = new PacketHandler(); // server => client

	public Dictionary<NetworkConnectionToClient, PacketHandler> packetHandler_c2s = new Dictionary<NetworkConnectionToClient, PacketHandler>(); // Client => Server

	public override void Awake()
	{
		base.Awake();

		m_Username = SystemInfo.deviceName;
		utpTransport = transport as UtpTransport;
	}

	public void StartHostGame(int maxPlayers, string regionId = null)
	{
		utpTransport.useRelay = true;
		utpTransport.AllocateRelayServer(maxPlayers, regionId,
		(string joinCode) =>
		{
			relayJoinCode = joinCode;
			textRoomCode.SetText($"RoomCode {relayJoinCode}");

			StartHost();
			LobbyManager.Instance.CreateRoom(joinCode);
		},
		() =>
		{
			UtpLog.Error($"Failed to start a Relay host.");
		});
	}


	public void JoinRelayServer(string _joinCode)
	{
		textRoomCode.SetText($"RoomCode {relayJoinCode}");
		relayJoinCode = _joinCode;
		utpTransport.useRelay = true;
		utpTransport.ConfigureClientWithJoinCode(_joinCode,
		() =>
		{
			StartClient();
		},
		() =>
		{
			UtpLog.Error($"Failed to join Relay server.");
		});
	}

	public async Task UnityLogin()
	{
		try
		{
			await UnityServices.InitializeAsync();
			await AuthenticationService.Instance.SignInAnonymouslyAsync();
			Debug.Log("Logged into Unity, player ID: " + AuthenticationService.Instance.PlayerId);
			isLoggedIn = true;
		}
		catch (Exception e)
		{
			isLoggedIn = false;
			Debug.Log(e);
		}
	}

	private void Update()
	{
		if (NetworkManager.singleton.isNetworkActive)
		{
			if (localPlayer == null)
			{
				FindLocalPlayer();
			}
		}
		else
		{
			localPlayer = null;
			if (CGameManager.isInitInstance)
			{
				CGameManager.Instance.gameUsers.Clear();
			}
		}
	}

	/// <summary>
	/// 서버가 실행되었을때 서버에서 실행
	/// </summary>
	public override void OnStartServer()
	{
		base.OnStopServer();
		Debug.Log("MyNetworkManager: Server Started!");

		m_SessionId = System.Guid.NewGuid().ToString();

		CGameManager.Instance.roomEventBus.Publish(new GameRoomEvent_RoomCreated());

		NetworkServer.RegisterHandler<GamePacket>(OnReceivedC2S);

	}

	/// <summary>
	/// 신규 Player가 접속하면 Server에서 실행
	/// </summary>
	/// <param name="conn"></param>
	public override void OnServerAddPlayer(NetworkConnectionToClient conn)
	{
		base.OnServerAddPlayer(conn);

		foreach (KeyValuePair<uint, NetworkIdentity> kvp in NetworkServer.spawned)
		{
			RAPlayer comp = kvp.Value.GetComponent<RAPlayer>();

			bool alreadyExists = CGameManager.Instance.gameUsers.Any(user => user.raplayer == comp);
			// Add to player list if new
			if (comp != null && !alreadyExists)
			{
				comp.sessionId = System.Guid.NewGuid().ToString();

				CGameUser gameUser = new CGameUser
				{
					userId = m_SessionId,
					userName = m_SessionId,
					raplayer = comp
				};
				CGameManager.Instance.gameUsers.Add(gameUser);
			}
		}
	}

	public override void OnStopServer()
	{
		base.OnStopServer();
		Debug.Log("MyNetworkManager: Server Stopped!");
		m_SessionId = "";
	}

	public override void OnServerDisconnect(NetworkConnectionToClient conn)
	{
		base.OnServerDisconnect(conn);

		Dictionary<uint, NetworkIdentity> spawnedPlayers = NetworkServer.spawned;

		// Update players list on client disconnect
		foreach (CGameUser user in CGameManager.Instance.gameUsers)
		{
			bool playerFound = false;

			foreach (KeyValuePair<uint, NetworkIdentity> kvp in spawnedPlayers)
			{
				RAPlayer comp = kvp.Value.GetComponent<RAPlayer>();

				// Verify the player is still in the match
				if (comp != null && user.raplayer == comp)
				{
					playerFound = true;
					break;
				}
			}

			if (!playerFound)
			{
				CGameManager.Instance.gameUsers.Remove(user);
				break;
			}
		}
	}

	public override void OnStopClient()
	{
		base.OnStopClient();

		Debug.Log("MyNetworkManager: Left the Server!");

		localPlayer = null;

		m_SessionId = "";
	}

	public override void OnClientConnect()
	{
		base.OnClientConnect();
		Debug.Log($"MyNetworkManager: {m_Username} Connected to Server!");
		var panel = UIPanelManager.Instance.GetPanel<UIPanelLoading>();
		UIPanelManager.Instance.HidePanel(panel);


		VoiceChatManager.Instance.Join3DChannel(() =>
		{

		});
	}

	public override void OnClientDisconnect()
	{
		base.OnClientDisconnect();
		Debug.Log("MyNetworkManager: Disconnected from Server!");
	}

	/// <summary>
	/// Finds the local player if they are spawned in the scene.
	/// </summary>
	void FindLocalPlayer()
	{
		//Check to see if the player is loaded in yet
		if (NetworkClient.localPlayer == null)
			return;

		localPlayer = NetworkClient.localPlayer.GetComponent<RAPlayer>();
		CameraController.Instance.SetPlayer(RANetworkManager.instance.localPlayer);
		// 사용 예시
		SetLayerRecursively(localPlayer.gameObject, LayerMask.NameToLayer("LocalPlayer"));

		GameRoomEvent_GenerateLocalPlayer gameRoomEvent_GenerateLocalPlayer = new GameRoomEvent_GenerateLocalPlayer();
		CGameManager.Instance.roomEventBus.Publish(gameRoomEvent_GenerateLocalPlayer);

	}
	void SetLayerRecursively(GameObject obj, int newLayer)
	{
		obj.layer = newLayer;
		foreach (Transform child in obj.transform)
		{
			if (child.gameObject.layer == LayerMask.NameToLayer("PlayerHead"))
			{
				continue;
			}
			SetLayerRecursively(child.gameObject, newLayer);
		}
	}



	public override void OnStartClient()
	{
		base.OnStartClient();

		NetworkClient.RegisterHandler<ResponseSetupInfo>(OnResponseSetupInfo);
		NetworkClient.RegisterHandler<GamePacket>(OnReceivedPacketS2C);

	}
	private void OnResponseSetupInfo(ResponseSetupInfo message)
	{
	}
	private void OnReceivedC2S(NetworkConnectionToClient networkConnectionToClient, GamePacket gamePacket)
	{
		if (gamePacket.data == null || gamePacket.data.Length == 0)
		{
			Debug.LogWarning("Received empty packet data.");
			return;
		}
		if(!packetHandler_c2s.ContainsKey(networkConnectionToClient))
		{
			packetHandler_c2s.Add(networkConnectionToClient, new PacketHandler());
		}
		packetHandler_c2s[networkConnectionToClient].HandlePacket(gamePacket);
	}

	public void BroadcastMessage<T>(T message) where T : PacketBase
	{
		foreach (NetworkConnectionToClient connection in NetworkServer.connections.Values)
		{
			//connection.Send<T>(message);
			SendPacketToClient<T>(connection, message);
		}
	}

	public void SendPacketToServer<T>(T packet) where T : PacketBase
	{
		Debug.Log("SendPacketToServer");

		byte[] data = SerializeUtil.SerializeToBytes<T>(packet);

		//512 바이트씩 쪼개서 보낸다
		int totalSize = data.Length;
		int currentByteIndex = 0;
		while (currentByteIndex < totalSize)
		{
			Debug.Log("SendPacketToClient" + currentByteIndex);
			int currentByteSize = Mathf.Min(512, totalSize - currentByteIndex);
			byte[] packetData = new byte[currentByteSize];
			Array.Copy(data, currentByteIndex, packetData, 0, currentByteSize);

			GamePacket gamePacket = new GamePacket
			{
				packetType = packet.GetPacketType(),
				totalSize = totalSize,
				currentByteIndex = currentByteIndex,
				currentByteSize = currentByteSize,
				data = packetData
			};
			Debug.Log(gamePacket.packetType + " " + gamePacket.currentByteIndex + " " + gamePacket.currentByteSize + " " + gamePacket.totalSize);

			if (gamePacket.data == null)
			{
				Debug.LogError("GamePacket data is null.");
			}
			NetworkClient.Send(gamePacket);
			currentByteIndex += currentByteSize;
		}
	}

	public void SendPacketToClient<T>(NetworkConnectionToClient connection, T packet) where T : PacketBase
	{
		if (connection == null || !connection.isReady)
		{
			Debug.LogWarning("Connection is null or not ready.");
			return;
		}
		Debug.Log("SendPacketToClient");

		byte[] data = SerializeUtil.SerializeToBytes<T>(packet);

		//512 바이트씩 쪼개서 보낸다
		int totalSize = data.Length;
		int currentByteIndex = 0;
		while (currentByteIndex < totalSize)
		{
			Debug.Log("SendPacketToClient" + currentByteIndex);
			int currentByteSize = Mathf.Min(512, totalSize - currentByteIndex);
			byte[] packetData = new byte[currentByteSize];
			Array.Copy(data, currentByteIndex, packetData, 0, currentByteSize);

			GamePacket gamePacket = new GamePacket
			{
				packetType = packet.GetPacketType(),
				totalSize = totalSize,
				currentByteIndex = currentByteIndex,
				currentByteSize = currentByteSize,
				data = packetData
			};
			Debug.Log(gamePacket.packetType + " " + gamePacket.currentByteIndex + " " + gamePacket.currentByteSize + " " + gamePacket.totalSize);

			if(gamePacket.data == null)
			{
				Debug.LogError("GamePacket data is null.");
			}

			connection.Send(gamePacket);
			currentByteIndex += currentByteSize;
		}
	}

	public void OnReceivedPacketS2C(GamePacket gamePacket)
	{
		if (gamePacket.data == null || gamePacket.data.Length == 0)
		{
			Debug.LogWarning("Received empty packet data.");
			return;
		}
		if(packetHandler_s2c == null)
		{
			Debug.LogError("PacketHandler is null.");
			return;
		}
		packetHandler_s2c.HandlePacket(gamePacket);
	}
}

public struct ResponseSetupInfo : NetworkMessage
{

}


public struct GamePacket : NetworkMessage
{
	public ePacketType packetType;
	public int totalSize;
	public int currentByteIndex;
	public int currentByteSize;
	public byte[] data;
}