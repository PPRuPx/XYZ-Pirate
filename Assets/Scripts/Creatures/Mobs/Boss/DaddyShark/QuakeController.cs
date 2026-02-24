using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Components.Effects.CameraRelated;
using UnityEngine.Experimental.Rendering.Universal;

namespace Creatures.Mobs.Boss.DaddyShark
{
    public class QuakeController : MonoBehaviour
    {
        [Header("Objects")]
        [SerializeField] private GameObject _blockPrefab;
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private Light2D[] _safetyLights;

        [Header("Timings")]
        [SerializeField] private float _warningDuration = 1.2f;
        [SerializeField] private float _maxFallDelay = 0.4f;

        [Header("Visuals")]
        [SerializeField] private QuakeEffect _quakeEffect;
        [ColorUsage(true, true)]
        [SerializeField] private Color _safeColor = Color.green;
        [ColorUsage(true, true)]
        [SerializeField] private Color _dangerColor = Color.red;
        [SerializeField] private float _idleIntensity = 0f;
        [SerializeField] private float _activeIntensity = 1.5f;

        private readonly int[][] safeGroups = new int[][]
        {
            new int[] { 2, 3, 4 },
            new int[] { 6, 7, 8, 9 },
            new int[] { 12, 13, 14, 15 },
            new int[] { 17, 18, 19 }
        };

        private void Start()
        {
            ResetLights();
        }

        [ContextMenu("Activate Quake")]
        public void ActivateQuake()
        {
            _quakeEffect.Shake();
            StartCoroutine(QuakeRoutine());
        }

        private IEnumerator QuakeRoutine()
        {
            HashSet<int> safeIndices = new HashSet<int>();
            foreach (var group in safeGroups)
                safeIndices.Add(group[Random.Range(0, group.Length)]);

            for (int i = 0; i < _safetyLights.Length; i++)
            {
                if (_safetyLights[i] == null) continue;
                _safetyLights[i].color = safeIndices.Contains(i) ? Color.green : Color.red;
                _safetyLights[i].intensity = 1.5f;
            }

            yield return new WaitForSeconds(_warningDuration);

            for (int i = 0; i < _spawnPoints.Length; i++)
            {
                if (!safeIndices.Contains(i))
                    StartCoroutine(SpawnWithDelay(_spawnPoints[i].position));
            }

            yield return new WaitForSeconds(1f);
            foreach (var light in _safetyLights) if (light != null) light.intensity = 0.2f;
        }

        private IEnumerator SpawnWithDelay(Vector3 pos)
        {
            yield return new WaitForSeconds(Random.Range(0f, _maxFallDelay));
            Instantiate(_blockPrefab, pos, Quaternion.identity);
        }

        private void ResetLights()
        {
            foreach (var light in _safetyLights)
            {
                if (light != null)
                {
                    light.color = Color.white;
                    light.intensity = _idleIntensity;
                }
            }
        }
    }
}