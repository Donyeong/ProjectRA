using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum eControllerType
{
    KeyboardAndMouse,
}
public enum eInputType
{
	Axis,
    Button
}
public enum eInputContentType
{
	Jump,
	Crouch,
	Move,
	Aim,
	Interact,
	Grab,
	None,
}

[SerializeField]
public class InputInfo
{
	public eInputType inputType;
	public eInputContentType inputContentType;
	public KeyCode keyCode;
}

public class RAInputManager : SingletonMono<RAInputManager>
{
	public Dictionary<eInputContentType, InputInfo> keySetting = new Dictionary<eInputContentType, InputInfo>();
	public eControllerType controllerType = eControllerType.KeyboardAndMouse;


	protected override void Awake()
	{
		base.Awake();
		LoadKey();
	}

	public void LoadKey()
	{
		keySetting.Add(eInputContentType.Jump, new InputInfo {
			inputType = eInputType.Button,
			inputContentType = eInputContentType.Jump,
			keyCode = KeyCode.Space
		});

		keySetting.Add(eInputContentType.Crouch, new InputInfo {
			inputType = eInputType.Button,
			inputContentType = eInputContentType.Crouch,
			keyCode = KeyCode.LeftControl
		});

		keySetting.Add(eInputContentType.Move, new InputInfo {
			inputType = eInputType.Axis,
			inputContentType = eInputContentType.Move,
			keyCode = KeyCode.None // Axis does not use KeyCode
		});

		keySetting.Add(eInputContentType.Aim, new InputInfo {
			inputType = eInputType.Axis,
			inputContentType = eInputContentType.Aim,
			keyCode = KeyCode.None // Axis does not use KeyCode
		});

		keySetting.Add(eInputContentType.Interact, new InputInfo {
			inputType = eInputType.Button,
			inputContentType = eInputContentType.Interact,
			keyCode = KeyCode.E
		});

		keySetting.Add(eInputContentType.Grab, new InputInfo {
			inputType = eInputType.Button,
			inputContentType = eInputContentType.Grab,
			keyCode = KeyCode.F
		});


	}

	public void SetKeySetting(eInputContentType contentType, InputInfo inputInfo)
	{
		if (keySetting.ContainsKey(contentType))
		{
			keySetting[contentType] = inputInfo;
		}
		else
		{
			keySetting.Add(contentType, inputInfo);
		}
	}

	public Sprite GetIcon(KeyCode keyCode)
	{
		string iconName = string.Empty;
		//숫자키인경우
		if (keyCode >= KeyCode.Alpha0 && keyCode <= KeyCode.Alpha9)
		{
			iconName = $"keyboard_{(int)keyCode - (int)KeyCode.Alpha0}";
		}
		else if (keyCode >= KeyCode.Keypad0 && keyCode <= KeyCode.Keypad9)
		{
			iconName = $"keyboard_{(int)keyCode - (int)KeyCode.Keypad0}";
		}
		else if (keyCode == KeyCode.Space)
		{
			iconName = "keyboard_space";
		}
		else if (keyCode == KeyCode.LeftControl || keyCode == KeyCode.RightControl)
		{
			iconName = "keyboard_ctrl";
		}
		else if (keyCode == KeyCode.Return || keyCode == KeyCode.KeypadEnter)
		{
			iconName = "keyboard_enter";
		} else
		{	
			iconName = $"keyboard_{keyCode.ToString().ToLower()}";
		}
		return Resources.Load<Sprite>("UI/InputIcon/KeyboardMouse/Default/" + iconName);
	}
}
