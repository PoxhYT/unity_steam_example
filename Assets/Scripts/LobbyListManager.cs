using Steamworks;
using System.Collections.Generic;
using UnityEngine;

public class LobbyListManager : MonoBehaviour
{
    public static LobbyListManager Instance;

    public GameObject LobbiesMenu;
    public GameObject LobbyDataItem;
    public GameObject LobbyListContent;
    public GameObject LobbyButton, HostButton;
    public List<GameObject> lobbies = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void GetListOfLobbies()
    {
        LobbyButton.SetActive(false);
        HostButton.SetActive(false);
        LobbiesMenu.SetActive(true);
        SteamLobby.Instance.GetLobbiesList();
    }

    public void DisplayLobbies(List<CSteamID> LobbyIDs, LobbyDataUpdate_t result)
    {
        for(int i = 0; i < LobbyIDs.Count; i++)
        {
            CSteamID lobby = LobbyIDs[i];
            if (lobby.m_SteamID == result.m_ulSteamIDLobby)
            {
                GameObject CreatedItem = Instantiate(LobbyDataItem);
                CreatedItem.GetComponent<LobbyDataEntry>().LobbyID = (CSteamID)lobby.m_SteamID;

                string LobbyName = SteamMatchmaking.GetLobbyData((CSteamID)lobby.m_SteamID, "name");
                if(LobbyName.Contains("STB"))
                {
                    CreatedItem.GetComponent<LobbyDataEntry>().LobbyName = LobbyName;
                    CreatedItem.GetComponent<LobbyDataEntry>().SetLobbyData();
                    CreatedItem.transform.SetParent(LobbyListContent.transform);
                    CreatedItem.transform.localPosition = Vector3.one;
                    lobbies.Add(CreatedItem);
                }  else
                {
                    Destroy(CreatedItem);
                }
            }
        }
    }

    public void DestroyLobbies()
    {
        foreach(GameObject lobby in lobbies)
        {
            Destroy(lobby);
        }
        lobbies.Clear();
    }
}
