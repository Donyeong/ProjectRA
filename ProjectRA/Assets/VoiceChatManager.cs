using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Vivox;
using UnityEngine;
[Serializable]
public class Channel3DSetting
{

	//가청거리
	[SerializeField] private int audibleDistance = 32;

	//작아지기 시작하는 거리
	[SerializeField] private int conversationalDistance = 1;

	//FadeModel에 따른 감쇠 강도
	[SerializeField] private float audioFadeIntensityByDistance = 1.0f;

	//위치에따른 음량 감쇠 모델
	[SerializeField] private AudioFadeModel audioFadeModel = AudioFadeModel.InverseByDistance;

	public Channel3DProperties GetChannel3DProperties()
	{

		return new Channel3DProperties(audibleDistance, conversationalDistance, audioFadeIntensityByDistance, audioFadeModel);

	}

}

public class VoicePlayer
{
	VivoxParticipant vivoxParticipant;

}
public class VoiceChannel
{
	public string channelName;
	public List<VoicePlayer> voicePlayers = new List<VoicePlayer>();
}


public class VoiceChatManager : SingletonMono<VoiceChatManager>
{
	[SerializeField] private Channel3DSetting channel3DSetting = new Channel3DSetting();
	[SerializeField] private float positonUpdateRate = 0.5f;

	public bool isJoinedChannel = false;
	public string currentChannelName = String.Empty;
	public string localPlayerId = String.Empty;
	// Start is called before the first frame update
	void Start()
    {

	}

    // Update is called once per frame
    void Update()
    {
		VivoxService.Instance.ParticipantAddedToChannel += OnParticipantAddedToChannel;
		VivoxService.Instance.ParticipantRemovedFromChannel += OnParticipantRemovedFromChannel;
		var channels = VivoxService.Instance.ActiveChannels;
	}

	private void OnParticipantAddedToChannel(VivoxParticipant e)
	{
		e.ParticipantSpeechDetected += () =>
		{
			//Debug.Log($"Participant {e.ParticipantId} is speaking in channel {e.ChannelName}");
		};
	}

	private void OnParticipantRemovedFromChannel(VivoxParticipant e)
	{
	}
	public string GetChannelNameFromRoom()
    {
		string roomCode = RANetworkManager.instance.relayJoinCode;
		return $"RA_VC_{roomCode}";
	}

    public void JoinVoidChannel()
    {
		string channelName = GetChannelNameFromRoom();
		VivoxService.Instance.JoinGroupChannelAsync(channelName, ChatCapability.AudioOnly);
        isJoinedChannel = true;
		currentChannelName = channelName;
	}

	public void Join3DChannel()
	{
		if (isJoinedChannel)
		{
			ExitVoidChannel();
		}
		string channelName = GetChannelNameFromRoom();
		VivoxService.Instance.JoinPositionalChannelAsync(channelName, ChatCapability.AudioOnly, channel3DSetting.GetChannel3DProperties());
		isJoinedChannel = true;
		currentChannelName = channelName;
		
	}

	public void UpdateMicVolume(int _volume)
	{
		VivoxService.Instance.SetInputDeviceVolume(_volume);
	}

	public void SetMicMute(bool mute)
	{
		if (mute) {
			VivoxService.Instance.MuteInputDevice();
		} else
		{
			VivoxService.Instance.UnmuteInputDevice();
		}
	}

	public void ExitVoidChannel()
	{
		if(!isJoinedChannel)
		{
			return;
		}
		VivoxService.Instance.LeaveChannelAsync(currentChannelName);
		isJoinedChannel = false;
	}

	public void UpdatePosition(GameObject speakeObj, string channelName)
	{
		VivoxService.Instance.Set3DPosition(speakeObj, channelName);
	}
}
