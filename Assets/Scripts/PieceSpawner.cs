using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    [Header("Danh sách 3 khay chứa gạch dưới đáy màn hình")]
    [SerializeField] private DraggablePiece[] trayPieces;

    [Header("Kho tất cả các hình dạng gạch (ScriptableObjects)")]
    [SerializeField] private ShapeData[] shapeLibrary;

    private void Start()
    {
        SpawnNewBatch();
    }

    public void SpawnNewBatch()
    {
        if (shapeLibrary == null || shapeLibrary.Length == 0)
        {
            Debug.LogError("Chưa kéo các file ShapeData vào Shape Library của PieceSpawner!");
            return;
        }

        foreach (DraggablePiece piece in trayPieces)
        {
            int randomIndex = Random.Range(0, shapeLibrary.Length);
            ShapeData randomShape = shapeLibrary[randomIndex];

            piece.SetupPiece(randomShape, this);
        }

        CheckGameOver();
    }

    public void OnPiecePlaced()
    {
        bool allPiecesUsed = true;

        foreach (DraggablePiece piece in trayPieces)
        {
            if (piece.gameObject.activeSelf)
            {
                allPiecesUsed = false;
                break;
            }
        }

        if (allPiecesUsed)
        {
            SpawnNewBatch();
        }
        else
        {
            CheckGameOver();
        }
    }

    private void CheckGameOver()
    {
        bool canMakeAnyMove = false;

        foreach (DraggablePiece piece in trayPieces)
        {
            if (piece.gameObject.activeSelf)
            {
                if (GridManager.Instance.CanShapeFitAnywhere(piece.CurrentShape.offsets))
                {
                    canMakeAnyMove = true;
                    break; 
                }
            }
        }

        if (!canMakeAnyMove)
        {
            Debug.LogWarning("GAME OVER! Không còn chỗ trống nào vừa với các khối gạch hiện tại!");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerGameOver();
            }
        }
    }
}