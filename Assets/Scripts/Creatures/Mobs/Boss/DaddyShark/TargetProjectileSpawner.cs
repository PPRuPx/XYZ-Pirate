using System;
using System.Collections;
using Creatures.Weapons;
using UnityEngine;
using Utils;

namespace Creatures.Mobs.Boss.DaddyShark
{
    public class TargetProjectileSpawner : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private DirectionalProjectile _prefab;
        [SerializeField] private int _burstCount;
        [SerializeField] private float _delay;

        private bool _isShooting;

        [ContextMenu("Launch!")]
        public void LaunchProjectiles()
        {
            if (_target == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) _target = player.transform;
            }

            if (!_isShooting) 
                StartCoroutine(SpawnProjectiles());
        }

        private IEnumerator SpawnProjectiles()
        {
            if (_target == null) yield break;

            _isShooting = true;
            for (int i = 0; i < _burstCount; i++)
            {
                Vector2 direction = (_target.position - transform.position).normalized;

                var instance = SpawnUtils.Spawn(_prefab.gameObject, transform.position);
                var projectile = instance.GetComponent<DirectionalProjectile>();
                
                projectile.Launch(direction);

                yield return new WaitForSeconds(_delay);
            }
            _isShooting = false;
        }
    }
    
}