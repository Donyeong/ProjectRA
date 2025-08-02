using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Packet_RoomOptionChangeReq : PacketBase
{
	public RoomOption roomOption;
	public override ePacketType GetPacketType()
	{
		return ePacketType.RoomOptionChangeReq;
	}
	public override void OnReceived()
	{
		Debug.Log(GetPacketType() + " received.");
		CGameManager.Instance.roomOption = roomOption;
	}
}