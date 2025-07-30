using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ePlayerState
{
	Normal,
	FallDown,
	Dead,
}
public class RAPlayer : Actor
{
	public Transform camera_position;

	/// <summary>
	/// The Sessions ID for the current server.
	/// </summary>
	[SyncVar]
	public string sessionId = "";


	/// <summary>
	/// Player name.
	/// </summary>
	public string username;

	public string ip;

	/// <summary>
	/// Platform the user is on.
	/// </summary>
	public string platform;
	public float power = 5f;

	public float hp;
	public float maxHp;
	public float stamina;
	public float maxStamina = 50;
	public bool exhaustion = false;
	public float exhaustionTime = 0f;

	public float staminaHealCooldown = 0;

	public ePlayerState currentState = ePlayerState.Normal;

	public Vector3 viewDir = Vector3.forward;

	public PlayerAnimController playerAnimController;
	public RAPLayerMovement playerMovement;

	public bool isCrouched {
		get {
			return playerMovement.character.IsCrouched();
		}
	}

	private void Awake()
	{
		username = SystemInfo.deviceName;
		platform = Application.platform.ToString();
		if (NetworkManager.singleton != null) {
			ip = NetworkManager.singleton.networkAddress;
		}
		playerAnimController = GetComponentInChildren<PlayerAnimController>();
		playerMovement = GetComponent<RAPLayerMovement>();
	}

	private void Start()
	{
	}

	public void Update()
	{
		if (exhaustion)
		{
			exhaustionTime -= Time.deltaTime;
			if (exhaustionTime <= 0f)
			{
				exhaustionTime = 0f;
				exhaustion = false;

				GameRoomEvent_OnUpdateExhaustion roomEvent = new GameRoomEvent_OnUpdateExhaustion();
				roomEvent.target = this;
				roomEvent.isExhaustion = false;
				CGameManager.Instance.roomEventBus.Publish(roomEvent);
			}
		}
		if (staminaHealCooldown >= 0)
		{
			staminaHealCooldown -= Time.deltaTime;
			if( staminaHealCooldown <= 0f )
			{
				staminaHealCooldown = 0f;
			}
		}
		if(staminaHealCooldown <= 0 && stamina < maxStamina)
		{
			float prev = stamina;
			stamina += Time.deltaTime * 5f; // Regenerate stamina over time
			if (stamina > maxStamina)
				stamina = maxStamina;

			GameRoomEvent_OnUpdateStamina roomEventStamina = new GameRoomEvent_OnUpdateStamina();
			roomEventStamina.target = this;
			roomEventStamina.delta = stamina - prev;
			CGameManager.Instance.roomEventBus.Publish(roomEventStamina);
		}
	}

	/// <summary>
	/// Called after player has spawned in the scene.
	/// </summary>
	public override void OnStartServer()
	{
		Debug.Log("Player has been spawned on the server!");
	}

	public bool UseStamina(float val)
	{
		if(exhaustion)
			return false;

		staminaHealCooldown = 0.1f;
		if (stamina >= val)
		{
			stamina -= val;
			GameRoomEvent_OnUpdateStamina roomEventStamina = new GameRoomEvent_OnUpdateStamina();
			roomEventStamina.target = this;
			roomEventStamina.delta = -val;
			CGameManager.Instance.roomEventBus.Publish(roomEventStamina);
		} else
		{
			GameRoomEvent_OnUpdateStamina roomEventStamina = new GameRoomEvent_OnUpdateStamina();
			roomEventStamina.target = this;
			roomEventStamina.delta = stamina;
			CGameManager.Instance.roomEventBus.Publish(roomEventStamina);
			stamina = 0;
			exhaustion = true;
			exhaustionTime = 3f;

			GameRoomEvent_OnUpdateExhaustion roomEvent = new GameRoomEvent_OnUpdateExhaustion();
			roomEvent.target = this;
			roomEvent.isExhaustion = true;
			CGameManager.Instance.roomEventBus.Publish(roomEvent);
		}

		return true;
	}

	public void TakeDamage(AttackInfo attackInfo)
	{
		hp -= attackInfo.damage;
		hp = Mathf.Clamp(hp, 0, maxHp);
		GameRoomEvent_OnPlayerDamage roomEvent = new GameRoomEvent_OnPlayerDamage();
		roomEvent.target = this;
		roomEvent.attackInfo = attackInfo;
		CGameManager.Instance.roomEventBus.Publish(roomEvent);

		playerMovement.Knockback(attackInfo.direction, attackInfo.knockbackPower);


		if (hp == 0)
		{
			currentState = ePlayerState.Dead;
			playerAnimController.SetDead();
			playerMovement.character.SetMovementMode(ECM2.Character.MovementMode.None);
			playerMovement.enabled = false;
			GameRoomEvent_OnPlayerDie gameRoomEvent_OnPlayerDie = new GameRoomEvent_OnPlayerDie();
			gameRoomEvent_OnPlayerDie.target = this;
			CGameManager.Instance.roomEventBus.Publish(gameRoomEvent_OnPlayerDie);
		}
	}
}

public enum eAttackType
{
	MonsterAttack
}
public class AttackInfo
{
	public Actor attacker;
	public float damage;
	public Vector3 direction;
	public float knockbackPower;
	public eAttackType attackType;
}