using UnityEngine;
using UnityEngine.UIElements;

public class GameManager :  MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private BoardManager _boardManager;
    [SerializeField] private PlayerController _playerController;
    
    public TurnManager TurnManager { get; private set; }
    
    private int _foodAmount = 100;

    public UIDocument UIDocument;
    private Label _foodLabel;
    
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
        _foodLabel = UIDocument.rootVisualElement.Q<Label>("FoodLabel");
        _foodLabel.text = "Food: " + _foodAmount;
        
        TurnManager = new TurnManager();
        TurnManager.OnTick += OnTurnHappen;

        
        _boardManager.Init();
        _playerController.Spawn(_boardManager, new Vector2Int(1, 1));
    }

    private void OnTurnHappen()
    {
        ChangeFood(-1);
    }

    public void ChangeFood(int amount)
    {
        _foodAmount += amount;
        _foodLabel.text = "Food: " + _foodAmount;
        Debug.Log("Current amount of food : " + _foodAmount);
    }
}