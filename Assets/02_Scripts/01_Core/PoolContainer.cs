using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using System.Linq;

public class PoolContainer<T> : IPoolContainer where T : PoolableComponent
{
    private readonly IObjectPool<T> _pool;
    private readonly HashSet<T> _activeObjects = new();
    private readonly PoolableObjSO _data;
    private readonly Transform _root;

    public PoolContainer(PoolableObjSO data, Transform root)
    {
        _data = data;
        _root = root;
        _pool = new ObjectPool<T>(CreateFunc, OnGet, OnRelease, OnDestroyPool, true, data.defaultCapacity, data.maxSize);
    }

    private T CreateFunc() => Object.Instantiate(_data.prefab, _root) as T;

    private void OnGet(T obj)
    {
        obj.gameObject.SetActive(true);
        obj.OnSpawn();
        _activeObjects.Add(obj);
    }

    private void OnRelease(T obj)
    {
        obj.OnDespawn();
        obj.gameObject.SetActive(false);
        _activeObjects.Remove(obj);
    }

    private void OnDestroyPool(T obj)
    {
        _activeObjects.Remove(obj);
        Object.Destroy(obj.gameObject);
    }

    public T Get() => _pool.Get();
    public void Release(T obj) => _pool.Release(obj);

    public void DespawnAll()
    {
        if (_activeObjects.Count == 0) return;

        var activeList = _activeObjects.ToList();
        foreach (var obj in activeList)
        {
            _pool.Release(obj);
        }
        _activeObjects.Clear();
    }

    public void Clear() => _pool.Clear();

    public PoolableComponent GetBase()
    {
        return Get();
    }

    public void ReleaseBase(PoolableComponent obj)
    {
        Release(obj as T);
    }
}