using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelIngameHud : MonoBehaviour
{
    public TMP_Text price;
    public GameObject interactInfo;
	public TMP_Text interactInfoText;
	public GameObject propPrice;
	public TMP_Text propPriceText;
	public RAProp target = null;


	public TMP_Text hpText;
	public TMP_Text staminaText;
	public Color staminaNormal;
	public Color staminaExhausted;

	public GameObject playerListHud;
	public GameObject roleHud;
	public Image roleImage;
	public TMP_Text roleText;
		
	// Start is called before the first frame update
	void Start()
	{
		CGameManager.Instance.roomEventBus.AddListner<GameRoomEvent_GenerateLocalPlayer>(OnGenerateLocalPlayer);

		CGameManager.Instance.roomEventBus.AddListner<GameRoomEvent_OnDamageProp>(OnDamageProp);
		CGameManager.Instance.roomEventBus.AddListner<GameRoomEvent_SellPlaceUpdate>(OnSellPlaceUpdate);
		CGameManager.Instance.roomEventBus.AddListner<GameRoomEvent_OnInteractAimIn>(OnInteractAimIn);
		CGameManager.Instance.roomEventBus.AddListner<GameRoomEvent_OnInteractAimOut>(OnInteractAimOut);
		CGameManager.Instance.roomEventBus.AddListner<GameRoomEvent_OnGrabProp>(OnGrabProp);
		CGameManager.Instance.roomEventBus.AddListner<GameRoomEvent_OnDropProp>(OnDropProp);
		CGameManager.Instance.roomEventBus.AddListner<GameRoomEvent_OnDamageProp>(OnDamageProp);

		CGameManager.Instance.roomEventBus.AddListner<GameRoomEvent_OnUpdateStamina>(OnUpdateStamina);
		CGameManager.Instance.roomEventBus.AddListner<GameRoomEvent_OnUpdateExhaustion>(OnUpdateExhaustion);
		CGameManager.Instance.roomEventBus.AddListner<GameRoomEvent_OnPlayerDamage>(OnPlayerDamage);

		CGameManager.Instance.roomEventBus.AddListner<GameRoomEvent_OnStartGame>(OnGameStart);

	}

	// Update is called once per frame
	void Update()
    {
        
    }

	public void OnGameStart(GameRoomEvent_OnStartGame e)
	{
		roleHud.SetActive(true);
		SetRoleUI();
		SetPriceText(0);
		playerListHud.SetActive(false);
	}

	public void SetRoleUI()
	{
		eRoleType roleType = CGameManager.Instance.localPlayer.roleType;
		roleImage.sprite = ResourceManager.Instance.LoadResource<Sprite>($"UI/RoleImage/{roleType.ToString()}");
		string roleName = "";
		switch(roleType)
		{
			case eRoleType.Citizen:
				roleName = "시민";
				break;
			case eRoleType.Bumin:
				roleName = "범인";
				break;
			case eRoleType.None:
			default:
				roleName = "알 수 없음";
				break;
		}
		roleText.SetText(roleName);
	}

	public void OnGenerateLocalPlayer(GameRoomEvent_GenerateLocalPlayer e)
	{
		UpdateHpText(CGameManager.Instance.localPlayer);
		UpdateStaminaText(CGameManager.Instance.localPlayer);
	}

    public void OnSellPlaceUpdate(GameRoomEvent_SellPlaceUpdate e)
	{
		SetPriceText(e.sellPlace.GetTotalPrice());
	}

	public void SetPriceText(int currentPrice)
	{
		price.SetText($"{currentPrice.ToString()} / {CGameManager.Instance.goalPrice}");
	}

	public void OnInteractAimIn(GameRoomEvent_OnInteractAimIn e)
	{
        interactInfo.SetActive(true);
		interactInfoText.SetText(e.interactableObject.GetInteractText());
	}

	public void OnInteractAimOut(GameRoomEvent_OnInteractAimOut e)
	{
		interactInfo.SetActive(false);
	}

	public void OnGrabProp(GameRoomEvent_OnGrabProp e)
	{
		propPrice.SetActive(true);
		propPriceText.SetText(e.targetProp.price.ToString());
		target = e.targetProp;
	}
	public void OnDropProp(GameRoomEvent_OnDropProp e)
	{
		propPrice.SetActive(false);
		target = null;
	}

	public void OnDamageProp(GameRoomEvent_OnDamageProp e)
	{
		if(propPrice.activeInHierarchy)
		{
			if (e.targetProp == target)
			{
				propPriceText.SetText(e.targetProp.price.ToString());
			}
		}
	}

	public void OnUpdateStamina(GameRoomEvent_OnUpdateStamina e)
	{
		UpdateStaminaText(e.target);
	}

	public void OnUpdateExhaustion(GameRoomEvent_OnUpdateExhaustion e)
	{
		UpdateStaminaText(e.target);
	}
	public void OnPlayerDamage(GameRoomEvent_OnPlayerDamage e)
	{
		UpdateHpText(e.target);
	}

	public void UpdateHpText(RAPlayer player)
	{
		if (player == CGameManager.Instance.localPlayer)
		{
			hpText.SetText(player.hp.ToString("0") + "/" + player.maxHp.ToString("0"));
		}
	}

	public void UpdateStaminaText(RAPlayer player)
	{
		if (player == CGameManager.Instance.localPlayer)
		{
			staminaText.SetText(player.stamina.ToString("0") + "/" + player.maxStamina.ToString("0"));
			if (player.exhaustion)
			{
				staminaText.color = staminaExhausted;
			}
			else
			{
				staminaText.color = staminaNormal;
			}
		}
	}
}
