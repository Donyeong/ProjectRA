using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public class SerializeUtil
{
	// 객체 → byte[]
	public static byte[] SerializeToBytes<T>(T obj)
	{
		// JsonUtility로 JSON 문자열 생성
		string json = JsonUtility.ToJson(obj);
		// UTF8 인코딩으로 byte[] 변환
		return Encoding.UTF8.GetBytes(json);
	}

	// byte[] → 객체
	public static T DeserializeFromBytes<T>(byte[] bytes)
	{
		// byte[] → JSON 문자열 변환
		string json = Encoding.UTF8.GetString(bytes);
		// JSON → 객체 변환
		return JsonUtility.FromJson<T>(json);
	}
}
