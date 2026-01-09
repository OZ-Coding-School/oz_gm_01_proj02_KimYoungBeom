using UnityEngine;

public class PublicAbstract
{

}

public abstract class PoolableComponent : MonoBehaviour, IPoolable
{
    public abstract void OnSpawn();
    public abstract void OnDespawn();
    public PoolableObjSO poolData;
    public abstract void ReturnPool();
}