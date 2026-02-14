using UnityEngine;
using UnityEngine.Events;

namespace Utils.ObjectPool
{
    public class PoolItem : MonoBehaviour
    {
        private int _id;
        private Pool _pool;

        [SerializeField] private UnityEvent _onRestart;
        
        public void Retain(int id, Pool pool)
        {
            _id = id;
            _pool = pool;
        }
        
        public void Release()
        {
            _pool.Release(_id, this);
        }
        
        public void Restart()
        {
            _onRestart?.Invoke();
        }
    }
}