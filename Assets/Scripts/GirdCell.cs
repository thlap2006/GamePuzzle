using UnityEngine;

public class GridCell : MonoBehaviour
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public bool IsOccupied { get; private set; }

    [SerializeField] private SpriteRenderer cellRenderer;

    private Color defaultColor;

    public void Init(int x, int y, Color cellColor)
    {
        X = x;
        Y = y;
        IsOccupied = false;
        defaultColor = cellColor;

        gameObject.name = $"Cell_{x}_{y}";

        if (cellRenderer != null)
        {
            cellRenderer.color = defaultColor;
        }
    }

    public void PlaceBlock(Color blockColor)
    {
        IsOccupied = true;
        if (cellRenderer != null)
        {
            cellRenderer.color = blockColor;
        }
    }

    public void ClearBlock()
    {
        IsOccupied = false;
        if (cellRenderer != null)
        {
            cellRenderer.color = defaultColor;
        }
    }
}
