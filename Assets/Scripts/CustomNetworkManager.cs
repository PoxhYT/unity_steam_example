using Mirror;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] PlayerObjectController Player;
    public List<PlayerObjectController> Players { get; } = new List<PlayerObjectController>();

    public override void OnServerAddPlayer(NetworkConnectionToClient connection)
    {
        Debug.Log(SceneManager.GetActiveScene().name);
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            PlayerObjectController PlayerInstance = Instantiate(Player);

            PlayerInstance.ConnectionID = connection.connectionId;
            PlayerInstance.PlayerID = Players.Count + 1;
            PlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.CurrentLobbyID, Players.Count);

            NetworkServer.AddPlayerForConnection(connection, PlayerInstance.gameObject);
        }
    }
}
