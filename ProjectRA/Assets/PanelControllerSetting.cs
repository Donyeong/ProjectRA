using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelControllerSetting : SettingUIBase
{
	public GameObject slotOrigin;

	public List<UIKeySettingSLot> keySettingSlots = new List<UIKeySettingSLot>();
	public override void Init()
	{
		base.Init();
	}

	public override void OnOpen()
	{
		base.OnOpen();
		SetupKey();
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
				slotComponent.button.onClick.AddListener(() => OnSlotClick(inputInfo));
			}
		}
	}

	public void OnSlotClick(InputInfo inputInfo)
	{

	}
}
