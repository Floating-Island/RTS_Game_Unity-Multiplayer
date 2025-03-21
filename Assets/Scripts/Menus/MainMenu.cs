using Mirror;
using UnityEngine;
using UnityEngine.UIElements;
using Steamworks;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel = null;

    [SerializeField] private bool useSteam = true;


    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private void Start()
    {
        SetupSteamCallbacks();

    }

    private void SetupSteamCallbacks()
    {
        if (!useSteam) { return; }

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
    }

    public void HostLobby()
    {
        landingPagePanel.SetActive(false);

        if (useSteam)
        {
            int maxMembers = 4;
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, maxMembers);
        }
        else
        {
            NetworkManager.singleton.StartHost();
        }
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            landingPagePanel.SetActive(true);
            return;
        }

        NetworkManager.singleton.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostAddress", SteamUser.GetSteamID().ToString());
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEnter(LobbyEnter_t callback)
    {
        if (NetworkServer.active) { return; }
        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostAddress");
        NetworkManager.singleton.networkAddress = hostAddress;
        NetworkManager.singleton.StartClient();
        landingPagePanel.SetActive(false);
    }
}
