using UnityEngine;
public enum GameState
{
    Playing,
    GameOver
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    [Header("Cấu hình Điểm số")]
    [SerializeField] private int pointsPerBlock = 1;      
    [SerializeField] private int basePointsPerLine = 10;  

    [Header("Tham chiếu hệ thống")]
    [SerializeField] private PieceSpawner pieceSpawner;

    private int currentScore = 0;
    private int highScore = 0;

    private const string HighScoreKey = "PUZZLE_HIGH_SCORE";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    private void Start()
    {
        CurrentState = GameState.Playing;
        currentScore = 0;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScoreDisplay(currentScore, highScore);
        }
    }

    public void AddScore(int blocksPlacedCount, int linesClearedCount)
    {
        int placementPoints = blocksPlacedCount * pointsPerBlock;

        int comboMultiplier = (linesClearedCount * (linesClearedCount + 1)) / 2;
        int linePoints = comboMultiplier * basePointsPerLine;

        currentScore += (placementPoints + linePoints);

        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();                          
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScoreDisplay(currentScore, highScore);
        }
    }

    public void TriggerGameOver()
    {
        if (CurrentState == GameState.GameOver) return;

        CurrentState = GameState.GameOver;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOverPanel(currentScore, highScore);
        }
    }

    public void RestartGame()
    {
        CurrentState = GameState.Playing;
        currentScore = 0;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideGameOverPanel();
            UIManager.Instance.UpdateScoreDisplay(currentScore, highScore);
        }

        if (GridManager.Instance != null)
        {
            GridManager.Instance.ClearAllCells();
        }

        if (pieceSpawner != null)
        {
            pieceSpawner.SpawnNewBatch();
        }
    }
}