using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private BoardManager _boardManager;
    private Vector2Int _cellPosition;

    private void Update()
    {
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
                _cellPosition = newCellTarget;
                transform.position = _boardManager.CellToWorldPosition(_cellPosition);
            }
        }
    }

    public void Spawn(BoardManager boardManager, Vector2Int cellPosition)
    {
        _boardManager = boardManager;
        _cellPosition = cellPosition;
        
        transform.position = _boardManager.CellToWorldPosition(cellPosition);
    }
}
