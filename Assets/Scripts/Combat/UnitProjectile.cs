using Mirror;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rigidBody = null;
    [SerializeField] private float destroyAfterSeconds = 5f;
    [SerializeField] private float launchForce = 10f;
    [SerializeField] private int projectileDamage = 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetInitialVelocity();
    }

    private void SetInitialVelocity()
    {
        rigidBody.linearVelocity = transform.forward * launchForce;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Invoke(nameof(ServerDestroy), destroyAfterSeconds);
    }

    [Server]
    private void ServerDestroy()
    {
        NetworkServer.Destroy(gameObject);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        //what happens if it's a building/terrain?
        if (HasSameConnection(other)) { return; }

        DealDamage(other);

        CancelInvoke(nameof(ServerDestroy));
        ServerDestroy();
    }

    private bool HasSameConnection(Collider other)
    {
        bool sameConnection = false;
        if (other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
        {
            if (networkIdentity.connectionToClient == connectionToClient)
            { 
                sameConnection = true;
            }
        }
        return sameConnection;
    }

    private void DealDamage(Collider other)
    {
        if (other.TryGetComponent<Health>(out Health health))
        {
            health.ReceiveDamage(projectileDamage);
        }
    }
}
