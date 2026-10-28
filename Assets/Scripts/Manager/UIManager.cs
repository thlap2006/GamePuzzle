using System.Collections;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Hiển thị Điểm trên màn hình chính (HUD)")]
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("Hiển thị Chữ Nảy COMBO (2x, 3x, 4x...)")]
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private float comboDisplayDuration = 1.4f; 

    [Header("Bảng thông báo Thua cuộc (Game Over Popup)")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI bestScorePopupText;

    private Coroutine comboAnimCoroutine;

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
        HideComboPopupImmediate();
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

    public void ShowComboPopup(int comboCount, int bonusScore)
    {
        if (comboText == null) return;

        comboText.text = $"COMBO x{comboCount}!\n<size=65%>+{bonusScore}</size>";

        comboText.color = GetComboTierColor(comboCount);

        comboText.gameObject.SetActive(true);
        if (comboAnimCoroutine != null)
        {
            StopCoroutine(comboAnimCoroutine);
        }
        comboAnimCoroutine = StartCoroutine(ComboPopupRoutine());
    }

    public void HideComboPopupImmediate()
    {
        if (comboText != null)
        {
            comboText.gameObject.SetActive(false);
        }
    }

    private Color GetComboTierColor(int combo)
    {
        if (combo == 2) return new Color(1f, 0.90f, 0.15f); 
        if (combo == 3) return new Color(1f, 0.55f, 0.10f); 
        if (combo == 4) return new Color(1f, 0.20f, 0.65f); 
        if (combo == 5) return new Color(0.15f, 0.95f, 1f); 
        return new Color(1f, 0.20f, 0.25f);                 
    }


    private IEnumerator ComboPopupRoutine()
    {
        Transform textTransform = comboText.transform;

        float timer = 0f;
        float popUpTime = 0.15f;
        while (timer < popUpTime)
        {
            timer += Time.deltaTime;
            float scale = Mathf.Lerp(0.2f, 1.35f, timer / popUpTime);
            textTransform.localScale = Vector3.one * scale;
            yield return null;
        }

        timer = 0f;
        float settleTime = 0.1f;
        while (timer < settleTime)
        {
            timer += Time.deltaTime;
            float scale = Mathf.Lerp(1.35f, 1.0f, timer / settleTime);
            textTransform.localScale = Vector3.one * scale;
            yield return null;
        }

        yield return new WaitForSeconds(comboDisplayDuration);

        timer = 0f;
        float shrinkTime = 0.2f;
        while (timer < shrinkTime)
        {
            timer += Time.deltaTime;
            float scale = Mathf.Lerp(1.0f, 0f, timer / shrinkTime);
            textTransform.localScale = Vector3.one * scale;
            yield return null;
        }

        comboText.gameObject.SetActive(false);
    }

    public void ShowGameOverPanel(int finalScore, int highScore)
    {
        HideComboPopupImmediate();

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