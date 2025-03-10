using Mirror;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    [SerializeField] 
    private Targetable target;

    [Command]
    public void CmdSetTarget(GameObject target)
    {
        if (!target.TryGetComponent<Targetable>(out Targetable targetable))
        { 
            return;
        }

        this.target = targetable;
    }

    [Command]
    public void CmdClearTarget()
    {
        target = null;
    }
}
