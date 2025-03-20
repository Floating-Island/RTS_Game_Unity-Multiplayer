using System;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private Button startGameButton = null;

    private void Start()
    {
        startGameButton.gameObject.SetActive(false);
        RTS_NetworkManager.ClientOnConnected += HandleClientConnected;
        RTS_Networked_Player.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool state)
    {
        startGameButton.gameObject.SetActive(state);
    }

    private void OnDestroy()
    {
        RTS_NetworkManager.ClientOnConnected -= HandleClientConnected;
        RTS_Networked_Player.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
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
