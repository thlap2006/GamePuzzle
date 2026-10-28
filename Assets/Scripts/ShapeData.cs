using UnityEngine;

[CreateAssetMenu(fileName = "Shape_", menuName = "Puzzle/Shape Data")]
public class ShapeData : ScriptableObject
{
    [Header("Thông tin khối gạch")]
    public string shapeName = "New Shape";
    public Color shapeColor = Color.cyan;

    [Header("Tọa độ các ô con so với tâm (0,0)")]
    public Vector2Int[] offsets = new Vector2Int[]
    {
        new Vector2Int(0, 0)
    };
}