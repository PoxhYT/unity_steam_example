using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System.Collections.Generic;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance;
    public PlayerObjectController PlayerController;

    public TMP_Text LobbyName;

    public GameObject PlayerListViewContent;
    public GameObject PlayerListItem;
    public GameObject LocalPlayerObject;

    public ulong CurrentLobbyID;
    public bool CreatedItem = false;
    public List<PlayerListItem> Items = new List<PlayerListItem>();

    public Button StartButton;
    public TMP_Text ReadyButtonText;

    CustomNetworkManager CustomNetworkManager;

    CustomNetworkManager GetCustomNetworkManager
    {
        get
        {
            if (CustomNetworkManager != null)
            {
                return CustomNetworkManager;
            }
            return CustomNetworkManager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;    
    }

    public void ReadyPlayer()
    {
        if(PlayerController.IsPlayerReady)
        {
            ReadyButtonText.text = "Unready";
        } else
        {
            ReadyButtonText.text = "Ready";
        }
        PlayerController.ToggleChange();
    }

    public void UpdateButton()
    {
        if(PlayerController.IsPlayerReady)
        {
            ReadyButtonText.text = "Unready";
        } else
        {
            ReadyButtonText.text = "Ready";
        }
    }

    public void CheckIfAllReady()
    {
        bool IsReadyToStart = false;
        foreach(PlayerObjectController player in GetCustomNetworkManager.Players)
        {
            if(player.IsPlayerReady)
            {
                IsReadyToStart = true;
            } else
            {
                IsReadyToStart = false;
                break;
            }
        }

        if(IsReadyToStart)
        {
            if(PlayerController.PlayerID == 1)
            {
                StartButton.interactable = true;
            } else
            {
                StartButton.interactable = false;
            }
        } else
        {
            StartButton.interactable = false;
        }
    }

    public void UpdateLobbyName()
    {
        CurrentLobbyID = GetCustomNetworkManager.GetComponent<SteamLobby>().CurrentLobbyID;
        LobbyName.text = SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "name");
    }

    public void UpdatePlayerList()
    {
        if (!CreatedItem) CreateHostPlayerItem();
        if (Items.Count < GetCustomNetworkManager.Players.Count) CreateClientPlayerItem();
        if (Items.Count > GetCustomNetworkManager.Players.Count) RemovePlayerItem();
        if (Items.Count == GetCustomNetworkManager.Players.Count) UpdatePlayerItem();
    }

    public void FindPlayer()
    {
        LocalPlayerObject = GameObject.Find("LocalPlayer");
        PlayerController = LocalPlayerObject.GetComponent<PlayerObjectController>();
    }

    public void CreateHostPlayerItem()
    {
        print(12);
        foreach(PlayerObjectController player in GetCustomNetworkManager.Players)
        {
            GameObject NewPlayerItem = Instantiate(PlayerListItem) as GameObject;
            PlayerListItem Item = NewPlayerItem.GetComponent<PlayerListItem>();

            Item.PlayerName = player.PlayerName;
            Item.ConnectionID = player.ConnectionID;
            Item.PlayerSteamID = player.PlayerSteamID;
            Item.IsPlayerReady = player.IsPlayerReady;
            Item.SetPlayerData();

            NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
            NewPlayerItem.transform.localScale = Vector2.one;

            Items.Add(Item);
        }
        CreatedItem = true;
    }
    
    public void CreateClientPlayerItem()
    {
        print(1);
        foreach (PlayerObjectController player in GetCustomNetworkManager.Players)
        {
            if(!Items.Any(target => target.ConnectionID == player.ConnectionID))
            {
                GameObject PlayerItem = Instantiate(PlayerListItem) as GameObject;
                PlayerListItem Item = PlayerItem.GetComponent<PlayerListItem>();

                Item.PlayerName = player.PlayerName;
                Item.ConnectionID = player.ConnectionID;
                Item.PlayerSteamID = player.PlayerSteamID;
                Item.IsPlayerReady = player.IsPlayerReady;
                Item.SetPlayerData();

                PlayerItem.transform.SetParent(PlayerListViewContent.transform);
                PlayerItem.transform.localScale = Vector3.one;

                Items.Add(Item);
            }
        }
    }

    public void UpdatePlayerItem()
    {
        foreach (PlayerObjectController player in GetCustomNetworkManager.Players)
        {
            foreach(PlayerListItem target in Items)
            {
                if(target.ConnectionID == player.ConnectionID)
                {
                    target.PlayerName = player.PlayerName;
                    target.IsPlayerReady = player.IsPlayerReady;
                    target.SetPlayerData();
                    if(player == PlayerController) UpdateButton();
                }
            }
        }
        CheckIfAllReady();
    }

    public void RemovePlayerItem()
    {
        List<PlayerListItem> RemovePlayerItem = new List<PlayerListItem>();
        foreach(PlayerListItem player in Items)
        {
            if(GetCustomNetworkManager.Players.Any(target => target.ConnectionID == player.ConnectionID))
            {
                RemovePlayerItem.Add(player);
            }
        }
        if(RemovePlayerItem.Count > 0)
        {
            foreach(PlayerListItem RemovePlayerItemTarget in RemovePlayerItem)
            {
                GameObject RemoveObject = RemovePlayerItemTarget.gameObject;
                Items.Remove(RemovePlayerItemTarget);
                Destroy(RemovePlayerItemTarget);
                RemoveObject = null;
            }
        }
    }

    public void StartGame(string SceneName)
    {
        PlayerController.CanStartGame(SceneName);
    }
}
