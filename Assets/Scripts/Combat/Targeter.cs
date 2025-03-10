using Mirror;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    private Targetable target;

    [Command]
    public void CmdSetTarget(GameObject target)
    {
        if(!hasAuthority) {return;}
        
        if (!target.TryGetComponent<Targetable>(out Targetable targetable))
        { 
            return;
        }

        this.target = targetable;
    }

    [Command]
    public void CmdClearTarget()
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
