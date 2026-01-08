#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class NodeGraphBaker : EditorWindow
{
    public NodeGraphSO targetGraph;
    public Transform nodeRoot;
    public float gridUnit = 1.0f;

    [MenuItem("Tools/Cubes/Node Graph Baker")]
    public static void ShowWindow() => GetWindow<NodeGraphBaker>("Node Baker");

    private void OnGUI()
    {
        targetGraph = (NodeGraphSO)EditorGUILayout.ObjectField("대상 SO", targetGraph, typeof(NodeGraphSO), false);
        nodeRoot = (Transform)EditorGUILayout.ObjectField("노드 루트(부모)", nodeRoot, typeof(Transform), true);
        gridUnit = EditorGUILayout.FloatField("그리드 단위", gridUnit);

        if (GUILayout.Button("씬 정보를 SO에 베이킹하기")) Bake();
    }

    private void Bake()
    {
        if (targetGraph == null || nodeRoot == null) return;

        Undo.RecordObject(targetGraph, "Bake Node Graph");
        targetGraph.Nodes.Clear();

        SpatialNode[] sceneNodes = nodeRoot.GetComponentsInChildren<SpatialNode>();
        foreach (var node in sceneNodes)
        {
            Vector2Int calcCoord = new Vector2Int(
                Mathf.RoundToInt(node.transform.position.x / gridUnit),
                Mathf.RoundToInt(node.transform.position.z / gridUnit)
            );

            NodeData newData = new NodeData
            {
                worldPos = node.transform.position,
                gridCoord = calcCoord,
                allowedDirs = GetDirectionsFromShape(node.NodeShape),
                nodeShape = node.NodeShape,
                nodeState = node.NodeState,
            };
            targetGraph.AddNode(newData);
        }

        EditorUtility.SetDirty(targetGraph);
        AssetDatabase.SaveAssets();
        Utils.Log($"{targetGraph.Nodes.Count}개의 노드 데이터가 성공적으로 베이킹되었습니다.");
    }

    private List<Vector2Int> GetDirectionsFromShape(ENodeShape shape)
    {
        return shape switch
        {
            ENodeShape.Cross => new() { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right },
            ENodeShape.Horizontal => new() { Vector2Int.left, Vector2Int.right },
            ENodeShape.Vertical => new() { Vector2Int.up, Vector2Int.down },
            ENodeShape.UpRight => new() { Vector2Int.up, Vector2Int.right },
            ENodeShape.UpLeft => new() { Vector2Int.up, Vector2Int.left },
            ENodeShape.DownRight => new() { Vector2Int.down, Vector2Int.right },
            ENodeShape.DownLeft => new() { Vector2Int.down, Vector2Int.left },
            ENodeShape.TUp => new() { Vector2Int.down, Vector2Int.left, Vector2Int.right },
            ENodeShape.TDown => new() { Vector2Int.up, Vector2Int.left, Vector2Int.right },
            ENodeShape.TRight => new() { Vector2Int.up, Vector2Int.down, Vector2Int.left },
            ENodeShape.TLeft => new() { Vector2Int.up, Vector2Int.down, Vector2Int.right },
            _ => new()
        };
    }
}
#endif