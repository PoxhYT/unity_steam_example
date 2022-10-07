using Mirror;
using Steamworks;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance;

    [SerializeField] GameObject notification;

    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;

    protected Callback<LobbyMatchList_t> LobbyList;
    protected Callback<LobbyDataUpdate_t> LobbyDataUpdated;
    public List<CSteamID> LobbyIDs = new List<CSteamID>();

    [SerializeField] TMP_Text LobbyType;
    [SerializeField] TMP_Text LobbyName;
    [SerializeField] GameObject LobbyCreation;
    [SerializeField] GameObject MainButtons;

    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";
    CustomNetworkManager Manager;

    private void Start()
    {
        if (!SteamManager.Initialized) { Debug.Log("Steam is not open!"); notification.SetActive(true); return; }
        if(Instance == null) { Instance = this; }
        Manager = GetComponent<CustomNetworkManager>();

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinReuest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        LobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
        LobbyDataUpdated = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);
    }

    public void OpenLobbyCreation()
    {
        MainButtons.SetActive(false);
        LobbyCreation.SetActive(true);
    }

    public void HostLobby()
    {
        ELobbyType ELobbyType;
        switch(LobbyType.text)
        {
            case "Friends only":
                ELobbyType = ELobbyType.k_ELobbyTypeFriendsOnly;
                break;
            default:
                ELobbyType = ELobbyType.k_ELobbyTypePublic;
                break;

        }
        SteamMatchmaking.CreateLobby(ELobbyType, Manager.maxConnections);
    }

    void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK) { return; }
        Debug.Log("Lobby created succesfully");
        Manager.StartHost();
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", LobbyName.text);
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "type", ELobbyType.k_ELobbyTypePublic.ToString());

        print(SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "type"));
    }

    void OnJoinReuest(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Request to join");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    void OnLobbyEntered(LobbyEnter_t callback)
    {
        CurrentLobbyID = callback.m_ulSteamIDLobby;
        /*LobbyName.text = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name");
        LobbyName.gameObject.SetActive(true);*/

        if (NetworkServer.active) { return; }
        Manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        Manager.StartClient();
    }

    public void JoinLobby(CSteamID LobbyID)
    {
        SteamMatchmaking.JoinLobby(LobbyID);
    }

    public void GetLobbiesList()
    {
        if (LobbyIDs.Count > 0) LobbyIDs.Clear();
        SteamMatchmaking.AddRequestLobbyListResultCountFilter(60);
        SteamMatchmaking.RequestLobbyList();
    }

    void OnGetLobbyList(LobbyMatchList_t callback)
    {
        if (LobbyListManager.Instance.lobbies.Count > 0) LobbyListManager.Instance.DestroyLobbies();

        for(int i = 0; i < callback.m_nLobbiesMatching; i++) 
        {
            CSteamID LobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            LobbyIDs.Add(LobbyID);
            SteamMatchmaking.RequestLobbyData(LobbyID);
        }
    }

    void OnGetLobbyData(LobbyDataUpdate_t callback)
    {
        LobbyListManager.Instance.DisplayLobbies(LobbyIDs, callback);
    }
}