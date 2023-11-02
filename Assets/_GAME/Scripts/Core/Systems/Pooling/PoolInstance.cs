using UnityEngine;
using UnityEngine.Pool;

namespace MemeFight
{
    public abstract class PoolInstance<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] protected T _prefab;
        [SerializeField] protected int _defaultCapacity = 10;
        [SerializeField] protected int _maxPoolSize = 20;
        [SerializeField] protected bool _collectionChecks = false;

        private IObjectPool<T> _pool;

        protected IObjectPool<T> Pool
        {
            get
            {
                if (_pool == null)
                {
                    _pool = new ObjectPool<T>(
                        CreatePooledItem,
                        OnGetFromPool,
                        OnReturnToPool,
                        OnDestroyPooledObject,
                        _collectionChecks,
                        _defaultCapacity,
                        _maxPoolSize);
                }

                return _pool;
            }
        }

        protected virtual T CreatePooledItem()
        {
            return Instantiate(_prefab);
        }

        protected virtual void OnGetFromPool(T element)
        {
            element.gameObject.SetActive(true);
        }

        protected virtual void OnReturnToPool(T element)
        {
            element.transform.SetParent(transform);
            element.gameObject.SetActive(false);
        }

        protected virtual void OnDestroyPooledObject(T element)
        {
            Destroy(element);
        }

        public void Clear()
        {
            _pool.Clear();
        }

        public abstract T Get();
        public abstract void Return(T element);
    }
}
