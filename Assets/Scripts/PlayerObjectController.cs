using UnityEngine;
using Mirror;
using Steamworks;

public class PlayerObjectController : NetworkBehaviour
{
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerID;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool IsPlayerReady;

    public static LobbyController Instance;

    CustomNetworkManager CustomNetworkManager;

    CustomNetworkManager GetCustomNetworkManager
    {
        get
        {
            if(CustomNetworkManager != null)
            {
                return CustomNetworkManager;
            }
            return CustomNetworkManager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    public override void OnStartAuthority()
    {
        CommandSetPlayerName(SteamFriends.GetPersonaName().ToString());
        Debug.Log(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalPlayer";
        LobbyController.Instance.FindPlayer();
        LobbyController.Instance.UpdateLobbyName();
    }

    public override void OnStartClient()
    {
        GetCustomNetworkManager.Players.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        GetCustomNetworkManager.Players.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
    }

    [Command] void CommandSetPlayerName(string name)
    {
        this.PlayerNameUpdate(this.PlayerName, name);
    }

    [Command]
    void CommandSetPlayerReady()
    {
        this.PlayerReadyUpdate(this.IsPlayerReady, !this.IsPlayerReady);
    }

    public void ToggleChange()
    {
        if(hasAuthority)
        {
            CommandSetPlayerReady();
        }
    }

    public void PlayerNameUpdate(string OldValue, string NewValue)
    {
        if(isServer)
        {
            this.PlayerName = NewValue;
        }

        if(isClient)
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    public void PlayerReadyUpdate(bool OldVlaue, bool NewValue)
    {
        if(isServer)
        {
            this.IsPlayerReady = NewValue;
        }

        if (isClient) 
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }
} 
