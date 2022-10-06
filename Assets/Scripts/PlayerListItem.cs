using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Steamworks;

public class PlayerListItem : MonoBehaviour
{
    public string PlayerName;
    public int ConnectionID;
    public ulong PlayerSteamID;
    bool AvatarReceived;

    public TMP_Text PlayerNameText;
    public TMP_Text PlayerReadyText;
    public bool IsPlayerReady;
    public RawImage PlayerImage;

    protected Callback<AvatarImageLoaded_t> ImageLoaded;

    public void ToggleReadyStatus()
    {
        if(IsPlayerReady)
        {
            PlayerReadyText.color = Color.green;
            PlayerReadyText.text = "Ready";
        } else
        {
            PlayerReadyText.color = Color.red;
            PlayerReadyText.text = "Unready";
        }
    }

    private void Start()
    {
        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);    
    }

    public void SetPlayerData()
    {
        PlayerNameText.text = PlayerName;
        ToggleReadyStatus();
        if (!AvatarReceived) { GetPlayerIcon(); }
    }

    void GetPlayerIcon()
    {
        int ImageID = SteamFriends.GetLargeFriendAvatar((CSteamID)PlayerSteamID);
        if (ImageID == -1) return;
        PlayerImage.texture = GetSteamImageAsTexture(ImageID);
    }

    void OnImageLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID != PlayerSteamID) return;
        PlayerImage.texture = GetSteamImageAsTexture(callback.m_iImage);
    }

    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if (isValid)
        {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        AvatarReceived = true;
        return texture;
    }
}
