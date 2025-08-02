using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelControllerSetting : SettingUIBase
{
	public GameObject slotOrigin;

	public List<UIKeySettingSLot> keySettingSlots = new List<UIKeySettingSLot>();

	public UIKeySettingSLot currentChangeMode = null;
	public override void Init()
	{
		base.Init();
	}

	public override void OnOpen()
	{
		base.OnOpen();
		SetupKey();
	}


	public void Update()
	{
		if (currentChangeMode != null)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				currentChangeMode.Setup(currentChangeMode.info);
				currentChangeMode = null;
				return;
			}

			if (Input.anyKeyDown)
			{
				KeyCode keyCode = KeyCode.None;
				foreach (KeyCode code in System.Enum.GetValues(typeof(KeyCode)))
				{
					if (Input.GetKeyDown(code))
					{
						keyCode = code;
						break;
					}
				}

				if (keyCode != KeyCode.None)
				{
					currentChangeMode.info.keyCode = keyCode;
					currentChangeMode.Setup(currentChangeMode.info);
					currentChangeMode = null;
				}
			}
		}
	}

	public void Clear()
	{
		foreach (var slot in keySettingSlots)
		{
			Destroy(slot.gameObject);
		}
		keySettingSlots.Clear();
	}

	public void SetupKey()
	{
		Clear();
		slotOrigin.SetActive(false);
		foreach (var key in RAInputManager.Instance.keySetting)
		{
			InputInfo inputInfo = key.Value;
			if (inputInfo.inputType == eInputType.Button)
			{
				var slot = Instantiate(slotOrigin, slotOrigin.transform.parent);
				var slotComponent = slot.GetComponent<UIKeySettingSLot>();
				slotComponent.Setup(inputInfo);
				keySettingSlots.Add(slotComponent);
				slot.SetActive(true);
				slotComponent.button.onClick.AddListener(() => OnSlotClick(slotComponent));
			}
		}
	}

	public void OnSlotClick(UIKeySettingSLot slot)
	{
		if(currentChangeMode != null)
		{
			currentChangeMode.Setup(slot.info);
		}
		currentChangeMode = slot;
		currentChangeMode.SetChangeMode();
	}
}
