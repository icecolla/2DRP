using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    public class CellData
    {
        public bool Passable;
    }
    
    private Tilemap _tilemap;
    private CellData[,] _boardData;

    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private Tile[] _groundTiles;
    [SerializeField] private Tile[] _wallTiles;

    void Start()
    {
        _tilemap = GetComponentInChildren<Tilemap>();
        _boardData = new CellData[_width, _height];

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                Tile tile;
                _boardData[x, y] = new CellData();

                if (x == 0 || y == 0 || x == _width - 1 || y == _height - 1)
                {
                    tile = _wallTiles[Random.Range(0, _wallTiles.Length)];
                    _boardData[x, y].Passable = false;
                }
                else
                {
                    tile =  _groundTiles[Random.Range(0, _groundTiles.Length)];
                    _boardData[x, y].Passable = true;
                }

                _tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }
}
