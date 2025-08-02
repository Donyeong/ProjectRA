using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Packet_GaemStartNotify : PacketBase
{
	public List<RoomInfo> roomInfo;
	public int goalPrice;
	public override ePacketType GetPacketType()
	{
		return ePacketType.GaemStartNotify;
	}
	public override void OnReceived()
	{
		Debug.Log(GetPacketType() + " received.");
		MapManager.Instance.GameMapStart(roomInfo);

		CGameManager.Instance.goalPrice = goalPrice;
		GameRoomEvent_OnStartGame gameRoomEvent_OnStartGame = new GameRoomEvent_OnStartGame();
		CGameManager.Instance.roomEventBus.Publish(gameRoomEvent_OnStartGame);
	}
	public Packet_GaemStartNotify(List<RoomInfo> roomInfo)
	{
		this.roomInfo = roomInfo;
	}
}