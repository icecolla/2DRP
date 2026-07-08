using UnityEngine;

public class Enemy : CellObject
{
    public int Health = 3;
    public int AttackDamage = 3;

    private int _currentHealth;

    private void Awake()
    {
        GameManager.Instance.TurnManager.OnTick += TurnHappened;
    }

    private void OnDestroy()
    {
        // Remove the callback so a destroyed enemy isn't notified of turns
        GameManager.Instance.TurnManager.OnTick -= TurnHappened;
    }

    public override void Init(Vector2Int coord)
    {
        base.Init(coord);
        _currentHealth = Health;
    }

    public override bool PlayerWantsToEnter()
    {
        // The player bumped us: take damage and block entry (like a wall)
        _currentHealth -= 1;

        if (_currentHealth <= 0)
        {
            Destroy(gameObject);
        }

        return false;
    }

    private bool MoveTo(Vector2Int coord)
    {
        BoardManager board = GameManager.Instance.BoardManager;
        BoardManager.CellData targetCell = board.GetCellData(coord);

        // Don't move into a wall, the exit, food or another occupied/impassable cell
        if (targetCell == null
            || !targetCell.IsPassable
            || targetCell.ContainedObject != null)
        {
            return false;
        }

        // Remove enemy from its current cell
        BoardManager.CellData currentCell = board.GetCellData(_cell);
        currentCell.ContainedObject = null;

        // Add it to the target cell
        targetCell.ContainedObject = this;
        _cell = coord;
        transform.position = board.CellToWorldPosition(coord);

        return true;
    }

    private void TurnHappened()
    {
        Vector2Int playerCell = GameManager.Instance.PlayerController.Cell;

        int xDist = playerCell.x - _cell.x;
        int yDist = playerCell.y - _cell.y;

        int absXDist = Mathf.Abs(xDist);
        int absYDist = Mathf.Abs(yDist);

        if ((xDist == 0 && absYDist == 1)
            || (yDist == 0 && absXDist == 1))
        {
            // Adjacent to the player: attack (reduce food)
            GameManager.Instance.ChangeFood(-AttackDamage);
        }
        else
        {
            // Move one cell closer along the axis with the biggest distance
            if (absXDist > absYDist)
            {
                if (!TryMoveInX(xDist))
                {
                    TryMoveInY(yDist);
                }
            }
            else
            {
                if (!TryMoveInY(yDist))
                {
                    TryMoveInX(xDist);
                }
            }
        }
    }

    private bool TryMoveInX(int xDist)
    {
        // Player to our right
        if (xDist > 0)
        {
            return MoveTo(_cell + Vector2Int.right);
        }

        // Player to our left
        return MoveTo(_cell + Vector2Int.left);
    }

    private bool TryMoveInY(int yDist)
    {
        // Player above
        if (yDist > 0)
        {
            return MoveTo(_cell + Vector2Int.up);
        }

        // Player below
        return MoveTo(_cell + Vector2Int.down);
    }
}
