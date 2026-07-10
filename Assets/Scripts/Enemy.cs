using UnityEngine;

public class Enemy : CellObject
{
    // Cached animator parameter hashes — cheaper than a string lookup on every call
    private static readonly int MovingHash = Animator.StringToHash("Moving");
    private static readonly int AttackHash = Animator.StringToHash("Attack");

    public int Health = 3;
    public int AttackDamage = 3;

    private int _currentHealth;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        GameManager.Instance.TurnManager.OnTick += TurnHappened;
    }

    private void Update()
    {
        // Play the walk animation only while this enemy itself is sliding
        _animator.SetBool(MovingHash, GameManager.Instance.ObjectMover.IsObjectMoving(transform));
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
        // Never step onto the player: the player isn't stored as a ContainedObject,
        // so the occupancy check below would not catch them
        if (coord == GameManager.Instance.PlayerController.Cell)
        {
            return false;
        }

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
        // Slide smoothly to the new cell (registered with the shared mover)
        GameManager.Instance.ObjectMover.AddToMove(transform, board.CellToWorldPosition(coord));

        return true;
    }

    private void TurnHappened()
    {
        // Destroy() is deferred to the end of the frame, so a just-killed enemy
        // would still receive this tick and act. Ignore it.
        if (_currentHealth <= 0)
        {
            return;
        }

        Vector2Int playerCell = GameManager.Instance.PlayerController.Cell;

        int xDist = playerCell.x - _cell.x;
        int yDist = playerCell.y - _cell.y;

        int absXDist = Mathf.Abs(xDist);
        int absYDist = Mathf.Abs(yDist);

        // Turn to face the player (keep current facing when directly above/below).
        // Enemy sprites are authored facing left, so flipX means "look right".
        if (xDist != 0)
        {
            _spriteRenderer.flipX = xDist > 0;
        }

        if ((xDist == 0 && absYDist == 1)
            || (yDist == 0 && absXDist == 1))
        {
            // Adjacent to the player: attack (reduce food).
            // Apply damage before the triggers so a broken Animator can't swallow it.
            GameManager.Instance.ChangeFood(-AttackDamage);
            _animator.SetTrigger(AttackHash);
            GameManager.Instance.PlayerController.Hit();
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
