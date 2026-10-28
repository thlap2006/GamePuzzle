#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class ShapeLibraryGenerator
{

    [MenuItem("Puzzle Tools/Tự Động Tạo 32 Khối Gạch")]
    public static void GenerateAllShapes()
    {
        string folderPath = "Assets/Shapes";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        Color cyanNeon = new Color(0.00f, 0.90f, 1.00f); 
        Color blueNeon = new Color(0.20f, 0.50f, 1.00f); 
        Color greenNeon = new Color(0.15f, 0.95f, 0.45f); 
        Color yellowNeon = new Color(1.00f, 0.85f, 0.10f); 
        Color orangeNeon = new Color(1.00f, 0.50f, 0.10f); 
        Color pinkNeon = new Color(1.00f, 0.20f, 0.65f); 
        Color purpleNeon = new Color(0.70f, 0.25f, 1.00f); 
        Color redNeon = new Color(1.00f, 0.22f, 0.25f); 


        CreateShape(folderPath, "01_Single_1x1", cyanNeon, V(0, 0));
        CreateShape(folderPath, "02_Ngang_2", cyanNeon, V(0, 0), V(1, 0));
        CreateShape(folderPath, "03_Doc_2", cyanNeon, V(0, 0), V(0, 1));
        CreateShape(folderPath, "04_Ngang_3", blueNeon, V(-1, 0), V(0, 0), V(1, 0));
        CreateShape(folderPath, "05_Doc_3", blueNeon, V(0, -1), V(0, 0), V(0, 1));
        CreateShape(folderPath, "06_Ngang_4", orangeNeon, V(-1, 0), V(0, 0), V(1, 0), V(2, 0));
        CreateShape(folderPath, "07_Doc_4", orangeNeon, V(0, -1), V(0, 0), V(0, 1), V(0, 2));
        CreateShape(folderPath, "08_Ngang_5", redNeon, V(-2, 0), V(-1, 0), V(0, 0), V(1, 0), V(2, 0));
        CreateShape(folderPath, "09_Doc_5", redNeon, V(0, -2), V(0, -1), V(0, 0), V(0, 1), V(0, 2));

        CreateShape(folderPath, "10_Cheo_Phai_2", greenNeon, V(0, 0), V(1, 1));
        CreateShape(folderPath, "11_Cheo_Trai_2", greenNeon, V(0, 0), V(-1, 1));
        CreateShape(folderPath, "12_Cheo_Phai_3", greenNeon, V(-1, -1), V(0, 0), V(1, 1));
        CreateShape(folderPath, "13_Cheo_Trai_3", greenNeon, V(1, -1), V(0, 0), V(-1, 1));

        CreateShape(folderPath, "14_GocNho_PhaiTren", yellowNeon, V(0, 0), V(1, 0), V(0, 1));
        CreateShape(folderPath, "15_GocNho_TraiTren", yellowNeon, V(0, 0), V(-1, 0), V(0, 1));
        CreateShape(folderPath, "16_GocNho_PhaiDuoi", yellowNeon, V(0, 0), V(1, 0), V(0, -1));
        CreateShape(folderPath, "17_GocNho_TraiDuoi", yellowNeon, V(0, 0), V(-1, 0), V(0, -1));

        CreateShape(folderPath, "18_GocLon_PhaiTren", pinkNeon, V(-1, -1), V(0, -1), V(1, -1), V(-1, 0), V(-1, 1));
        CreateShape(folderPath, "19_GocLon_TraiTren", pinkNeon, V(-1, -1), V(0, -1), V(1, -1), V(1, 0), V(1, 1));
        CreateShape(folderPath, "20_GocLon_PhaiDuoi", pinkNeon, V(-1, 1), V(0, 1), V(1, 1), V(-1, 0), V(-1, -1));
        CreateShape(folderPath, "21_GocLon_TraiDuoi", pinkNeon, V(-1, 1), V(0, 1), V(1, 1), V(1, 0), V(1, -1));

        CreateShape(folderPath, "22_Chu_T_Len", purpleNeon, V(-1, 0), V(0, 0), V(1, 0), V(0, 1));
        CreateShape(folderPath, "23_Chu_T_Xuong", purpleNeon, V(-1, 0), V(0, 0), V(1, 0), V(0, -1));
        CreateShape(folderPath, "24_Chu_T_Phai", purpleNeon, V(0, -1), V(0, 0), V(0, 1), V(1, 0));
        CreateShape(folderPath, "25_Chu_T_Trai", purpleNeon, V(0, -1), V(0, 0), V(0, 1), V(-1, 0));

        CreateShape(folderPath, "26_Chu_Z_Ngang", orangeNeon, V(-1, 1), V(0, 1), V(0, 0), V(1, 0));
        CreateShape(folderPath, "27_Chu_S_Ngang", orangeNeon, V(-1, 0), V(0, 0), V(0, 1), V(1, 1));
        CreateShape(folderPath, "28_Chu_Z_Doc", orangeNeon, V(0, -1), V(0, 0), V(1, 0), V(1, 1));
        CreateShape(folderPath, "29_Chu_S_Doc", orangeNeon, V(1, -1), V(1, 0), V(0, 0), V(0, 1));

        CreateShape(folderPath, "30_Vuong_2x2", yellowNeon, V(0, 0), V(1, 0), V(0, 1), V(1, 1));
        CreateShape(folderPath, "31_ChuNhat_Ngang_3x2", pinkNeon, V(-1, 0), V(0, 0), V(1, 0), V(-1, 1), V(0, 1), V(1, 1));
        CreateShape(folderPath, "32_Dau_Cong_Plus", redNeon, V(0, 0), V(-1, 0), V(1, 0), V(0, 1), V(0, -1));
        CreateShape(folderPath, "33_Vuong_3x3", redNeon,
            V(-1, -1), V(0, -1), V(1, -1),
            V(-1, 0), V(0, 0), V(1, 0),
            V(-1, 1), V(0, 1), V(1, 1));

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("ĐÃ TẠO THÀNH CÔNG 33 KHỐI GẠCH TRONG THƯ MỤC Assets/Shapes!");
    }

    private static Vector2Int V(int x, int y) => new Vector2Int(x, y);

    private static void CreateShape(string folder, string fileName, Color color, params Vector2Int[] offsets)
    {
        string assetPath = $"{folder}/{fileName}.asset";

        ShapeData shape = AssetDatabase.LoadAssetAtPath<ShapeData>(assetPath);
        if (shape == null)
        {
            shape = ScriptableObject.CreateInstance<ShapeData>();
            AssetDatabase.CreateAsset(shape, assetPath);
        }

        shape.shapeName = fileName;
        shape.shapeColor = color;
        shape.offsets = offsets;

        EditorUtility.SetDirty(shape);
    }
}
#endif