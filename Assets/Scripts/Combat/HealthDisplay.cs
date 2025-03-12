using System;
using UnityEngine;
using UnityEngine.UI;
public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private Health health = null;
    [SerializeField] private GameObject healthBarCanvas = null;
    [SerializeField] private Image healthBarFill = null;

    [SerializeField] private float deactivateDelay = 0.8f;
    
    
    private void Awake()
    {
        health.OnHealthUpdate += HandleHealthUpdated;
    }

    private void OnDestroy()
    {
        health.OnHealthUpdate -= HandleHealthUpdated;
    }

    private void OnMouseEnter()
    {
        ActivateHealthBar();
    }

    private void ActivateHealthBar()
    {
        CancelInvoke(nameof(DeactivateHealthBarDelayed));
        healthBarCanvas.SetActive(true);
    }

    private void OnMouseExit()
    {
        DeactivateHealthBarDelayed();
    }

    private void DeactivateHealthBarDelayed()
    {
        Invoke(nameof(DeactivateHealthBar), deactivateDelay);
        
    }

    private void DeactivateHealthBar()
    {
        healthBarCanvas.SetActive(false);
    }

    private void HandleHealthUpdated(int oldHealth, int newHealth)
    {
        healthBarFill.fillAmount = (float)newHealth / health.GetMaxHealth();
    }
}
