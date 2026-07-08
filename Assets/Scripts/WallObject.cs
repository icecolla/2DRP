using UnityEngine;
using UnityEngine.Tilemaps;

public class WallObject : CellObject
{
    public Tile ObstacleTile;
    public Tile DamagedObstacleTile;
    public int MaxHealth = 3;

    private int _healthPoint;
    private Tile _originalTile;

    public override void Init(Vector2Int cell)
    {
        base.Init(cell);

        _healthPoint = MaxHealth;

        // Save the tile currently under the wall so we can restore it once destroyed.
        _originalTile = GameManager.Instance.BoardManager.GetCellTile(cell);
        GameManager.Instance.BoardManager.SetCellTile(cell, ObstacleTile);
    }

    public override bool PlayerWantsToEnter()
    {
        _healthPoint -= 1;

        if (_healthPoint > 0)
        {
            // Show the damaged tile when the wall is nearly destroyed (1 HP left).
            if (_healthPoint == 1 && DamagedObstacleTile != null)
            {
                GameManager.Instance.BoardManager.SetCellTile(_cell, DamagedObstacleTile);
            }
            // Still standing: block the player.
            return false;
        }

        // Destroyed: restore the original tile and let the player in.
        GameManager.Instance.BoardManager.SetCellTile(_cell, _originalTile);
        Destroy(gameObject);
        return true;
    }
}
