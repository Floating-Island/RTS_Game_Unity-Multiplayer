using Mirror;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
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

    [Server]
    public void ClearTarget()
    {
        if(hasAuthority)
        {
            target = null;
        }
    }

    public Targetable CurrentTarget()
    {
        return target;
    }
}
