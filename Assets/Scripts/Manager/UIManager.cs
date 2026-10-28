using UnityEngine;
using TMPro; 

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Hiển thị Điểm trên màn hình chính (HUD)")]
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("Bảng thông báo Thua cuộc (Game Over Popup)")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI bestScorePopupText;

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
    }

    private void Start()
    { 
        HideGameOverPanel();
    }

    public void UpdateScoreDisplay(int currentScore, int highScore)
    {
        if (currentScoreText != null)
        {
            currentScoreText.text = currentScore.ToString();
        }

        if (highScoreText != null)
        {
            highScoreText.text = highScore.ToString();
        }
    }

    public void ShowGameOverPanel(int finalScore, int highScore)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = finalScore.ToString();
        }

        if (bestScorePopupText != null)
        {
            bestScorePopupText.text = highScore.ToString();
        }
    }

    public void HideGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }
}