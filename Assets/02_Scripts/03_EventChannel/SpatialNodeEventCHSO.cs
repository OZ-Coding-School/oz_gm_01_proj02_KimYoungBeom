using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpatialNodeEvent", menuName = "Cubes/EventCHSO/Spatial Node Event Channel")]
public class SpatialNodeEventCHSO : ScriptableObject
{
    public event Action<SpatialNode> onEvent;
    public void Raised(SpatialNode node)
    {
        onEvent?.Invoke(node);
    }
}
