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
		RefDataManager.Instance.LoadRefData();
	}


	public void IntroMain()
	{
		SceneManager.LoadScene("Lobby");
		UIPanelManager.Instance.HidePanel<UIPanelLoading>();
	}


}
