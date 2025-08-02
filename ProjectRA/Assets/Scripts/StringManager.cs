using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringManager : MonoBehaviour
{
	public static string GetLocalizeString(string key)
	{
		return $"__{key}";
	}
}
