using Mirror.BouncyCastle.Asn1.X509;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class UIPanelRoomOption : UIPanelBase
{
	public RAButton btnClose;

	public Slider sliderRoomSize;
	public TMP_Text textRoomSize;

	public void Start()
	{
		btnClose.onClick.AddListener(() => UIPanelManager.Instance.HidePanel(this));
		sliderRoomSize.onValueChanged.AddListener(OnSliderRoomSizeChanged);
	}
	public override void OnShowPanel()
	{
		base.OnShowPanel();

		sliderRoomSize.value = CGameManager.Instance.roomOption.mapSize;
		textRoomSize.text = $"{sliderRoomSize.value}";
	}


	public void OnSliderRoomSizeChanged(float value)
	{
		textRoomSize.text = $"{sliderRoomSize.value}";
		UpdateRoomOption();
	}

	public void UpdateRoomOption()
	{
		CGameManager.Instance.roomOption.mapSize = (int)sliderRoomSize.value;
		Packet_RoomOptionChangeReq packet = new Packet_RoomOptionChangeReq();
		packet.roomOption = CGameManager.Instance.roomOption;
		RANetworkManager.instance.SendPacketToServer(packet);
		Debug.Log($"Room option changed: {packet.roomOption}");
	}
}
