using System;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitQueuer : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Health health = null;

    [SerializeField]
    private UnitSpawner unitSpawner = null;

    [SerializeField]
    private TMP_Text queuedUnitsText = null;

    [SerializeField]
    private Image unitProgressImage = null;

    [SerializeField]
    private int maxUnitQueue = 5;

    [SerializeField]
    private float unitPreparationDelaySeconds = 7f;

    // to store the smooth velocity each time it's used.
    private float progressImageVelocity;

    [SyncVar (hook = nameof(ClientHandleQueuedUnitsUpdated))]
    private int queuedUnits = 0;

    [SyncVar]
    private float unitTimer = 0f;

    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
    }

    [Server]
    private void ServerHandleDie()
    {
        enabled = false;
    }

    private void Update()
    {
        if(isServer)
        {
            ProduceUnits();
        }
        if (isClient)
        {
            UpdateTimerDisplay();
        }
    }

    private void UpdateTimerDisplay()
    {
        float newProgress = unitTimer / unitPreparationDelaySeconds;

        if (newProgress < unitProgressImage.fillAmount)
        {
            unitProgressImage.fillAmount = newProgress;
        }
        else
        {
            float smoothingMaxSpeed = 0.1f;
            unitProgressImage.fillAmount = Mathf.SmoothDamp(unitProgressImage.fillAmount, newProgress, ref progressImageVelocity, smoothingMaxSpeed);
        }
    }

    [Server]
    private void ProduceUnits()
    {
        if (queuedUnits == 0) { return; }

        unitTimer += Time.deltaTime;

        if (unitTimer < unitPreparationDelaySeconds) { return; }

        unitSpawner.SpawnUnit();

        unitTimer = 0f;
        queuedUnits--;
    }

    private void ClientHandleQueuedUnitsUpdated(int oldQueueAmount, int newQueueAmount)
    {
        queuedUnitsText.text = newQueueAmount.ToString();
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if(netIdentity.isOwned && eventData.button == PointerEventData.InputButton.Right)
        {
            AddUnitToQueue();
        }
    }

    [Command]
    void AddUnitToQueue()
    {
        if (queuedUnits >= maxUnitQueue) { return; }
        
        if (unitSpawner.BuyUnit())
        {
            queuedUnits++;
        }
    }

}
