using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoomEvent
{
	public RoomEvent()
	{
	}
}
public class GameRoomEvent_OnInteractAimIn : RoomEvent
{
	public InteractableObject interactableObject;
}
public class GameRoomEvent_OnInteractAimOut : RoomEvent
{
}
public class GameRoomEvent_OnGrabProp : RoomEvent
{
	public RAProp targetProp;
}
public class GameRoomEvent_OnDamageProp : RoomEvent
{
	public RAProp targetProp;
}
public class GameRoomEvent_OnDropProp : RoomEvent
{
}

public class GameRoomEvent_OnStartButtonClick : RoomEvent
{
}

public class GameRoomEvent_OnRoomSettingButtonClick : RoomEvent
{
}


public class GameRoomEvent_OnPlayerDamage : RoomEvent
{
	public RAPlayer target;
	public AttackInfo attackInfo;
}
public class GameRoomEvent_GenerateLocalPlayer : RoomEvent
{
}

public class GameRoomEvent_OnUpdateStamina : RoomEvent
{
	public RAPlayer target;
	public float delta;
}

public class GameRoomEvent_OnHealPlayer : RoomEvent
{
	public RAPlayer target;
	public float delta;
}


public class GameRoomEvent_OnUpdateExhaustion : RoomEvent
{
	public RAPlayer target;
	public bool isExhaustion;
}

public class GameRoomEvent_OnPlayerDie : RoomEvent
{
	public RAPlayer target;
}
