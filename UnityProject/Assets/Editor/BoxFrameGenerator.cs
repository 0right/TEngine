using UnityEngine;
using UnityEditor;

public class BoxFrameGenerator : EditorWindow
{
    private float thickness = 0.1f; // 框架粗细
    private float size = 1f;     // 边长
    private GameObject frameObject;

    [MenuItem("Tools/Box Frame Generator")]
    public static void ShowWindow()
    {
        GetWindow<BoxFrameGenerator>("框架生成器");
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();

        thickness = EditorGUILayout.FloatField("框架粗细", thickness);
        size = EditorGUILayout.FloatField("边长", size);

        if (GUILayout.Button("生成框架") || EditorGUI.EndChangeCheck())
        {
            if (frameObject == null)
            {
                CreateFrame();
            }
            else
            {
                UpdateFrame();
            }
        }
    }

    private void CreateFrame()
    {
        frameObject = new GameObject("BoxFrame");
        
        // 创建四个边
        CreateEdge("TopEdge", new Vector3(0, size/2, 0), new Vector3(size, thickness, thickness));
        CreateEdge("BottomEdge", new Vector3(0, -size/2, 0), new Vector3(size, thickness, thickness));
        CreateEdge("LeftEdge", new Vector3(-size/2, 0, 0), new Vector3(thickness, size, thickness));
        CreateEdge("RightEdge", new Vector3(size/2, 0, 0), new Vector3(thickness, size, thickness));

        Rigidbody rb = frameObject.AddComponent<Rigidbody>();
    }

    private void CreateEdge(string name, Vector3 position, Vector3 scale)
    {
        GameObject edge = GameObject.CreatePrimitive(PrimitiveType.Cube);
        edge.name = name;
        edge.transform.parent = frameObject.transform;
        edge.transform.localPosition = position;
        edge.transform.localScale = scale;
        
        // 删除自动生成的BoxCollider（因为我们要在父物体上添加）
        DestroyImmediate(edge.GetComponent<BoxCollider>());
        
        // 在父物体上添加该边对应的BoxCollider
        BoxCollider bc = frameObject.AddComponent<BoxCollider>();
        bc.center = position;
        bc.size = scale;
    }

    private void UpdateFrame()
    {
        // 获取所有边
        Transform topEdge = frameObject.transform.Find("TopEdge");
        Transform bottomEdge = frameObject.transform.Find("BottomEdge");
        Transform leftEdge = frameObject.transform.Find("LeftEdge");
        Transform rightEdge = frameObject.transform.Find("RightEdge");

        // 更新边的位置和大小
        topEdge.localPosition = new Vector3(0, size/2, 0);
        bottomEdge.localPosition = new Vector3(0, -size/2, 0);
        leftEdge.localPosition = new Vector3(-size/2, 0, 0);
        rightEdge.localPosition = new Vector3(size/2, 0, 0);

        topEdge.localScale = new Vector3(size, thickness, thickness);
        bottomEdge.localScale = new Vector3(size, thickness, thickness);
        leftEdge.localScale = new Vector3(thickness, size, thickness);
        rightEdge.localScale = new Vector3(thickness, size, thickness);

        // 获取所有BoxCollider并更新
        BoxCollider[] colliders = frameObject.GetComponents<BoxCollider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            Transform edge = null;
            switch (i)
            {
                case 0: edge = topEdge; break;
                case 1: edge = bottomEdge; break;
                case 2: edge = leftEdge; break;
                case 3: edge = rightEdge; break;
            }
            
            if (edge != null)
            {
                colliders[i].center = edge.localPosition;
                colliders[i].size = edge.localScale;
            }
        }
    }
} 