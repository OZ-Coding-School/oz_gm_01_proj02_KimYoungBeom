using UnityEngine;

[CreateAssetMenu(fileName = "PoolData_", menuName = "Cubes/Pool/PoolableObjectSO")]
public class PoolableObjSO : ScriptableObject
{
    public PoolableComponent prefab;
    public int defaultCapacity = 10;
    public int maxSize = 1000;
}