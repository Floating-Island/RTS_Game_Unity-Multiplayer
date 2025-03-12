using System;
using Mirror;
using UnityEngine;

public class UnitFiring : NetworkBehaviour
{
    [SerializeField]
    private Targeter targeter = null;

    [SerializeField]
    private GameObject projectilePrefab = null;
    
    [SerializeField]
    private Transform projectileSpawnPoint = null;

    [SerializeField]
    private float fireRange = 5f;
    [SerializeField]
    private float fireRate = 1f;

    [SerializeField] private float rotationSpeed = 20f;

    private float lastFireTime;

    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.CurrentTarget();
        if (target == null) { return; }

        if (!CanFireAtTarget()) { return; }

        RotateTowardsTarget(target);
        TryFireProjectile(target);
    }

    private void RotateTowardsTarget(Targetable target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void TryFireProjectile(Targetable target)
    {
        float currentTime = Time.time;

        if (currentTime > (1 / fireRate) + lastFireTime)
        {
            FireProjectile(target);
            lastFireTime = currentTime;
        }
    }

    [Server]
    private void FireProjectile(Targetable target)
    {
        Quaternion projectileRotation = Quaternion.LookRotation(target.GetAimAtPoint().position - projectileSpawnPoint.position);

        GameObject projectileInstance = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation);
        NetworkServer.Spawn(projectileInstance, connectionToClient);
    }

    [Server]
    private bool CanFireAtTarget()
    {
        return (targeter.CurrentTarget().transform.position - transform.position).sqrMagnitude <= fireRange * fireRange;
    }
}

