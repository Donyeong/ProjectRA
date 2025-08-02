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

		// �ػ� ��� �ʱ�ȭ
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

		// ��üȭ�� ��� �ʱ�ȭ
		fullscreenToggle.isOn = Screen.fullScreen;

		// ǰ�� ���� �ʱ�ȭ
		qualityDropdown.ClearOptions();
		qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
		qualityDropdown.value = QualitySettings.GetQualityLevel();
		qualityDropdown.RefreshShownValue();

		// �̺�Ʈ ����
		resolutionDropdown.onValueChanged.AddListener(SetResolution);
		fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
		qualityDropdown.onValueChanged.AddListener(SetQuality);
	}

	public override void OnOpen()
	{
		base.OnOpen();
		// �ʿ�� �߰� �ʱ�ȭ
	}

	public override void OnExit()
	{
		base.OnExit();
		// �ʿ�� ���� ����
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