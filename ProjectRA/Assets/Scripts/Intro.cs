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

		//�α��� �ɼ� ����
		LoginOptions options = new LoginOptions();

		//���÷��� �̸� ����
		options.DisplayName = Guid.NewGuid().ToString();

		//�α���
		await VivoxService.Instance.LoginAsync(options);

		Debug.Log("Vivox Login");
	}

}
