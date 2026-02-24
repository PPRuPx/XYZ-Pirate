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
        
        [Header("Spawn bound")] [Space]
        [SerializeField] private float _sectorAngle = 60;
        [SerializeField] private float _sectorRotation;
        [SerializeField] private float _force = 1;
        
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
                        {
                            var middleAngleDelta = (180 - _sectorRotation - _sectorAngle) / 2;
                            var randomAngle = Random.Range(middleAngleDelta, middleAngleDelta + _sectorAngle);
                            Vector3 direction = GetUnitOnCircle(randomAngle);
                            rb.AddForce(direction * _force, ForceMode2D.Impulse);
                        }
                    }
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var position = _target.transform.position;

            var middleAngleDelta = (180 - _sectorRotation - _sectorAngle) / 2;
            
            var rightBound = GetUnitOnCircle(middleAngleDelta);
            UnityEditor.Handles.DrawLine(position, position + rightBound);
            
            var leftBound = GetUnitOnCircle(middleAngleDelta + _sectorAngle);
            UnityEditor.Handles.DrawLine(position, position + leftBound);
            UnityEditor.Handles.DrawWireArc(position, Vector3.forward, rightBound, _sectorAngle, 1);

            UnityEditor.Handles.color = new Color(1f, 1f, 1f, 0.2f);
            UnityEditor.Handles.DrawSolidArc(position, Vector3.forward, rightBound, _sectorAngle, 1);
        }
#endif
        
        private Vector3 GetUnitOnCircle(float angleDegrees)
        {
            var angleRadians = angleDegrees * Mathf.PI / 180.0f;

            var x = Mathf.Cos(angleRadians);
            var y = Mathf.Sin(angleRadians);

            return new Vector3(x, y, 0);
        }

        [Serializable]
        public class Loot
        {
            [Range(0, 100)] public float Chance;
            public GameObject Prefab;
            public int Count = 1;
        }
    }
}