using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    public class CellData
    {
        public bool IsPassable;
        public CellObject ContainedObject;
    }
    
    private Tilemap _tilemap;
    private CellData[,] _boardData;
    private Grid _grid;

    private List<Vector2Int> _emptyCellList;

    [SerializeField] private int _boardWidth;
    [SerializeField] private int _boardHeight;
    [SerializeField] private Tile[] _groundTiles;
    [SerializeField] private Tile[] _wallTiles;
    
    [SerializeField] private FoodObject[] _foods;
    [SerializeField] private WallObject _wallPrefab;

    [SerializeField] private PlayerController Player;

    public void Init()
    {
        // Get reference to the Tilemap component from child objects
        _tilemap = GetComponentInChildren<Tilemap>();
        // Get reference to the Grid component from child objects
        _grid = GetComponentInChildren<Grid>();
        // Initialize list to keep track of empty/passable cells
        _emptyCellList = new List<Vector2Int>();
        
        // Create 2D array to store board data for each cell
        _boardData = new CellData[_boardWidth, _boardHeight];

        // Loop through each row of the board
        for (int y = 0; y < _boardHeight; ++y)
        {
            // Loop through each column of the board
            for (int x = 0; x < _boardWidth; ++x)
            {
                // Variable to hold the tile that will be placed at this position
                Tile tile;
                // Create new CellData object for this board position
                _boardData[x, y] = new CellData();

                // Check if current position is on the board edge (border)
                if (x == 0 || y == 0 || x == _boardWidth - 1 || y == _boardHeight - 1)
                {
                    // Select a random wall tile from the wall tiles array
                    tile = _wallTiles[Random.Range(0, _wallTiles.Length)];
                    // Mark this cell as not passable (wall)
                    _boardData[x, y].IsPassable = false;
                }
                else
                {
                    // Select a random ground tile from the ground tiles array
                    tile =  _groundTiles[Random.Range(0, _groundTiles.Length)];
                    // Mark this cell as passable (ground)
                    _boardData[x, y].IsPassable = true;
                    
                    // Add this passable cell position to the empty cells list
                    _emptyCellList.Add(new Vector2Int(x, y));
                }

                // Place the selected tile at the current grid position
                _tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
        
        // Remove position (1,1) from empty cells list (likely player spawn point)
        _emptyCellList.Remove(new Vector2Int(1, 1));
        // Generate walls on random empty cells (before food so food won't spawn on them)
        GenerateWall();
        // Generate food items on random empty cells
        GenerateFood();
    }

    public Vector3 CellToWorldPosition(Vector2Int cellPosition)
    {
        return _grid.GetCellCenterWorld((Vector3Int)cellPosition);
    }
    
    public Tile GetCellTile(Vector2Int cellIndex)
    {
        return _tilemap.GetTile<Tile>(new Vector3Int(cellIndex.x, cellIndex.y, 0));
    }

    public void SetCellTile(Vector2Int cellIndex, Tile tile)
    {
        _tilemap.SetTile(new Vector3Int(cellIndex.x, cellIndex.y, 0), tile);
    }

    public CellData GetCellData(Vector2Int cellIndex)
    {
        if (cellIndex.x <  0 || cellIndex.x >= _boardWidth || cellIndex.y >= _boardHeight) return null;
        return _boardData[cellIndex.x, cellIndex.y];
    }

    private void AddObject(CellObject obj, Vector2Int coord)
    {
        CellData data = _boardData[coord.x, coord.y];
        obj.transform.position = CellToWorldPosition(coord);
        data.ContainedObject = obj;
        obj.Init(coord);
    }

    private void GenerateWall()
    {
        int wallCount = Random.Range(6, 10);
        for (int i = 0; i < wallCount; ++i)
        {
            int randomIndex = Random.Range(0, _emptyCellList.Count);
            Vector2Int cellPosition = _emptyCellList[randomIndex];
            _emptyCellList.RemoveAt(randomIndex);
            WallObject newWall = Instantiate(_wallPrefab);
            AddObject(newWall, cellPosition);
        }
    }

    private void GenerateFood()
    {
        int foodCount = Random.Range(1, 5);
        for (int i = 0; i < foodCount; ++i)
        {
            int randomIndex = Random.Range(0, _emptyCellList.Count);
            Vector2Int cellPosition = _emptyCellList[randomIndex];
            _emptyCellList.RemoveAt(randomIndex);
            int foodIndex = Random.Range(0, _foods.Length);
            FoodObject newFood = Instantiate(_foods[foodIndex]);
            AddObject(newFood, cellPosition);
        }
    }
}
