using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
	public void Start()
	{
		RANetworkManager.instance.UnityLogin();
		IntroMain();
	}


	public void IntroMain()
	{
		SceneManager.LoadScene("Lobby");
	}


}
