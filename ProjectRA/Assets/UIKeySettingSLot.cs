using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIKeySettingSLot : MonoBehaviour
{
	public InputInfo info;
	public Image icon;
	public TMP_Text keyType;
	public RAButton button;



	public void Setup(InputInfo _info)
	{
		info = _info;
		icon.sprite = RAInputManager.Instance.GetIcon(_info.keyCode);
		keyType.text = info.inputContentType.ToString();
	}




}

