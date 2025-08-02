using ResourceModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using static RANetworkManager;
public enum ePacketType
{
	GaemStartNotify,
	RoomOptionChangeReq,
	None
}
public class PacketHandler
{
	Dictionary<ePacketType, List<byte>> packetBuffer = new Dictionary<ePacketType, List<byte>>();
	public void HandlePacket(GamePacket gamePacket)
	{
		List<byte> buffer = GetBuffer(gamePacket.packetType);
		if (gamePacket.currentByteIndex == 0)
		{
			if (buffer.Count > 0)
			{
				Debug.LogWarning("Buffer is not empty when starting a new packet. Clearing buffer.");
				buffer.Clear();
			}
		}

		buffer.AddRange(gamePacket.data);
		Debug.Log($"Received packet: {gamePacket.packetType}, Index: {gamePacket.currentByteIndex}, Size: {gamePacket.currentByteSize}, Total: {gamePacket.totalSize}");
		if (buffer.Count >= gamePacket.totalSize)
		{
			// Packet is complete, process it
			ProcessCompletePacket(gamePacket.packetType, buffer);
			buffer.Clear(); // Clear the buffer after processing
		}
	}

	private void ProcessCompletePacket(ePacketType packetType, List<byte> buffer)
	{
		switch (packetType)
		{
			case ePacketType.GaemStartNotify:
				HandleCompletePacket<Packet_GaemStartNotify>(buffer);
				break;
			case ePacketType.RoomOptionChangeReq:
				HandleCompletePacket<Packet_RoomOptionChangeReq>(buffer);
				break;
			default:
				Debug.LogWarning($"Unhandled packet type: {packetType}");
				break;
		}
	}

	private void HandleCompletePacket<T>(List<byte> buffer) where T : PacketBase
	{
		T pacekt = SerializeUtil.DeserializeFromBytes<T>(buffer.ToArray());
		pacekt.OnReceived();
	}



	public List<byte> GetBuffer(ePacketType packetType)
	{
		if (!packetBuffer.ContainsKey(packetType))
		{
			packetBuffer[packetType] = new List<byte>();
		}
		return packetBuffer[packetType];
	}
}
[Serializable]
public class PacketBase
{
	public virtual ePacketType GetPacketType()
	{
		return ePacketType.None;
	}

	public virtual void OnReceived()
	{

	}
}

