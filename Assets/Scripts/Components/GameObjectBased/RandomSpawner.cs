using System.Collections;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Components.GameObjectBased
{
    public class RandomSpawner : MonoBehaviour
    {
        [Header("Spawn bound")] [Space]
        [SerializeField] private float _sectorAngle = 60;
        [SerializeField] private float _sectorRotation;

        [Header("Spawn params")] [Space] 
        [SerializeField] private float _waitTime;
        [SerializeField] private float _speed;

        private Coroutine _coroutine;

        public void StartDrop(GameObject[] items)
        {
            TryStopCoroutine();

            _coroutine = StartCoroutine(StartSpawn(items));
        }

        public void DropImmediate(GameObject[] items)
        {
            foreach (var item in items)
            {
                Spawn(item);
            }
        }

        private IEnumerator StartSpawn(GameObject[] particles)
        {
            for (var i = 0; i < particles.Length; i++)
            {
                Spawn(particles[i]);
                yield return new WaitForSeconds(_waitTime);
            }
        }

        private void Spawn(GameObject particle)
        {
            var instance = SpawnUtils.Spawn(particle, transform.position);
            var rigidBody = instance.GetComponent<Rigidbody2D>();

            var randomAngle = Random.Range(0, _sectorAngle);
            var forceVector = AngleToVectorInSector(randomAngle);
            rigidBody.AddForce(forceVector * _speed, ForceMode2D.Impulse);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var position = transform.position;

            var middleAngleDelta = (180 - _sectorRotation - _sectorAngle) / 2;
            var rightBound = GetUnitOnCircle(middleAngleDelta);
            UnityEditor.Handles.DrawLine(position, position + rightBound);

            var leftBound = GetUnitOnCircle(middleAngleDelta + _sectorAngle);
            UnityEditor.Handles.DrawLine(position, position + leftBound);
            UnityEditor.Handles.DrawWireArc(position, Vector3.forward, rightBound, _sectorAngle, 1);

            UnityEditor.Handles.color = new Color(1f, 1f, 1f, 0.1f);
            UnityEditor.Handles.DrawSolidArc(position, Vector3.forward, rightBound, _sectorAngle, 1);
        }
#endif

        private Vector2 AngleToVectorInSector(float angle)
        {
            var angleMiddleDelta = (180 - _sectorRotation - _sectorAngle) / 2;
            return GetUnitOnCircle(angle + angleMiddleDelta);
        }

        private Vector3 GetUnitOnCircle(float angleDegrees)
        {
            var angleRadians = angleDegrees * Mathf.PI / 180.0f;

            var x = Mathf.Cos(angleRadians);
            var y = Mathf.Sin(angleRadians);

            return new Vector3(x, y, 0);
        }

        private void OnDisable()
        {
            TryStopCoroutine();
        }

        private void OnDestroy()
        {
            TryStopCoroutine();
        }

        private void TryStopCoroutine()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
        }
    }
}