using UnityEngine;

public abstract class PossessedObject : MonoBehaviour
{
    // Basic class for every possessed Object

    protected bool isPossessed = false;

    public virtual void OnPossessionStart()
    {
        isPossessed = true;
    }

    public virtual void OnPossessionEnd()
    {
        isPossessed = false;
    }
}
