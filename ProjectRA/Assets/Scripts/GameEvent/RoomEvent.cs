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

