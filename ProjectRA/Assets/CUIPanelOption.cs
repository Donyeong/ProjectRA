using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public enum eSettingCategory
{
	Graphics,
	Audio,
	Controls,
	Friend,
	Gameplay
}
public class CUIPanelOption : UIPanelBase
{
	public RAButton close;
	public eSettingCategory currentCategory;
	public SettingUIBase[] settingUIs;
	public SettingUIBase currentOpenUI;
	public GameObject selectImage;

	public List<RASettingButton> buttons;
	public GameObject origin;

	private RectTransform selectRect;
	private RectTransform[] buttonRects;
	private Vector3 targetPos;
	public Vector2 targetOffset;
	public float moveSpeed = 10f;

	void Start()
	{
		SettingButtons();
		ChangeCategory(eSettingCategory.Graphics);

		selectRect = selectImage.GetComponent<RectTransform>();
		buttonRects = new RectTransform[buttons.Count];
		foreach(SettingUIBase ui in settingUIs)
		{
			ui.Init();
			ui.gameObject.SetActive(false);
		}

		for (int i = 0; i < buttons.Count; i++)
		{
			buttonRects[i] = buttons[i].button.GetComponent<RectTransform>();
			int idx = i;
			EventTrigger trigger = buttons[i].gameObject.AddComponent<EventTrigger>();
			var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
			entry.callback.AddListener((data) => OnButtonHover(idx));
			trigger.triggers.Add(entry);
		}
		// 초기 위치
		targetPos = selectRect.anchoredPosition;

		close.onClick.AddListener(() => UIPanelManager.Instance.HidePanel(this));
	}

	public void SettingButtons()
	{
		//eSettingCategory 타입별로 버튼 생성
		origin.SetActive(false);
		foreach (eSettingCategory category in System.Enum.GetValues(typeof(eSettingCategory)))
		{
			GameObject buttonObj = Instantiate(origin, origin.transform.parent);
			buttonObj.SetActive(true);
			RASettingButton button = buttonObj.GetComponent<RASettingButton>();
			button.Init(category);

			//클릭 이벤트 추가
			button.button.onClick.AddListener(() => ChangeCategory(category));

			buttons.Add(button);
		}
	}

	void Update()
	{
		// 부드럽게 이동
		selectRect.anchoredPosition = Vector3.Lerp(selectRect.anchoredPosition, targetPos, Time.deltaTime * moveSpeed);
	}

	void OnButtonHover(int idx)
	{
		Vector2 worldPos = buttonRects[idx].position;
		Vector2 localPos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			selectRect.parent as RectTransform,
			RectTransformUtility.WorldToScreenPoint(null, worldPos),
			null,
			out localPos
		);
		targetPos = localPos + targetOffset;
	}

	void ChangeCategory(eSettingCategory category)
	{
		SettingUIBase ui = settingUIs.FirstOrDefault(i => i.settingCategory == category);
		if(ui == null)
		{
			Debug.LogError("No SettingUIBase found for category: " + category);
			return;
		}
		currentCategory = category;
		if(currentOpenUI != null)
		{
			currentOpenUI.gameObject.SetActive(false);
		}
		currentOpenUI = ui;
		currentOpenUI.gameObject.SetActive(true);
		currentOpenUI.OnOpen();
	}
}
