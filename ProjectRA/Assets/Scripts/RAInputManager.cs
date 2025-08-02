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
	KeyboardMoveLeft,
	KeyboardMoveRight,
	KeyboardMoveForward,
	KeyboardMoveBack,
	GrabRangeUp,
	GrabRangeDown,
	Sprint,
	None,
}

[SerializeField]
public class InputInfo
{
	public eInputType inputType;
	public eInputContentType inputContentType;
	public KeyCode keyCode;
	public bool isKeyboardOnly = false;
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

	public void Update()
	{
		
	}

	public bool GetKeyDown(eInputContentType key)
	{
		if (keySetting.ContainsKey(key) && keySetting[key].inputType == eInputType.Button)
		{
			return Input.GetKeyDown(keySetting[key].keyCode);
		}
		return false;
	}

	public bool GetKey(eInputContentType key)
	{
		if (keySetting.ContainsKey(key) && keySetting[key].inputType == eInputType.Button)
		{
			return Input.GetKey(keySetting[key].keyCode);
		}
		return false;
	}

	public bool GetKeyUp(eInputContentType key)
	{
		if (keySetting.ContainsKey(key) && keySetting[key].inputType == eInputType.Button)
		{
			return Input.GetKeyUp(keySetting[key].keyCode);
		}
		return false;
	}

	public void LoadKey()
	{
		keySetting.Add(eInputContentType.Jump, new InputInfo {
			inputType = eInputType.Button,
			inputContentType = eInputContentType.Jump,
			keyCode = KeyCode.Space,
			isKeyboardOnly = false
		});

		keySetting.Add(eInputContentType.Crouch, new InputInfo {
			inputType = eInputType.Button,
			inputContentType = eInputContentType.Crouch,
			keyCode = KeyCode.LeftControl,
			isKeyboardOnly = false
		});

		keySetting.Add(eInputContentType.Move, new InputInfo {
			inputType = eInputType.Axis,
			inputContentType = eInputContentType.Move,
			keyCode = KeyCode.None,
			isKeyboardOnly = false
		});

		keySetting.Add(eInputContentType.Aim, new InputInfo {
			inputType = eInputType.Axis,
			inputContentType = eInputContentType.Aim,
			keyCode = KeyCode.None,
			isKeyboardOnly = false
		});

		keySetting.Add(eInputContentType.Interact, new InputInfo {
			inputType = eInputType.Button,
			inputContentType = eInputContentType.Interact,
			keyCode = KeyCode.E,
			isKeyboardOnly = false
		});

		keySetting.Add(eInputContentType.Grab, new InputInfo {
			inputType = eInputType.Button,
			inputContentType = eInputContentType.Grab,
			keyCode = KeyCode.Mouse0,
			isKeyboardOnly = false
		});
		keySetting.Add(eInputContentType.KeyboardMoveLeft, new InputInfo
		{
			inputType = eInputType.Button,
			inputContentType = eInputContentType.KeyboardMoveLeft,
			keyCode = KeyCode.A,
			isKeyboardOnly = true
		});
		keySetting.Add(eInputContentType.KeyboardMoveRight, new InputInfo {
			inputType = eInputType.Button,
			inputContentType = eInputContentType.KeyboardMoveRight,
			keyCode = KeyCode.D,
			isKeyboardOnly = true
		});
		keySetting.Add(eInputContentType.KeyboardMoveForward, new InputInfo {
			inputType = eInputType.Button,
			inputContentType = eInputContentType.KeyboardMoveForward,
			keyCode = KeyCode.W,
			isKeyboardOnly = true
		});
		keySetting.Add(eInputContentType.KeyboardMoveBack, new InputInfo {
			inputType = eInputType.Button,
			inputContentType = eInputContentType.KeyboardMoveBack,
			keyCode = KeyCode.S,
			isKeyboardOnly = true
		});
		keySetting.Add(eInputContentType.Sprint, new InputInfo {
			inputType = eInputType.Button,
			inputContentType = eInputContentType.Sprint,
			keyCode = KeyCode.LeftShift,
			isKeyboardOnly = false
		});
		keySetting.Add(eInputContentType.GrabRangeUp, new InputInfo {
			inputType = eInputType.Button,
			inputContentType = eInputContentType.GrabRangeUp,
			keyCode = KeyCode.Q,
			isKeyboardOnly = true
		});
		keySetting.Add(eInputContentType.GrabRangeDown, new InputInfo {
			inputType = eInputType.Button,
			inputContentType = eInputContentType.GrabRangeDown,
			keyCode = KeyCode.E,
			isKeyboardOnly = true
		});


	}


	public InputInfo GetKeyInfo(eInputContentType contentType)
	{
		return keySetting.ContainsKey(contentType) ? keySetting[contentType] : null;
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
		else if (keyCode == KeyCode.LeftShift || keyCode == KeyCode.RightShift)
		{
			iconName = "keyboard_shift";
		}
		else if (keyCode == KeyCode.LeftAlt || keyCode == KeyCode.RightAlt)
		{
			iconName = "keyboard_alt";
		}
		else if (keyCode == KeyCode.Mouse0)
		{
			iconName = "mouse_left";
		}
		else if (keyCode == KeyCode.Mouse1)
		{
			iconName = "mouse_right";
		}
		else if (keyCode == KeyCode.Mouse2)
		{
			iconName = "mouse_middle";
		}
		else if (keyCode == KeyCode.LeftArrow)
		{
			iconName = "keyboard_arrows_left";
		}
		else if (keyCode == KeyCode.RightArrow)
		{
			iconName = "keyboard_arrows_right";
		}
		else if (keyCode == KeyCode.UpArrow)
		{
			iconName = "keyboard_arrows_up";
		}
		else if (keyCode == KeyCode.DownArrow)
		{
			iconName = "keyboard_arrows_down";
		}
		else if (keyCode == KeyCode.LeftShift || keyCode == KeyCode.RightShift)
		{
			iconName = "keyboard_shift";
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
