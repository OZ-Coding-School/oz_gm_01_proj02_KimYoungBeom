using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class NodeLineViewer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpatialNode _spatialNode;
    [SerializeField] private Transform _circleTransform;
    [SerializeField] private LineRenderer _lineRenderer;

    [Header("Visual Settings")]
    public Color visualColor = Color.cyan;
    [Range(0.05f, 0.45f)] public float circleRadius = 0.1f;
    [Range(0.01f, 0.2f)] public float lineWidth = 0.05f;

    private void OnValidate() => UpdateVisuals();
    private void OnEnable() => UpdateVisuals();

    public void UpdateVisuals()
    {
        float finalScale = circleRadius * 2f;
        _circleTransform.localScale = new Vector3(finalScale, finalScale, 1f);

        UpdateLines(_spatialNode.NodeShape);
    }

    private void UpdateLines(ENodeShape shape)
    {
        _lineRenderer.useWorldSpace = false;
        _lineRenderer.alignment = LineAlignment.TransformZ;

        _lineRenderer.startWidth = lineWidth;
        _lineRenderer.endWidth = lineWidth;
        _lineRenderer.startColor = visualColor;
        _lineRenderer.endColor = visualColor;

        List<Vector3> points = new List<Vector3>();
        float half = 0.5f;

        Vector3 center = Vector3.zero;
        Vector3 up = new Vector3(0, half, 0);
        Vector3 down = new Vector3(0, -half, 0);
        Vector3 left = new Vector3(-half, 0, 0);
        Vector3 right = new Vector3(half, 0, 0);

        switch (shape)
        {
            case ENodeShape.Cross:
                points.AddRange(new[] { up, center, down, center, left, center, right });
                break;
            case ENodeShape.Horizontal:
                points.AddRange(new[] { left, right });
                break;
            case ENodeShape.Vertical:
                points.AddRange(new[] { up, down });
                break;
            case ENodeShape.UpRight:
                points.AddRange(new[] { up, center, right });
                break;
            case ENodeShape.UpLeft:
                points.AddRange(new[] { up, center, left });
                break;
            case ENodeShape.DownRight:
                points.AddRange(new[] { down, center, right });
                break;
            case ENodeShape.DownLeft:
                points.AddRange(new[] { down, center, left });
                break;
            case ENodeShape.TUp:
                points.AddRange(new[] { left, center, right, center, down });
                break;
            case ENodeShape.TDown:
                points.AddRange(new[] { left, center, right, center, up });
                break;
            case ENodeShape.TRight:
                points.AddRange(new[] { up, center, down, center, left });
                break;
            case ENodeShape.TLeft:
                points.AddRange(new[] { up, center, down, center, right });
                break;
        }

        _lineRenderer.positionCount = points.Count;
        _lineRenderer.SetPositions(points.ToArray());
    }
}