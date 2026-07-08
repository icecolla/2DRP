using UnityEngine;

public class CellObject : MonoBehaviour
{
    protected Vector2Int _cell;

    public virtual void Init(Vector2Int cell)
    {
        _cell = cell;
    }

    public virtual void PlayerEntered()
    {

    }

    public virtual bool PlayerWantsToEnter()
    {
        return true;
    }
}
