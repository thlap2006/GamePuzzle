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

    [Header("Cấu hình Điểm số & Combo")]
    [SerializeField] private int pointsPerBlock = 1;      
    [SerializeField] private int basePointsPerLine = 20;  

    [Tooltip("Số nước đi tối đa không ăn hàng mà vẫn giữ được chuỗi Combo (Để 2 giống Block Blast, để 0 nếu bắt ăn liên tiếp mỗi lượt)")]
    [SerializeField] private int graceMovesAllowed = 2;

    [Header("Tham chiếu hệ thống")]
    [SerializeField] private PieceSpawner pieceSpawner;

    private int currentScore = 0;
    private int highScore = 0;

    private int currentCombo = 0;
    private int movesWithoutClear = 0;

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
        currentCombo = 0;
        movesWithoutClear = 0;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScoreDisplay(currentScore, highScore);
            UIManager.Instance.HideComboPopupImmediate();
        }
    }

    public void AddScore(int blocksPlacedCount, int linesClearedCount)
    {
        int placementPoints = blocksPlacedCount * pointsPerBlock;
        int lineBonusPoints = 0;

        if (linesClearedCount > 0)
        {
            movesWithoutClear = 0;

            currentCombo += linesClearedCount;

            lineBonusPoints = (linesClearedCount * basePointsPerLine) * currentCombo;

            AudioManager.Instance?.PlayCombo(currentCombo);

            if (currentCombo >= 2 && UIManager.Instance != null)
            {
                UIManager.Instance.ShowComboPopup(currentCombo, lineBonusPoints);
            }
        }
        else
        {
            movesWithoutClear++;

            if (movesWithoutClear > graceMovesAllowed)
            {
                currentCombo = 0;
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.HideComboPopupImmediate();
                }
            }
        }

        currentScore += (placementPoints + lineBonusPoints);

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

        AudioManager.Instance?.PlayGameOver();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOverPanel(currentScore, highScore);
        }
    }

    public void RestartGame()
    {
        AudioManager.Instance?.PlayRetry();

        CurrentState = GameState.Playing;
        currentScore = 0;
        currentCombo = 0;
        movesWithoutClear = 0;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideGameOverPanel();
            UIManager.Instance.HideComboPopupImmediate();
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