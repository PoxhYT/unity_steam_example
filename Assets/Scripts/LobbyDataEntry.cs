using Steamworks;
using UnityEngine;
using TMPro;

public class LobbyDataEntry : MonoBehaviour
{
    public CSteamID LobbyID;
    public string LobbyName;
    public TMP_Text LobbyNameText;

    public void SetLobbyData()
    {
        if(LobbyName != "")
        {
            LobbyNameText.text = LobbyName;
        }
    }

    public void JoinLobby()
    {
        SteamLobby.Instance.JoinLobby(LobbyID);
    }
}
