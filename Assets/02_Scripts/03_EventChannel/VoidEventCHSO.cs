using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewVoidEvent", menuName = "Cubes/EventCHSO/Void Event Channel")]
public class VoidEventCHSO : ScriptableObject
{
    public event Action onEvent;
    public void Raised()
    {
        onEvent?.Invoke();
    }

}
