using UnityEngine;
using UnityEngine.Tilemaps;

public class WallObject : CellObject
{
    public Tile ObstacleTile;
    public int MaxHealth = 3;

    private int _healthPoint;

    public override void Init(Vector2Int cell)
    {
        base.Init(cell);
        _healthPoint = MaxHealth;
        GameManager.Instance.BoardManager.SetCellTile(cell, ObstacleTile);
        // Cell stays passable so the player can bump the wall; entry is gated by PlayerWantsToEnter.
    }

    public override bool PlayerWantsToEnter()
    {
        // Each bump damages the wall.
        _healthPoint -= 1;
        Debug.Log("Wall health: " + _healthPoint);

        if (_healthPoint > 0)
        {
            // Still standing: block the player.
            return false;
        }

        // Destroyed: restore the ground tile, free the cell and remove this object.
        BoardManager board = GameManager.Instance.BoardManager;
        board.SetCellTile(_cell, board.GetRandomGroundTile());
        BoardManager.CellData data = board.GetCellData(_cell);
        data.ContainedObject = null;
        Destroy(gameObject);
        return true;
    }
}
