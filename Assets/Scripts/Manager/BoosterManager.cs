using System.Collections;
using UnityEngine;
using TMPro;

public class BoosterManager : MonoBehaviour
{
    public static BoosterManager Instance { get; private set; }

    [Header("Cấu hình Đồng Xu (Coins) & Giá mua Búa")]
    [SerializeField] private int startingCoins = 100; 
    [SerializeField] private int coinsPerLineCleared = 15; 
    [SerializeField] private int hammerCost = 50;   

    [Header("Giao diện UI Xu & Búa (Từ Hyper Casual UI Pack)")]
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI hammerCostText;
    [SerializeField] private GameObject hammerActivePrompt; 

    private int currentCoins;
    private bool isHammerMode = false;
    private Camera mainCamera;
    private Vector3 cameraOriginalPos;
    private Coroutine shakeCoroutine;

    private const string CoinsSaveKey = "PUZZLE_COINS";

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

        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            cameraOriginalPos = mainCamera.transform.position;
        }

        currentCoins = PlayerPrefs.GetInt(CoinsSaveKey, startingCoins);
    }

    private void Start()
    {
        if (hammerCostText != null)
        {
            hammerCostText.text = hammerCost.ToString();
        }

        if (hammerActivePrompt != null)
        {
            hammerActivePrompt.SetActive(false);
        }

        UpdateCoinUI();
    }

    private void Update()
    {
        if (isHammerMode && Input.GetMouseButtonDown(0))
        {
            TryUseHammerAtMousePosition();
        }
    }

    public void OnLinesCleared(int linesCount)
    {
        int reward = linesCount * coinsPerLineCleared;
        AddCoins(reward);

        float shakeStrength = linesCount >= 2 ? 0.18f : 0.08f;
        TriggerCameraShake(0.15f, shakeStrength);
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        PlayerPrefs.SetInt(CoinsSaveKey, currentCoins);
        PlayerPrefs.Save();
        UpdateCoinUI();
    }

    public void OnHammerButtonClicked()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
            return;

        if (isHammerMode)
        {
            SetHammerMode(false);
            return;
        }

        if (currentCoins >= hammerCost)
        {
            SetHammerMode(true);
        }
        else
        {
            Debug.Log("Không đủ Xu để dùng Búa!");
            TriggerCameraShake(0.1f, 0.05f);
        }
    }

    private void SetHammerMode(bool active)
    {
        isHammerMode = active;
        if (hammerActivePrompt != null)
        {
            hammerActivePrompt.SetActive(active);
        }
    }

    private void TryUseHammerAtMousePosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = -mainCamera.transform.position.z;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePoint);
        worldPos.z = 0f;

        GridCell clickedCell = GridManager.Instance.GetCellFromWorldPosition(worldPos);

        if (clickedCell != null && GridManager.Instance.SmashArea(clickedCell, 1))
        {
            AddCoins(-hammerCost);
            SetHammerMode(false);

            TriggerCameraShake(0.25f, 0.25f);
        }
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = currentCoins.ToString();
        }
    }

    public void TriggerCameraShake(float duration, float magnitude)
    {
        if (mainCamera == null) return;

        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            mainCamera.transform.position = cameraOriginalPos;
        }
        shakeCoroutine = StartCoroutine(CameraShakeRoutine(duration, magnitude));
    }

    private IEnumerator CameraShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            Vector2 randomOffset = Random.insideUnitCircle * magnitude;
            mainCamera.transform.position = cameraOriginalPos + new Vector3(randomOffset.x, randomOffset.y, 0f);

            yield return null;
        }

        mainCamera.transform.position = cameraOriginalPos;
    }
}