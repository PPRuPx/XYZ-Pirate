using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Components
{
    public class DropLootComponent : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Loot[] _drop;

        [ContextMenu("DropLoot")]
        public void DropLoot()
        {
            foreach (var loot in _drop)
            {
                float roll = Random.Range(0f, 100f);
                if (roll <= loot.Chance)
                {
                    var go = Instantiate(loot.Prefab, _target.position, Quaternion.identity);
                    var rb = go.GetComponent<Rigidbody2D>();
                    if (rb != null)
                        rb.AddForce(loot.DropVector, ForceMode2D.Impulse);
                }
            }
        }
        
        [Serializable]
        public class Loot
        {
            [Range(0, 100)]
            public float Chance;
            public GameObject Prefab;
            public Vector2 DropVector;
        }
    }
}