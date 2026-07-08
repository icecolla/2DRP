using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private BoardManager _boardManager;
    private Vector2Int _cellPosition;
    private bool _isGameOver;

    public Vector2Int Cell => _cellPosition;

    public void Init()
    {
        _isGameOver = false;
    }

    public void GameOver()
    {
        _isGameOver = true;
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
                GameManager.Instance.TurnManager.Tick();
                CellObject containedObject = cellData.ContainedObject;
                if (containedObject == null)
                {
                    MoveTo(newCellTarget);
                }
                else if (containedObject.PlayerWantsToEnter())
                {
                    MoveTo(newCellTarget);
                    // Call PlayerEntered AFTER moving the player! Otherwise not in cell yet
                    containedObject.PlayerEntered();
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
