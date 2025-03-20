using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobby : MonoBehaviour
{
    [SerializeField] private GameObject landingPanel = null;
    [SerializeField] private TMP_InputField addressInput = null;
    [SerializeField] private Button joinButton = null;
    [SerializeField] private Button quitButton = null;

    private void OnEnable()
    {
        RTS_NetworkManager.ClientOnConnected += HandleCientConnected;
        RTS_NetworkManager.ClientOnDisconnected += HandleCientDisconnected;    
    }

    private void OnDisable()
    {
        RTS_NetworkManager.ClientOnConnected -= HandleCientConnected;
        RTS_NetworkManager.ClientOnDisconnected -= HandleCientDisconnected;    
    }

    public void Join()
    {
        string address = addressInput.text;

        NetworkManager.singleton.networkAddress = address;
        NetworkManager.singleton.StartClient();

        joinButton.interactable = false;
        quitButton.interactable = false;
    }

    private void HandleCientConnected()
    {
        joinButton.interactable = true;
        gameObject.SetActive(false);
        landingPanel.SetActive(false);
    }

    private void HandleCientDisconnected()
    {
        joinButton.interactable = true;
        quitButton.interactable = true;
    }
}
