using UnityEngine;
using Utils;
using Utils.ObjectPool;

namespace Components.GameObjectBased
{
    public class SpawnComponent : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private GameObject _prefab;
        [SerializeField] private bool _usePool;

        [ContextMenu("Spawn")]
        public void Spawn() => SpawnInstance();
        
        public GameObject SpawnInstance()
        {
            var targetPos = _target.position;
            var instance = _usePool
                ? Pool.Instance.Get(_prefab, targetPos)
                : SpawnUtils.Spawn(_prefab, targetPos);

            var scale = _target.lossyScale;
            instance.transform.localScale = scale;
            instance.SetActive(true);
            
            return instance;
        }

        public void SetPrefab(GameObject prefab) => _prefab = prefab;
    }
}
