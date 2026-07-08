using UnityEngine;
using UnityEngine.Tilemaps;

public class WallObject : CellObject
{
    public Tile ObstacleTile;

    public override void Init(Vector2Int cell)
    {
        base.Init(cell);
        GameManager.Instance.BoardManager.SetCellTile(cell, ObstacleTile);

        // Make the cell impassable so the player can't walk on the wall
        BoardManager.CellData data = GameManager.Instance.BoardManager.GetCellData(cell);
        data.IsPassable = false;
    }
}
