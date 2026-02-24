using System;
using Components;
using UnityEngine;

namespace Creatures.Mobs.Boss.DaddyShark
{
    public class DaddyShark : Creature
    {
        [Header("DaddyShark Specials")] 
        [SerializeField] protected int _stage2Hp;
        [SerializeField] protected int _stage3Hp;
        [SerializeField] protected ForceShieldComponent _forceShield;
        [SerializeField] protected TargetProjectileSpawner _targetProjectileSpawner;
        [SerializeField] protected QuakeController _quakeController;

        private bool isStage1 = true;
        private bool isStage2 = false;
        private bool isStage3 = false;

        public bool IsStage1 => isStage1;
        public bool IsStage2 => isStage2;
        public bool IsStage3 => isStage3;
        
        public event Action OnTakeDamage;

        protected static readonly int AgroKey = Animator.StringToHash("agro");

        public override void TakeDamage()
        {
            base.TakeDamage();

            if (isStage1 && HealthComponent.Health <= _stage2Hp)
            {
                isStage1 = false; 
                isStage2 = true;
                ActivateAgroVisuals();
            }
            else if (isStage2 && HealthComponent.Health <= _stage3Hp)
            {
                isStage2 = false; 
                isStage3 = true;
                ActivateAgroVisuals();
            }

            OnTakeDamage?.Invoke();
        }

        public void ActivateAgroVisuals()
        {
            if (_forceShield != null) 
                _forceShield.Use();
            Animator.SetTrigger(AgroKey);
        }

        public void Shoot() => _targetProjectileSpawner.LaunchProjectiles();
        public void Quake() => _quakeController.ActivateQuake();
    }
}