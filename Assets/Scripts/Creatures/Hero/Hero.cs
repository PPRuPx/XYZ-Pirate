using System.Collections;
using Components.ColliderBased;
using Components.GameObjectBased;
using Model.Definitions;
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
        [SerializeField] private SpawnComponent _throwSpawner;
    
        [Space] [Header("Interaction")] 
        [SerializeField] private CheckCircleOverlap _interactionCheck;

        [Space] [Header("Animations")] 
        [SerializeField] private AnimatorController _unarmed;
        [SerializeField] private AnimatorController _armed;

        [Space] [Header("Particles")] 
        [SerializeField] private ParticleSystem _hitParticles;

        private bool _allowDoubleJump;
        private bool _isOnWall;
        private bool _allowThrowSword;
        private bool _superThrow;

        private float _defaultJumpSpeed;
        private float _defaultGravityScale;

        private readonly Collider2D[] _interactionResult = new Collider2D[1];

        private GameSession _session;
    
        private static readonly int ThrowKey = Animator.StringToHash("throw");
        private static readonly int IsOnWallKey = Animator.StringToHash("is-on-wall");

        private const string CoinId = "Coin";
        private const string SwordId = "Sword";
        private const string HealPotionId = "Heal Potion";
        private const string RecoveryPotionId = "Recovery Potion";
        private const string JumpPotionId = "Jump Potion";
        
        private int CoinCount => _session.Data.Inventory.Count(CoinId);
        private int SwordCount => _session.Data.Inventory.Count(SwordId);
        private int HealPotionCount => _session.Data.Inventory.Count(HealPotionId);
        private int RecoveryPotionCount => _session.Data.Inventory.Count(RecoveryPotionId);
        private int JumpPotionCount => _session.Data.Inventory.Count(JumpPotionId);

        private string SelectedItemId => _session.QuickInventory.SelectedItem.Id;
        
        private bool CanThrow
        {
            get
            {
                if (SelectedItemId == SwordId)
                    return SwordCount > 1;
                
                var def = DefsFacade.I.Items.Get(SelectedItemId);
                return def.HasTag(ItemTag.Throwable);
            }
        }
        
        protected override void Awake()
        {
            base.Awake();

            _defaultJumpSpeed = _jumpSpeed;
            _defaultGravityScale = Rigidbody.gravityScale;
        }

        private void Start()
        {
            _session = FindObjectOfType<GameSession>();
            _session.Data.Inventory.OnChanged += OnInventoryChanged;
            
            HealthComponent.SetHealth(_session.Data.Hp.Value);
            UpdateHeroWeapon();
        }

        private void OnDestroy()
        {
            _session.Data.Inventory.OnChanged -= OnInventoryChanged;
        }

        private void OnInventoryChanged(string id, int value)
        {
            if (id == SwordId)
                UpdateHeroWeapon();
        }

        public void OnHealthChanged(int currentHealth) =>
            _session.Data.Hp.Value = currentHealth;

        protected override void Update()
        {
            base.Update();

            var moveToSameDirection = Direction.x * transform.lossyScale.x > 0;
            if (_wallCheck.IsTouchingLayers && moveToSameDirection)
            {
                _isOnWall = true;
                Rigidbody.gravityScale = _wallGravityScale;
            }
            else
            {
                _isOnWall = false;
                Rigidbody.gravityScale = _defaultGravityScale;
            }
            
            Animator.SetBool(IsOnWallKey, _isOnWall);
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
                Sounds.Play("Jump");
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
            if (CoinCount > 0)
                SpawnCoins();
        }

        private void SpawnCoins()
        {
            var numCoinsToDispose = Mathf.Min(CoinCount, 5);
            _session.Data.Inventory.Remove(CoinId, numCoinsToDispose);

            var burst = _hitParticles.emission.GetBurst(0);
            burst.count = numCoinsToDispose;
            _hitParticles.emission.SetBurst(0, burst);
        
            _hitParticles.gameObject.SetActive(true);
            _hitParticles.Play();
        }

        public void UseHealPotion()
        {
            if (HealPotionCount > 0)
            {
                _session.Data.Inventory.Remove(HealPotionId, 1);
                HealthComponent.ModifyHealth(
                    DefsFacade.I.HealPotion.HealAmount);
                _particles.Spawn("PotionEffect");
            }
        }
        
        public void UseRecoveryPotion()
        {
            if (RecoveryPotionCount > 0)
            {
                _session.Data.Inventory.Remove(RecoveryPotionId, 1);
                var healAmount = DefsFacade.I.Player.MaxHealth - _session.Data.Hp.Value;
                HealthComponent.ModifyHealth(Mathf.Max(0, healAmount));
                _particles.Spawn("PotionEffect");
            }
        }
        
        public void UseJumpPotion()
        {
            if (JumpPotionCount > 0)
            {
                _session.Data.Inventory.Remove(JumpPotionId, 1);
                ApplyJumpPowerBuff(
                    DefsFacade.I.JumpPotion.Multiplier, 
                    DefsFacade.I.JumpPotion.Duration);
                _particles.Spawn("PotionEffect");
            }
        }
        
        public void UseInventoryItem()
        {
            switch (SelectedItemId)
            {
                case HealPotionId:
                {
                    UseHealPotion();
                    break;
                }
                case RecoveryPotionId:
                {
                    UseRecoveryPotion();
                    break;
                }
                case JumpPotionId:
                {
                    UseJumpPotion();
                    break;
                }
                default:
                    return;
            }
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
            if (SwordCount <= 0)
                return;

            base.Attack();
        }

        private void UpdateHeroWeapon() =>
            Animator.runtimeAnimatorController = SwordCount > 0 ? _armed : _unarmed;
    
        public void OnDoThrow()
        {
            if (_superThrow)
            {
                var throwableCount = _session.Data.Inventory.Count(SelectedItemId);
                var possibleCount = SelectedItemId == SwordId ? throwableCount - 1 : throwableCount;
                var numThrows = Mathf.Min(_superThrowParticles, possibleCount);
                StartCoroutine(DoSuperThrow(numThrows));
            }
            else
            {
                ThrowAndRemoveFromInventory();
            }

            _superThrow = false;
        }
        
        private IEnumerator DoSuperThrow(int numThrows)
        {
            for (int i = 0; i < numThrows; i++)
            {
                ThrowAndRemoveFromInventory();
                yield return new WaitForSeconds(_superThrowDelay);
            }
        }

        private void ThrowAndRemoveFromInventory()
        {
            Sounds.Play("Range");

            var throwableId = _session.QuickInventory.SelectedItem.Id;
            var throwableDef = DefsFacade.I.Throwables.Get(throwableId);
            _throwSpawner.SetPrefab(throwableDef.Projectile);
            _throwSpawner.Spawn();

            _session.Data.Inventory.Remove(throwableId, 1);
        }

        public void StartThrowing()
        {
            _superThrowCooldown.Reset();
        }
        
        public void PerformThrowing()
        {
            if (!_throwCooldown.IsReady || !CanThrow) return;

            if (_superThrowCooldown.IsReady) _superThrow = true;

            Animator.SetTrigger(ThrowKey);
            _throwCooldown.Reset();
        }

        public void NextItem() =>
            _session.QuickInventory.SetNextItem();
        
        public void AddInInventory(string id, int value) =>
            _session.Data.Inventory.Add(id, value);
    }
}