using System.Collections;
using Components;
using Components.Collectables;
using Components.ColliderBased;
using Model.State;
using UnityEditor.Animations;
using UnityEngine;
using Utils;

namespace Creatures.Hero
{
    public class Hero : Creature
    {
        [Space] [Header("Special Movement")] 
        [SerializeField] private float _slamDownVelocity;
        [SerializeField] private LayerCheck _wallCheck;
        [SerializeField] private float _wallGravityScale;
        [SerializeField] protected float _invulOnHitTime;

        [Space] [Header("Special Attack")] 
        [SerializeField] private Cooldown _throwCooldown;
        [SerializeField] private Cooldown _superThrowCooldown;
        [SerializeField] private int _superThrowParticles;
        [SerializeField] private float _superThrowDelay;
    
        [Space] [Header("Interaction")] 
        [SerializeField] private CheckCircleOverlap _interactionCheck;

        [Space] [Header("Animations")] 
        [SerializeField] private AnimatorController _unarmed;
        [SerializeField] private AnimatorController _armed;

        [Space] [Header("Particles")] 
        [SerializeField] private ParticleSystem _hitParticles;

        private CoinsComponent _coinsComponent;
        private SwordsComponent _swordsComponent;

        private bool _allowDoubleJump;
        private bool _isOnWall;
        private bool _allowThrowSword;
        private bool _superThrow;

        private float _defaultJumpSpeed;
        private float _defaultGravityScale;

        private readonly Collider2D[] _interactionResult = new Collider2D[1];

        private GameSession _session;
    
        private static readonly int ThrowKey = Animator.StringToHash("throw");

        protected override void Awake()
        {
            base.Awake();
        
            _coinsComponent = GetComponent<CoinsComponent>();
            _swordsComponent = GetComponent<SwordsComponent>();

            _defaultJumpSpeed = _jumpSpeed;
            _defaultGravityScale = Rigidbody.gravityScale;
        }

        private void Start()
        {
            _session = FindObjectOfType<GameSession>();

            HealthComponent.SetHealth(_session.Data.Hp);
            _coinsComponent.SetCoins(_session.Data.Coins);
            _swordsComponent.SetSwords(_session.Data.Swords);

            UpdateHeroWeapon();
        }

        public void OnHealthChanged(int currentHealth) =>
            _session.Data.Hp = currentHealth;

        public void OnCoinsChanged(int currentCoins) =>
            _session.Data.Coins = currentCoins;

        protected override void Update()
        {
            base.Update();

            if (_wallCheck.IsTouchingLayers && Direction.x == transform.localScale.x)
            {
                _isOnWall = true;
                Rigidbody.gravityScale = _wallGravityScale;
            }
            else
            {
                _isOnWall = false;
                Rigidbody.gravityScale = _defaultGravityScale;
            }
        }

        protected override float CalculateYVelocity()
        {
            var isJumpPressing = Direction.y > 0;

            if (IsGrounded || _isOnWall)
                _allowDoubleJump = true;

            if (!isJumpPressing && _isOnWall)
                return 0f;

            return base.CalculateYVelocity();
        }

        protected override float CalculateJumpVelocity(float yVelocity)
        {
            if (!IsGrounded && _allowDoubleJump)
            {
                _particles.Spawn("Jump");
                _allowDoubleJump = false;
                IsJumpLocked = true;
                Invoke(nameof(UnlockJump), _jumpLockTime);
                return _jumpSpeed;
            }

            return base.CalculateJumpVelocity(yVelocity);
        }

        public override void TakeDamage()
        {
            base.TakeDamage();
            if (_coinsComponent.Coins() > 0)
                SpawnCoins();
        }

        private void SpawnCoins()
        {
            var numCoinsToDispose = Mathf.Min(_session.Data.Coins, 5);
            _coinsComponent.ModifyCoins(-numCoinsToDispose);
            _session.Data.Coins = _coinsComponent.Coins();

            var burst = _hitParticles.emission.GetBurst(0);
            burst.count = numCoinsToDispose;
            _hitParticles.emission.SetBurst(0, burst);
        
            _hitParticles.gameObject.SetActive(true);
            _hitParticles.Play();
        }

        public void ApplyJumpPowerBuff(float multiplier, float time)
        {
            _jumpSpeed *= multiplier;
            Invoke(nameof(ResetBuff), time);
        }

        private void ResetBuff() =>
            _jumpSpeed = _defaultJumpSpeed;

        public void Interact()
        {
            _interactionCheck.Check();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_groundCheck.IsTouchingLayers)
            {
                var contact = other.contacts[0];
                if (contact.relativeVelocity.y >= _slamDownVelocity)
                    _particles.Spawn("SlamDown");
            }
        }
    
        public override void Attack()
        {
            if (_session.Data.Swords < 1)
                return;

            base.Attack();
        }

        public void ArmHero()
        {
            _session.Data.Swords += 1;
            UpdateHeroWeapon();
        }

        private void UpdateHeroWeapon() =>
            Animator.runtimeAnimatorController = _session.Data.Swords > 0 ? _armed : _unarmed;

        public void StartThrowing()
        {
            _superThrowCooldown.Reset();
        }

        public void PerformThrowing()
        {
            if (_session.Data.Swords <= 1)
                return;

            _superThrow = _superThrowCooldown.IsReady;
        
            Animator.SetTrigger(ThrowKey);
            _throwCooldown.Reset();
        }
    
        public void OnDoThrow()
        {
            if (_superThrow)
            {
                var numThrows = Mathf.Min(_superThrowParticles, _session.Data.Swords - 1);
                StartCoroutine(DoSuperThrow(numThrows));
            }
            else
            {
                ThrowAndRemoveFromInventory();
            }

            _superThrow = false;
        }

        private void ThrowAndRemoveFromInventory()
        {
            _particles.Spawn("Throw");
            _session.Data.Swords--;
        }

        private IEnumerator DoSuperThrow(int numThrows)
        {
            for (int i = 0; i < numThrows; i++)
            {
                ThrowAndRemoveFromInventory();
                yield return new WaitForSeconds(_superThrowDelay);
            }
        }
    }
}