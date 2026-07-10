using System.Collections.Generic;
using UnityEngine;

// Centralized smooth-movement system. Objects register a transform + target world
// position; ObjectMover slides them all at the same speed and reports IsMoving so
// input can be blocked until every movement finishes.
public class ObjectMover
{
    private class MovingObject
    {
        public Transform Transform;
        public Vector3 Target;
    }

    private readonly List<MovingObject> _moving = new List<MovingObject>();
    private const float Speed = 5f;

    public bool IsMoving => _moving.Count > 0;

    // True while this specific transform still has a pending move
    public bool IsObjectMoving(Transform transform)
    {
        for (int i = 0; i < _moving.Count; ++i)
        {
            if (_moving[i].Transform == transform)
                return true;
        }
        return false;
    }

    public void AddToMove(Transform transform, Vector3 target)
    {
        _moving.Add(new MovingObject { Transform = transform, Target = target });
    }

    // Drop all pending moves (e.g. when a new level is generated)
    public void Clear()
    {
        _moving.Clear();
    }

    // Call every frame from GameManager.Update
    public void Tick()
    {
        for (int i = _moving.Count - 1; i >= 0; --i)
        {
            MovingObject m = _moving[i];

            // A registered object can be destroyed mid-move (e.g. an enemy dying)
            if (m.Transform == null)
            {
                _moving.RemoveAt(i);
                continue;
            }

            m.Transform.position = Vector3.MoveTowards(m.Transform.position, m.Target, Speed * Time.deltaTime);

            if (m.Transform.position == m.Target)
            {
                _moving.RemoveAt(i);
            }
        }
    }
}
