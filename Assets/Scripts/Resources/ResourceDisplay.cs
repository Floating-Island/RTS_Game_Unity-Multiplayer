using System;
using TMPro;
using UnityEngine;

public class ResourceDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text resourcesText = null;
    private RTS_Networked_Player player;

    private void Start()
    {
        StoreNetworkedPlayer();
        SetupResources();
    }

    private void OnDestroy()
    {
        ResourceStorage resourceStorage = player.GetResourceStorage();
        resourceStorage.ClientHandleResourcesUpdated -= UpdateResourceText;
    }

    private void StoreNetworkedPlayer()
    {
        player = RTS_Networked_Player.ClientNetworkedPlayer();
    }

    private void SetupResources()
    {
        ResourceStorage resourceStorage = player.GetResourceStorage();
        resourceStorage.ClientHandleResourcesUpdated += UpdateResourceText;
        UpdateResourceText(resourceStorage.CurrentResourceAmount());
    }

    private void UpdateResourceText(int updatedResource)
    {
        resourcesText.text = "Resources: " + updatedResource.ToString();
    }
}
