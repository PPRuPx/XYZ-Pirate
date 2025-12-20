using System;
using Components.ColliderBased;
using Components.GameObjectBased;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Creatures.Mobs
{
    public class SeashellAI : MonoBehaviour
    {
        [SerializeField] private LayerCheck _vision;

        [Header("Melee")] [Space] 
        [SerializeField] private Cooldown _meleeCooldown;

        [SerializeField] private CheckCircleOverlap _meleeAttack;
        [SerializeField] private LayerCheck _meleeCanAttack;

        [Header("Range")] [Space] 
        [SerializeField]private Cooldown _rangeCooldown;
        [SerializeField] private SpawnComponent _rangeAttack;

        private Animator _animator;
        
        private static readonly int MeleeKey = Animator.StringToHash("melee");
        private static readonly int RangeKey = Animator.StringToHash("range");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
        
        private void Update()
        {
            if (_vision.IsTouchingLayers)
            {
                if (_meleeCanAttack && _meleeCanAttack.IsTouchingLayers)
                {
                    if (_meleeCooldown.IsReady)
                        MeleeAttack();
                    return;
                }

                if (_rangeCooldown.IsReady)
                    RangeAttack();
            }
        }

        private void MeleeAttack()
        {
            _meleeCooldown.Reset();
            _animator.SetTrigger(MeleeKey);
        }

        private void RangeAttack()
        {
            _rangeCooldown.Reset();
            _animator.SetTrigger(RangeKey);
        }

        private void OnMeleeAttack()
        {
            _meleeAttack.Check();
        }

        private void OnRangeAttack()
        {
            _rangeAttack.Spawn();
        }

    }
}