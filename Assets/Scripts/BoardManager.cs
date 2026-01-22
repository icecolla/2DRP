using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    public class CellData
    {
        public bool IsPassable;
    }
    
    private Tilemap _tilemap;
    private CellData[,] _boardData;
    private Grid _grid;

    [SerializeField] private int _boardWidth;
    [SerializeField] private int _boardHeight;
    [SerializeField] private Tile[] _groundTiles;
    [SerializeField] private Tile[] _wallTiles;
    
    [SerializeField] private PlayerController Player;

    public void Init()
    {
        _tilemap = GetComponentInChildren<Tilemap>();
        _grid = GetComponentInChildren<Grid>();
        
        _boardData = new CellData[_boardWidth, _boardHeight];

        for (int y = 0; y < _boardHeight; ++y)
        {
            for (int x = 0; x < _boardWidth; ++x)
            {
                Tile tile;
                _boardData[x, y] = new CellData();

                if (x == 0 || y == 0 || x == _boardWidth - 1 || y == _boardHeight - 1)
                {
                    tile = _wallTiles[Random.Range(0, _wallTiles.Length)];
                    _boardData[x, y].IsPassable = false;
                }
                else
                {
                    tile =  _groundTiles[Random.Range(0, _groundTiles.Length)];
                    _boardData[x, y].IsPassable = true;
                }

                _tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
        
        Player.Spawn(this, new Vector2Int(1, 1));
    }

    public Vector3 CellToWorldPosition(Vector2Int cellPosition)
    {
        return _grid.GetCellCenterWorld((Vector3Int)cellPosition);
    }
    
    public CellData GetCellData(Vector2Int cellIndex)
    {
        if (cellIndex.x <  0 || cellIndex.x >= _boardWidth || cellIndex.y >= _boardHeight) return null;
        return _boardData[cellIndex.x, cellIndex.y];
    }
}
