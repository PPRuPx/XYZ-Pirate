using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Components.GameObjectBased
{
    public class DropLootComponent : MonoBehaviour
    {
        private const float Z_OFFSET = -0.1f;

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
                    for (int i = 0; i < loot.Count; i++)
                    {
                        var targetPos = _target.position;
                        var spawnPos = new Vector3(targetPos.x, targetPos.y, targetPos.z + Z_OFFSET);
                        var go = Instantiate(loot.Prefab, spawnPos, Quaternion.identity);
                        var rb = go.GetComponent<Rigidbody2D>();
                        if (rb != null)
                            rb.AddForce(loot.DropVector, ForceMode2D.Impulse);
                    }
                }
            }
        }

        [Serializable]
        public class Loot
        {
            [Range(0, 100)] public float Chance;
            public GameObject Prefab;
            public Vector2 DropVector;
            public int Count = 1;
        }
    }
}