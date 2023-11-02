using UnityEngine;

namespace MemeFight
{
    public interface IPoolable<T> where T : MonoBehaviour
    {
        /// <summary>Use this to store a reference to the pool, so it can be called later.</summary>
        void OnGetFromPool(PoolInstance<T> originPool);
        void ReturnToPool();
    }
}
