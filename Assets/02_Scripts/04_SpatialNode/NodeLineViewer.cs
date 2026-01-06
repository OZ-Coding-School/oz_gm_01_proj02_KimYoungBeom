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
        float r = circleRadius;

        Vector3 center = Vector3.zero;
        Vector3 uS = new Vector3(0, r, 0); Vector3 uE = new Vector3(0, half, 0);
        Vector3 dS = new Vector3(0, -r, 0); Vector3 dE = new Vector3(0, -half, 0);
        Vector3 lS = new Vector3(-r, 0, 0); Vector3 lE = new Vector3(-half, 0, 0);
        Vector3 rS = new Vector3(r, 0, 0); Vector3 rE = new Vector3(half, 0, 0);

        switch (shape)
        {
            case ENodeShape.Cross:
                points.AddRange(new[] { uE, uS, dS, dE, dS, center, lS, lE, lS, center, rS, rE });
                break;
            case ENodeShape.Horizontal:
                points.AddRange(new[] { lE, lS, rS, rE });
                break;
            case ENodeShape.Vertical:
                points.AddRange(new[] { uE, uS, dS, dE });
                break;
            case ENodeShape.UpRight:
                points.AddRange(new[] { uE, uS, center, rS, rE });
                break;
            case ENodeShape.UpLeft:
                points.AddRange(new[] { uE, uS, center, lS, lE });
                break;
            case ENodeShape.DownRight:
                points.AddRange(new[] { dE, dS, center, rS, rE });
                break;
            case ENodeShape.DownLeft:
                points.AddRange(new[] { dE, dS, center, lS, lE });
                break;
        }

        _lineRenderer.positionCount = points.Count;
        _lineRenderer.SetPositions(points.ToArray());
    }
}