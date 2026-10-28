using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DraggablePiece : MonoBehaviour
{
    [Header("Tham chiếu hiển thị")]
    [SerializeField] private SpriteRenderer blockVisualPrefab;
    [SerializeField] private bool useNeonStyle = true;

    [Header("Trải nghiệm kéo thả (UX)")]
    [SerializeField] private float trayScale = 0.6f;
    [SerializeField] private Vector3 dragOffset = new Vector3(0f, 1.5f, 0f);

    public ShapeData CurrentShape { get; private set; }

    private PieceSpawner spawner;
    private List<GameObject> spawnedVisuals = new List<GameObject>();
    private Vector3 startPosition;
    private Camera mainCamera;
    private bool isDragging = false;

    private void Awake()
    {
        mainCamera = Camera.main;
        startPosition = transform.position;
    }

    public void SetupPiece(ShapeData shapeData, PieceSpawner pieceSpawner)
    {
        CurrentShape = shapeData;
        spawner = pieceSpawner;

        transform.position = startPosition;
        transform.localScale = Vector3.one * trayScale;
        gameObject.SetActive(true);

        RebuildVisualBlocks();
    }

    private void RebuildVisualBlocks()
    {
        foreach (GameObject oldBlock in spawnedVisuals)
        {
            if (oldBlock != null) Destroy(oldBlock);
        }
        spawnedVisuals.Clear();

        float cellSize = GridManager.Instance != null ? GridManager.Instance.CellSize : 1.1f;
        Color baseColor = CurrentShape.shapeColor;

        foreach (Vector2Int offset in CurrentShape.offsets)
        {
            SpriteRenderer rimBlock = Instantiate(blockVisualPrefab, transform);
            rimBlock.transform.localPosition = new Vector3(offset.x * cellSize, offset.y * cellSize, -1f);
            rimBlock.sortingOrder = 10;

            GameObject coreObj = new GameObject("InnerCore");
            coreObj.transform.SetParent(rimBlock.transform, false);
            coreObj.transform.localScale = new Vector3(0.78f, 0.78f, 1f);
            SpriteRenderer coreRenderer = coreObj.AddComponent<SpriteRenderer>();
            coreRenderer.sprite = rimBlock.sprite;
            coreRenderer.sortingOrder = 11;

            GameObject glowObj = new GameObject("CenterGlow");
            glowObj.transform.SetParent(rimBlock.transform, false);
            glowObj.transform.localScale = new Vector3(0.36f, 0.36f, 1f);
            SpriteRenderer glowRenderer = glowObj.AddComponent<SpriteRenderer>();
            glowRenderer.sprite = rimBlock.sprite;
            glowRenderer.sortingOrder = 12;

            if (useNeonStyle)
            {
                rimBlock.color = Color.Lerp(baseColor, Color.white, 0.45f);
                coreRenderer.color = Color.Lerp(baseColor, Color.black, 0.40f);
                glowRenderer.color = Color.Lerp(baseColor, Color.white, 0.55f);
            }
            else
            {
                rimBlock.color = Color.Lerp(baseColor, Color.black, 0.45f);
                coreRenderer.color = baseColor;
                glowRenderer.color = Color.Lerp(baseColor, Color.white, 0.40f);
            }

            spawnedVisuals.Add(rimBlock.gameObject);
        }
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
            return;

        isDragging = true;
        transform.localScale = Vector3.one;
        transform.position = GetMouseWorldPos() + dragOffset;
    }

    private void OnMouseDrag()
    {
        if (!isDragging) return;

        transform.position = GetMouseWorldPos() + dragOffset;

        GridCell targetCell = GridManager.Instance.GetCellFromWorldPosition(transform.position);
        GridManager.Instance.UpdateGhostPreview(targetCell, CurrentShape.offsets, CurrentShape.shapeColor);
    }

    private void OnMouseUp()
    {
        if (!isDragging) return;
        isDragging = false;

        GridManager.Instance.ClearGhostPreview();

        GridCell targetCell = GridManager.Instance.GetCellFromWorldPosition(transform.position);

        if (GridManager.Instance.CanPlaceShape(targetCell, CurrentShape.offsets))
        {
            GridManager.Instance.PlaceShape(targetCell, CurrentShape.offsets, CurrentShape.shapeColor);
            gameObject.SetActive(false);

            if (spawner != null)
            {
                spawner.OnPiecePlaced();
            }
        }
        else
        {
            ReturnToTray();
        }
    }

    private void ReturnToTray()
    {
        transform.position = startPosition;
        transform.localScale = Vector3.one * trayScale;
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = -mainCamera.transform.position.z;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePoint);
        worldPos.z = 0f;
        return worldPos;
    }
}