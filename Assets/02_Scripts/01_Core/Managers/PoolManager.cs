using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] private List<PoolableObjSO> _prewarmList = new List<PoolableObjSO>();

    private readonly Dictionary<PoolableObjSO, IPoolContainer> _poolDict = new();

    private void Awake()
    {
        foreach (var data in _prewarmList)
        {
            InitializeAndPrewarm(data);
        }
    }

    private void InitializeAndPrewarm(PoolableObjSO data)
    {
        if (data == null || data.prefab == null) return;

        if (!_poolDict.ContainsKey(data))
        {
            var container = CreateContainer(data);
            _poolDict.Add(data, container);

            List<PoolableComponent> tempObjects = new List<PoolableComponent>();

            for (int i = 0; i < data.defaultCapacity; i++)
            {
                PoolableComponent prefab = container.GetBase();
                prefab.poolData = data;
                tempObjects.Add(prefab);
            }

            foreach (var obj in tempObjects)
            {
                container.ReleaseBase(obj);
            }
        }
    }

    private IPoolContainer CreateContainer(PoolableObjSO data)
    {
        var containerType = typeof(PoolContainer<>).MakeGenericType(data.prefab.GetType());
        return (IPoolContainer)System.Activator.CreateInstance(containerType, data, transform);
    }

    public T Spawn<T>(PoolableObjSO data, Vector3 position, Quaternion rotation) where T : PoolableComponent
    {
        if (!_poolDict.TryGetValue(data, out var container))
        {
            container = new PoolContainer<T>(data, transform);
            _poolDict.Add(data, container);
        }

        var typedContainer = (PoolContainer<T>)container;
        T obj = typedContainer.Get();
        obj.transform.SetPositionAndRotation(position, rotation);
        return obj;
    }
    public T Spawn<T>(PoolableObjSO data, Vector3 position) where T : PoolableComponent
    {
        return Spawn<T>(data, position, Quaternion.identity);
    }
    public void Despawn<T>(PoolableObjSO data, T obj) where T : PoolableComponent
    {
        if (_poolDict.TryGetValue(data, out var container))
        {
            ((PoolContainer<T>)container).Release(obj);
        }
    }

    public void DespawnAll()
    {
        foreach (var container in _poolDict.Values)
        {
            container.DespawnAll();
        }
    }
}