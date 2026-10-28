using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Cấu hình kích thước bàn cờ 2D")]
    [SerializeField] private int width = 8;
    [SerializeField] private int height = 8;
    [SerializeField] private float cellSize = 1.1f;

    [Header("Tham chiếu & Giao diện")]
    [SerializeField] private GridCell cellPrefab;
    [SerializeField] private Color primaryColor = new Color(0.85f, 0.85f, 0.85f);
    [SerializeField] private Color secondaryColor = new Color(0.75f, 0.75f, 0.75f);

    public float CellSize => cellSize;

    private GridCell[,] gridArray;
    private Vector2 originOffset;

    // Lưu danh sách các ô đang hiện bóng mờ để tắt đi cực nhanh khi di chuyển chuột
    private List<GridCell> activePreviewCells = new List<GridCell>();

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
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        gridArray = new GridCell[width, height];

        float offsetX = (width - 1) * cellSize * 0.5f;
        float offsetY = (height - 1) * cellSize * 0.5f;
        originOffset = new Vector2(offsetX, offsetY);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 worldPos = GetWorldPosition(x, y);
                GridCell newCell = Instantiate(cellPrefab, worldPos, Quaternion.identity, transform);

                bool isOffsetColor = (x + y) % 2 == 1;
                Color colorToUse = isOffsetColor ? secondaryColor : primaryColor;

                newCell.Init(x, y, colorToUse);
                gridArray[x, y] = newCell;
            }
        }
    }

    public bool CanPlaceShape(GridCell rootCell, Vector2Int[] shapeOffsets)
    {
        if (rootCell == null) return false;

        foreach (Vector2Int offset in shapeOffsets)
        {
            int targetX = rootCell.X + offset.x;
            int targetY = rootCell.Y + offset.y;

            GridCell targetCell = GetCell(targetX, targetY);
            if (targetCell == null || targetCell.IsOccupied)
            {
                return false;
            }
        }
        return true;
    }

    public bool CanShapeFitAnywhere(Vector2Int[] shapeOffsets)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridCell cell = GetCell(x, y);
                if (CanPlaceShape(cell, shapeOffsets))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void UpdateGhostPreview(GridCell rootCell, Vector2Int[] shapeOffsets, Color pieceColor)
    {
        ClearGhostPreview();

        if (!CanPlaceShape(rootCell, shapeOffsets)) return;

        foreach (Vector2Int offset in shapeOffsets)
        {
            GridCell cell = GetCell(rootCell.X + offset.x, rootCell.Y + offset.y);
            if (cell != null)
            {
                cell.ShowPreview(pieceColor);
                activePreviewCells.Add(cell);
            }
        }
    }

    public void ClearGhostPreview()
    {
        foreach (GridCell cell in activePreviewCells)
        {
            if (cell != null)
            {
                cell.HidePreview();
            }
        }
        activePreviewCells.Clear();
    }

    public void PlaceShape(GridCell rootCell, Vector2Int[] shapeOffsets, Color pieceColor)
    {
        ClearGhostPreview();

        foreach (Vector2Int offset in shapeOffsets)
        {
            int targetX = rootCell.X + offset.x;
            int targetY = rootCell.Y + offset.y;

            GridCell targetCell = GetCell(targetX, targetY);
            if (targetCell != null)
            {
                targetCell.PlaceBlock(pieceColor);
            }
        }

        int linesCleared = CheckAndClearLines();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(shapeOffsets.Length, linesCleared);
        }

        if (linesCleared > 0 && BoosterManager.Instance != null)
        {
            BoosterManager.Instance.OnLinesCleared(linesCleared);
        }
    }

    private int CheckAndClearLines()
    {
        List<int> fullRows = new List<int>();
        List<int> fullCols = new List<int>();

        for (int y = 0; y < height; y++)
        {
            bool isRowFull = true;
            for (int x = 0; x < width; x++)
            {
                if (!gridArray[x, y].IsOccupied)
                {
                    isRowFull = false;
                    break;
                }
            }
            if (isRowFull) fullRows.Add(y);
        }

        for (int x = 0; x < width; x++)
        {
            bool isColFull = true;
            for (int y = 0; y < height; y++)
            {
                if (!gridArray[x, y].IsOccupied)
                {
                    isColFull = false;
                    break;
                }
            }
            if (isColFull) fullCols.Add(x);
        }

        foreach (int y in fullRows)
        {
            for (int x = 0; x < width; x++)
            {
                gridArray[x, y].ClearBlock();
            }
        }

        foreach (int x in fullCols)
        {
            for (int y = 0; y < height; y++)
            {
                gridArray[x, y].ClearBlock();
            }
        }

        return fullRows.Count + fullCols.Count;
    }
    public bool SmashArea(GridCell centerCell, int radius = 1)
    {
        if (centerCell == null) return false;

        bool anyBlockSmashed = false;

        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                GridCell target = GetCell(centerCell.X + dx, centerCell.Y + dy);
                if (target != null && target.IsOccupied)
                {
                    target.ClearBlock();
                    anyBlockSmashed = true;
                }
            }
        }

        return anyBlockSmashed;
    }

    public void ClearAllCells()
    {
        ClearGhostPreview();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridArray[x, y].ClearBlock();
            }
        }
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        float worldX = (x * cellSize) - originOffset.x;
        float worldY = (y * cellSize) - originOffset.y;
        return new Vector3(worldX, worldY, 0f);
    }

    public GridCell GetCellFromWorldPosition(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt((worldPos.x + originOffset.x) / cellSize);
        int y = Mathf.RoundToInt((worldPos.y + originOffset.y) / cellSize);
        return GetCell(x, y);
    }

    public GridCell GetCell(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return gridArray[x, y];
        }
        return null;
    }
}