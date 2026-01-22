using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    private Tilemap _tilemap;

    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private Tile[] _groundTiles;

    void Start()
    {
        _tilemap = GetComponentInChildren<Tilemap>();

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                int tileNumber = Random.Range(0, _groundTiles.Length);
                _tilemap.SetTile(new Vector3Int(x, y, 0), _groundTiles[tileNumber]);
            }
        }
    }
}
