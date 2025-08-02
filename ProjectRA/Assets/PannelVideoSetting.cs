using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PannelVideoSetting : SettingUIBase
{
	public TMP_Dropdown resolutionDropdown;
	public Toggle fullscreenToggle;
	public TMP_Dropdown qualityDropdown;

	private Resolution[] resolutions;

	public override void Init()
	{
		base.Init();

		// 해상도 목록 초기화
		resolutions = Screen.resolutions;
		resolutionDropdown.ClearOptions();
		List<string> options = new List<string>();
		int currentResIndex = 0;
		for (int i = 0; i < resolutions.Length; i++)
		{
			string option = resolutions[i].width + " x " + resolutions[i].height + " @" + resolutions[i].refreshRate + "Hz";
			options.Add(option);
			if (resolutions[i].width == Screen.currentResolution.width &&
				resolutions[i].height == Screen.currentResolution.height)
			{
				currentResIndex = i;
			}
		}
		resolutionDropdown.AddOptions(options);
		resolutionDropdown.value = currentResIndex;
		resolutionDropdown.RefreshShownValue();

		// 전체화면 토글 초기화
		fullscreenToggle.isOn = Screen.fullScreen;

		// 품질 설정 초기화
		qualityDropdown.ClearOptions();
		qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
		qualityDropdown.value = QualitySettings.GetQualityLevel();
		qualityDropdown.RefreshShownValue();

		// 이벤트 연결
		resolutionDropdown.onValueChanged.AddListener(SetResolution);
		fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
		qualityDropdown.onValueChanged.AddListener(SetQuality);
	}

	public override void OnOpen()
	{
		base.OnOpen();
		// 필요시 추가 초기화
	}

	public override void OnExit()
	{
		base.OnExit();
		// 필요시 설정 저장
	}

	void SetResolution(int index)
	{
		Resolution res = resolutions[index];
		Screen.SetResolution(res.width, res.height, Screen.fullScreen, res.refreshRate);
	}

	void SetFullscreen(bool isFullscreen)
	{
		Screen.fullScreen = isFullscreen;
	}

	void SetQuality(int index)
	{
		QualitySettings.SetQualityLevel(index);
	}
}