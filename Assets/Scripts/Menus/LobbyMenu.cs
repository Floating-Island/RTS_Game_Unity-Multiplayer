using System;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private Button startGameButton = null;
    [SerializeField] private List<TMP_Text> playerList = null;

    private void Start()
    {
        startGameButton.gameObject.SetActive(false);
        RTS_NetworkManager.ClientOnConnected += HandleClientConnected;
        RTS_Networked_Player.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
        RTS_Networked_Player.ClientOnInfoUpdated += ClientHandleInfoUpdated;
    }

    private void ClientHandleInfoUpdated()
    {
        List<RTS_Networked_Player> players = ((RTS_NetworkManager)NetworkManager.singleton).GetPlayers();

        int playerIndex = 0;
        while (playerIndex < players.Count)
        {
            if (playerIndex > playerList.Count) { break; }
            playerList[playerIndex].text = players[playerIndex].GetDisplayName();
            ++playerIndex;
        }
        while (playerIndex < playerList.Count)
        {
            playerList[playerIndex].text = $"Waiting For Player...";
            ++playerIndex;
        }
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool state)
    {
        startGameButton.gameObject.SetActive(state);
    }

    private void OnDestroy()
    {
        RTS_NetworkManager.ClientOnConnected -= HandleClientConnected;
        RTS_Networked_Player.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
        RTS_Networked_Player.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
    }

    private void HandleClientConnected()
    {
        lobbyUI.SetActive(true);
    }

    public void StartGame()
    {
        RTS_Networked_Player player = NetworkClient.connection.identity.GetComponent<RTS_Networked_Player>();
        player.CmdStartGame();
    }

    public void LeaveLobby()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
            SceneManager.LoadScene("Main");
        }
    }
}
