using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
	public void Start()
	{
		Init();
	}

	private async  void Init()
	{
		await RANetworkManager.instance.UnityLogin();
		IntroMain();
		RefDataManager.Instance.LoadRefData();
		await VivoxService.Instance.InitializeAsync();
		await LoginAsync();
		VoiceChatManager.Instance.Initi();
	}


	public void IntroMain()
	{
		SceneManager.LoadScene("Lobby");
		UIPanelManager.Instance.HidePanel<UIPanelLoading>();
	}

	private async Task LoginAsync()
	{

		//로그인 옵션 생성
		LoginOptions options = new LoginOptions();

		//디스플레이 이름 설정
		options.DisplayName = Guid.NewGuid().ToString();

		//로그인
		await VivoxService.Instance.LoginAsync(options);

		Debug.Log("Vivox Login");
	}

}
