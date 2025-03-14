using System;
using Mirror;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    
    [SyncVar(hook = nameof(HealthUpdated))]
    private int currentHealth;

    private void  HealthUpdated(int oldHealth, int newHealth)
    {
        OnHealthUpdate?.Invoke(oldHealth, newHealth);
    }

    public event Action ServerOnDie;

    public event Action<int, int> OnHealthUpdate;

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
        GameOverHandler.ServerPlayerDied += ServerHandlePlayerDied;
    }
    
    private void ServerHandlePlayerDied(int connectionId)
    {
        if (connectionId != connectionToClient.connectionId) { return; }

        ReceiveDamage(currentHealth);
    }

    public override void OnStopServer()
    {
        GameOverHandler.ServerPlayerDied -= ServerHandlePlayerDied;
    }

    [Server]
    public void ReceiveDamage(int damageAmount)
    {
        if (currentHealth == 0)
        {
            return;
        }
        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (currentHealth == 0)
        {
            Debug.Log(name + "dead");
            ServerOnDie?.Invoke();
        }
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
