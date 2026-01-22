using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private BoardManager _boardManager;
    private Vector2Int _cellPosition;

    public void Spawn(BoardManager board, Vector2Int cell)
    {
        _boardManager = board;
        _cellPosition = cell;
        
        transform.position = _boardManager.CellToWorldPosition(cell);
    }
}
