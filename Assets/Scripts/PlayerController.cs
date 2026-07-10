using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Cached animator parameter hashes — cheaper than a string lookup on every call
    private static readonly int MovingHash = Animator.StringToHash("Moving");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int HitHash = Animator.StringToHash("Hit");

    private BoardManager _boardManager;
    private Vector2Int _cellPosition;
    private bool _isGameOver;
    private Animator _animator;

    public Vector2Int Cell => _cellPosition;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Init()
    {
        _isGameOver = false;
    }

    public void GameOver()
    {
        _isGameOver = true;
    }

    public void Attack()
    {
        _animator.SetTrigger(AttackHash);
    }

    // Called by an enemy when it damages the player
    public void Hit()
    {
        _animator.SetTrigger(HitHash);
    }

    private void Update()
    {
        if (_isGameOver)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                GameManager.Instance.StartNewGame();
            }
            return;
        }

        // Play the walk animation only while the player itself is sliding
        _animator.SetBool(MovingHash, GameManager.Instance.ObjectMover.IsObjectMoving(transform));

        // Wait until every object has finished sliding before accepting a new move
        if (GameManager.Instance.ObjectMover.IsMoving)
        {
            return;
        }

        Vector2Int newCellTarget = _cellPosition;
        bool hasMoved = false;

        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            newCellTarget.y += 1;
            hasMoved = true;
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            newCellTarget.y -= 1;
            hasMoved = true;
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            newCellTarget.x -= 1;
            hasMoved = true;
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            newCellTarget.x += 1;
            hasMoved = true;
        }

        if (hasMoved)
        {
            BoardManager.CellData cellData = _boardManager.GetCellData(newCellTarget);

            if (cellData != null && cellData.IsPassable)
            {
                CellObject containedObject = cellData.ContainedObject;
                if (containedObject == null)
                {
                    MoveTo(newCellTarget);
                    // Tick AFTER the player's cell is updated, so enemies react to the new position
                    GameManager.Instance.TurnManager.Tick();
                }
                else if (containedObject.PlayerWantsToEnter())
                {
                    MoveTo(newCellTarget);
                    GameManager.Instance.TurnManager.Tick();
                    // Call PlayerEntered AFTER moving the player! Otherwise not in cell yet
                    containedObject.PlayerEntered();
                }
                else
                {
                    // Blocked by a wall or an enemy: the player attacks it instead
                    Attack();
                    GameManager.Instance.TurnManager.Tick();
                }
            }
        }
    }

    public void Spawn(BoardManager boardManager, Vector2Int cellPosition)
    {
        _boardManager = boardManager;
        // Snap instantly on spawn — no sliding from the old position
        MoveTo(cellPosition, true);
    }

    public void MoveTo(Vector2Int target, bool immediate = false)
    {
        _cellPosition = target;

        if (immediate)
        {
            transform.position = _boardManager.CellToWorldPosition(target);
        }
        else
        {
            GameManager.Instance.ObjectMover.AddToMove(transform, _boardManager.CellToWorldPosition(target));
        }
    }
}
