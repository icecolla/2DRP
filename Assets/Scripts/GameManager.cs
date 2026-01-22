using System;
using UnityEngine;

public class GameManager :  MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private BoardManager _boardManager;
    [SerializeField] private PlayerController _playerController;
    
    public TurnManager TurnManager { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        TurnManager = new TurnManager();
        
        _boardManager.Init();
        _playerController.Spawn(_boardManager, new Vector2Int(1, 1));
    }
}