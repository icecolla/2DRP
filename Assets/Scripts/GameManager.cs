using UnityEngine;
using UnityEngine.UIElements;

public class GameManager :  MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private BoardManager _boardManager;
    [SerializeField] private PlayerController _playerController;

    public BoardManager BoardManager => _boardManager;
    
    public TurnManager TurnManager { get; private set; }
    
    private int _foodAmount = 100;
    private int _currentLevel = 1;

    public UIDocument UIDocument;
    private Label _foodLabel;
    private VisualElement _gameOverPanel;
    private Label _gameOverMessage;
    
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
        // Things that only need to happen once when the app launches
        TurnManager = new TurnManager();
        TurnManager.OnTick += OnTurnHappen;

        _foodLabel = UIDocument.rootVisualElement.Q<Label>("FoodLabel");
        _gameOverPanel = UIDocument.rootVisualElement.Q<VisualElement>("GameOverPanel");
        _gameOverMessage = _gameOverPanel.Q<Label>("GameOverMessage");

        StartNewGame();
    }

    public void StartNewGame()
    {
        _gameOverPanel.style.visibility = Visibility.Hidden;

        _currentLevel = 1;
        _foodAmount = 20;
        _foodLabel.text = "Food: " + _foodAmount;

        _boardManager.Clean();
        _boardManager.Init();

        _playerController.Init();
        _playerController.Spawn(_boardManager, new Vector2Int(1, 1));
    }

    public void NewLevel()
    {
        _boardManager.Clean();
        _boardManager.Init();
        _playerController.Spawn(_boardManager, new Vector2Int(1, 1));

        _currentLevel++;
    }

    private void OnTurnHappen()
    {
        ChangeFood(-1);
    }

    public void ChangeFood(int amount)
    {
        _foodAmount += amount;
        _foodLabel.text = "Food: " + _foodAmount;

        if (_foodAmount <= 0)
        {
            _playerController.GameOver();
            _gameOverPanel.style.visibility = Visibility.Visible;
            _gameOverMessage.text = "Game Over!\n\nYou traveled through " + _currentLevel +
                                    " levels\n\nPress Enter to play again";
        }
    }
}