using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewVector3Event", menuName = "Cubes/EventCHSO/Vector3 Event Channel")]
public class Vector3NStateEventCHSO : ScriptableObject
{
    public event Action<Vector3, ENodeState> onEvent;
    public void Raised(Vector3 worldPos, ENodeState state)
    {
        onEvent?.Invoke(worldPos, state);
    }
}
