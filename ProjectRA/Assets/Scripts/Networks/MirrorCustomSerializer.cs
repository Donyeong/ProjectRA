using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MirrorCustomSerializer
{
	public static void WriteByteArray(this NetworkWriter writer, byte[] value)
	{
		if (value == null)
		{
			writer.WriteInt(-1);
			return;
		}

		writer.WriteInt(value.Length);
		writer.WriteBytes(value, 0, value.Length);
	}

	public static byte[] ReadByteArray(this NetworkReader reader)
	{
		int length = reader.ReadInt();
		if (length < 0)
			return null;

		return reader.ReadBytes(length);
	}
}
