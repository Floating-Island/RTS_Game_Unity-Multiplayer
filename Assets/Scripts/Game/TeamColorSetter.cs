using System;
using Mirror;
using UnityEngine;

public class TeamColorSetter : NetworkBehaviour
{
    [SerializeField] private Renderer[] colorRenderers = new Renderer[0];

    [SyncVar(hook = nameof(HandleTeamColorUpdate))]
    private Color teamColor = new Color();

    private void HandleTeamColorUpdate(Color oldColor, Color newColor)
    {
        foreach (Renderer renderer in colorRenderers)
        {
            renderer.material.SetColor("_BaseColor", newColor);
        }
    }

    public override void OnStartServer()
    {
        RTS_Networked_Player player = RTS_Networked_Player.ServerNetworkedPlayer(this);
        teamColor = player.GetTeamColor();
    }
}
