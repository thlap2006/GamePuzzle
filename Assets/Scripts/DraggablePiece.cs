using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DraggablePiece : MonoBehaviour
{
    [Header("Tham chiếu hiển thị")]
    [SerializeField] private SpriteRenderer blockVisualPrefab;

    [Header("Trải nghiệm kéo thả (UX)")]
    [SerializeField] private float trayScale = 0.6f;
    [SerializeField] private Vector3 dragOffset = new Vector3(0f, 1.5f, 0f);

    public ShapeData CurrentShape { get; private set; }

    private PieceSpawner spawner;
    private List<SpriteRenderer> spawnedVisuals = new List<SpriteRenderer>();
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
        foreach (SpriteRenderer oldBlock in spawnedVisuals)
        {
            if (oldBlock != null) Destroy(oldBlock.gameObject);
        }
        spawnedVisuals.Clear();

        float cellSize = GridManager.Instance != null ? GridManager.Instance.CellSize : 1.1f;

        foreach (Vector2Int offset in CurrentShape.offsets)
        {
            SpriteRenderer block = Instantiate(blockVisualPrefab, transform);
            block.transform.localPosition = new Vector3(offset.x * cellSize, offset.y * cellSize, -1f);
            block.color = CurrentShape.shapeColor;
            block.sortingOrder = 10;

            spawnedVisuals.Add(block);
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
    }

    private void OnMouseUp()
    {
        if (!isDragging) return;
        isDragging = false;

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