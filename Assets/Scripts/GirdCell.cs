using System.Collections;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public bool IsOccupied { get; private set; }

    [Header("Hiển thị & Phong cách Khối")]
    [SerializeField] private SpriteRenderer cellRenderer;
    [Tooltip("Bật = Phong cách Đèn Neon | Tắt = Phong cách Viền Đậm Cartoon 3D")]
    [SerializeField] private bool useNeonStyle = true;

    private SpriteRenderer innerCoreRenderer;
    private SpriteRenderer centerGlowRenderer;

    private Color defaultColor;
    private Vector3 initialScale;
    private Coroutine scaleCoroutine;

    public void Init(int x, int y, Color cellColor)
    {
        X = x;
        Y = y;
        IsOccupied = false;
        defaultColor = cellColor;
        initialScale = transform.localScale;

        gameObject.name = $"Cell_{x}_{y}";

        CreateBlockLayers();

        ApplyEmptyVisual();
    }

    private void CreateBlockLayers()
    {
        if (cellRenderer == null) return;

        GameObject coreObj = new GameObject("InnerCore");
        coreObj.transform.SetParent(transform, false);
        coreObj.transform.localScale = new Vector3(0.78f, 0.78f, 1f);

        innerCoreRenderer = coreObj.AddComponent<SpriteRenderer>();
        innerCoreRenderer.sprite = cellRenderer.sprite;
        innerCoreRenderer.sortingOrder = cellRenderer.sortingOrder + 1;

        GameObject glowObj = new GameObject("CenterGlow");
        glowObj.transform.SetParent(transform, false);
        glowObj.transform.localScale = new Vector3(0.36f, 0.36f, 1f);

        centerGlowRenderer = glowObj.AddComponent<SpriteRenderer>();
        centerGlowRenderer.sprite = cellRenderer.sprite;
        centerGlowRenderer.sortingOrder = cellRenderer.sortingOrder + 2;
    }

    private void ApplyEmptyVisual()
    {
        if (cellRenderer == null) return;

        cellRenderer.color = Color.Lerp(defaultColor, Color.white, 0.08f);

        if (innerCoreRenderer != null)
        {
            innerCoreRenderer.color = defaultColor;
        }

        if (centerGlowRenderer != null)
        {
            centerGlowRenderer.gameObject.SetActive(false);
        }
    }

    private void ApplyBlockVisual(Color baseColor, float alphaMultiplier = 1f)
    {
        if (cellRenderer == null) return;

        if (useNeonStyle)
        {

            Color neonRim = Color.Lerp(baseColor, Color.white, 0.45f);
            neonRim.a = alphaMultiplier;
            cellRenderer.color = neonRim;

            if (innerCoreRenderer != null)
            {
                Color darkCore = Color.Lerp(baseColor, Color.black, 0.40f);
                darkCore.a = alphaMultiplier;
                innerCoreRenderer.color = darkCore;
            }

            if (centerGlowRenderer != null)
            {
                centerGlowRenderer.gameObject.SetActive(true);
                Color centerLight = Color.Lerp(baseColor, Color.white, 0.55f);
                centerLight.a = 0.85f * alphaMultiplier;
                centerGlowRenderer.color = centerLight;
            }
        }
        else
        {
            Color darkBorder = Color.Lerp(baseColor, Color.black, 0.45f);
            darkBorder.a = alphaMultiplier;
            cellRenderer.color = darkBorder;

            if (innerCoreRenderer != null)
            {
                Color mainFill = baseColor;
                mainFill.a = alphaMultiplier;
                innerCoreRenderer.color = mainFill;
            }

            if (centerGlowRenderer != null)
            {
                centerGlowRenderer.gameObject.SetActive(true);
                Color gloss = Color.Lerp(baseColor, Color.white, 0.4f);
                gloss.a = 0.6f * alphaMultiplier;
                centerGlowRenderer.color = gloss;
            }
        }
    }

    public void PlaceBlock(Color blockColor)
    {
        IsOccupied = true;
        ApplyBlockVisual(blockColor, 1f);
        TriggerPopEffect(1.18f, 0.12f);
    }

    public void ClearBlock()
    {
        IsOccupied = false;
        ApplyEmptyVisual();
        TriggerPopEffect(1.3f, 0.18f);
    }

    public void ShowPreview(Color previewColor)
    {
        if (IsOccupied) return;

        ApplyBlockVisual(previewColor, 0.45f);
    }

    public void HidePreview()
    {
        if (IsOccupied) return;

        ApplyEmptyVisual();
    }

    private void TriggerPopEffect(float maxScaleMultiplier, float duration)
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }
        scaleCoroutine = StartCoroutine(PopRoutine(maxScaleMultiplier, duration));
    }

    private IEnumerator PopRoutine(float maxMultiplier, float duration)
    {
        float halfDuration = duration * 0.5f;
        Vector3 targetScale = initialScale * maxMultiplier;

        float timer = 0f;
        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(initialScale, targetScale, timer / halfDuration);
            yield return null;
        }

        timer = 0f;
        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(targetScale, initialScale, timer / halfDuration);
            yield return null;
        }

        transform.localScale = initialScale;
    }
}