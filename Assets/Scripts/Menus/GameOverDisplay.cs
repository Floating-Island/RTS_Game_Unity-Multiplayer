using System;
using Mirror;
using TMPro;
using UnityEngine;

public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] private GameObject gameOverDisplayParent = null;
    [SerializeField] private TMP_Text winnerText = null;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameOverHandler.ClientOnGameOver += HandleClientOnGameOver;
    }

    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= HandleClientOnGameOver;
    }

    private void HandleClientOnGameOver(int winnerId)
    {
        gameOverDisplayParent.SetActive(true);
        winnerText.text = $"Player {winnerId} Wins!";
    }

    public void LeaveGame()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }
}
