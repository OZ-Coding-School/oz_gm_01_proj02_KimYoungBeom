using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBoolEvent", menuName = "Cubes/EventCHSO/Bool Event Channel")]
public class BoolEventCHSO : ScriptableObject
{
    public event Action<bool> onEvent;
    public void Raised(bool value)
    {
        onEvent?.Invoke(value);
    }

}
