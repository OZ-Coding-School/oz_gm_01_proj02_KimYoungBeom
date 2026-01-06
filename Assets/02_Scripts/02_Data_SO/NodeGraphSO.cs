using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNodeGraph", menuName = "Cubes/Node/Node Graph")]
public class NodeGraphSO : ScriptableObject
{
    [SerializeField] private List<NodeData> _nodes = new List<NodeData>();

    public List<NodeData> Nodes => _nodes;

    public void AddNode(NodeData newNode) => _nodes.Add(newNode);
}