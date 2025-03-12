using Mirror;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rigidBody = null;
    [SerializeField] private float destroyAfterSeconds = 5f;
    [SerializeField] private float launchForce = 10f;
    
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
        DestroySelf();
    }

    [Server]
    private void DestroySelf()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }
}
