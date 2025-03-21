using Mirror;
using UnityEngine;

public class Targetable : NetworkBehaviour
{
    [SerializeField] 
    private Transform aimAtPoint = null;

    public bool CanBeTargeted()
    {
        // If the object is not ours, it can be targeted
        return !netIdentity.isOwned;
    }

    public Transform GetAimAtPoint()
    {
        return aimAtPoint;
    }
}
