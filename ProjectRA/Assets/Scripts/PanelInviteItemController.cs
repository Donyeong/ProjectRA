using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using System;
using Mirror;
using TMPro;

public class PanelInviteItemController : MonoBehaviour
{
	public string PlayerName;
	//public int ConnectionID;
	public ulong PlayerSteamID;
	private bool AvatarReceived;

	public TMP_Text PlayerNameText;
	public RawImage PlayerIcon;

	[SerializeField]
	private Button buttonInvite;

	protected Callback<AvatarImageLoaded_t> ImageLoaded;

	public void Init()
	{
		ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
		buttonInvite.onClick.AddListener(() =>
		{
			Debug.Log("Invite button clicked for player: " + PlayerName + " with SteamID: " + PlayerSteamID);
			SteamFriends.InviteUserToGame((CSteamID)PlayerSteamID, RANetworkManager.instance.relayJoinCode);
		});
		SetPlayerValues();
	}

	private void OnImageLoaded(AvatarImageLoaded_t callback)
	{
		Debug.Log($"PlayerSteamID : {PlayerSteamID}");
		if (callback.m_steamID.m_SteamID == PlayerSteamID)
		{
			PlayerIcon.texture = GetSteamImageAsTexture2D(callback.m_iImage);
		}
		else//another player
			return;
	}

	void GetPlayerIcon()
	{

		int ImageID = SteamFriends.GetLargeFriendAvatar((CSteamID)PlayerSteamID);//SteamFriends.GetSmallFriendAvatar((CSteamID)PlayerSteamID); //
		if (ImageID == -1)
		{
			Debug.Log($"no profile image : {(CSteamID)PlayerSteamID}");
			return;
		}

		PlayerIcon.texture = GetSteamImageAsTexture2D(ImageID);
	}

	public void SetPlayerValues()
	{
		PlayerNameText.text = PlayerName;
		if (!AvatarReceived)
			GetPlayerIcon();
	}

	private Texture2D GetSteamImageAsTexture2D(int iImage)
	{
		Texture2D ret = null;
		uint ImageWidth;
		uint ImageHeight;
		bool bIsValid = SteamUtils.GetImageSize(iImage, out ImageWidth, out ImageHeight);

		if (bIsValid)
		{
			byte[] Image = new byte[ImageWidth * ImageHeight * 4];

			bIsValid = SteamUtils.GetImageRGBA(iImage, Image, (int)(ImageWidth * ImageHeight * 4));
			if (bIsValid)
			{
				ret = new Texture2D((int)ImageWidth, (int)ImageHeight, TextureFormat.RGBA32, false, true);
				ret.LoadRawTextureData(Image);
				ret.Apply();
			}
		}

		return ret;
	}

}