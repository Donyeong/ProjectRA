using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public class SerializeUtil
{
	// ��ü �� byte[]
	public static byte[] SerializeToBytes<T>(T obj)
	{
		// JsonUtility�� JSON ���ڿ� ����
		string json = JsonUtility.ToJson(obj);
		// UTF8 ���ڵ����� byte[] ��ȯ
		return Encoding.UTF8.GetBytes(json);
	}

	// byte[] �� ��ü
	public static T DeserializeFromBytes<T>(byte[] bytes)
	{
		// byte[] �� JSON ���ڿ� ��ȯ
		string json = Encoding.UTF8.GetString(bytes);
		// JSON �� ��ü ��ȯ
		return JsonUtility.FromJson<T>(json);
	}
}
