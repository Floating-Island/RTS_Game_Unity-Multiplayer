using System;
using TMPro;
using UnityEngine;

public class ResourceDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text resourcesText = null;
    private RTS_Networked_Player player;

    // Update is called once per frame
    void Update()
    {
        StoreNetworkedPlayer();
    }

    private void OnDestroy()
    {
        ResourceStorage resourceStorage = player.GetResourceStorage();
        resourceStorage.ClientHandleResourcesUpdated -= UpdateResourceText;
    }

    private void StoreNetworkedPlayer()
    {
        if (player == null)
        {
            player = RTS_Networked_Player.NetworkedPlayer();

            if (player == null) { return; }

            ResourceStorage resourceStorage = player.GetResourceStorage();
            resourceStorage.ClientHandleResourcesUpdated += UpdateResourceText;
            UpdateResourceText(resourceStorage.CurrentResourceAmount());
        }

    }

    private void UpdateResourceText(int updatedResource)
    {
        resourcesText.text = "Resources: " + updatedResource.ToString();
    }
}
