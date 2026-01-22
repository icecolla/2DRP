using UnityEngine;

public class GameManager :  MonoBehaviour
{
    [SerializeField] private BoardManager _boardManager;
    [SerializeField] private PlayerController _playerController;
    
    private TurnManager _turnManager;

    private void Start()
    {
        _turnManager= new TurnManager();
        
        _boardManager.Init();
        _playerController.Spawn(_boardManager, new Vector2Int(1, 1));
    }
}