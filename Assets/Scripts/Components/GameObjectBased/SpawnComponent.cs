using UnityEngine;

namespace Components.GameObjectBased
{
    public class SpawnComponent : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private GameObject _prefab;

        [ContextMenu("Spawn")]
        public void Spawn() => SpawnInstance();
        
        public GameObject SpawnInstance()
        {
            var instance = Instantiate(_prefab, _target.position, Quaternion.identity);
            instance.transform.localScale = _target.lossyScale;
            instance.SetActive(true);
            return instance;
        }

        public void SetPrefab(GameObject prefab) => _prefab = prefab;
    }
}
