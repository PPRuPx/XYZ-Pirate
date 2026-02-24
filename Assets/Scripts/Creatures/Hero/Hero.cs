using System.Collections;
using Components;
using Components.ColliderBased;
using Components.Effects.CameraRelated;
using Components.GameObjectBased;
using Components.Health;
using Model;
using Model.Definitions;
using Model.Definitions.Player;
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

        [Space] [Header("Special Tactics")] 
        [SerializeField] private Cooldown _throwCooldown;
        [SerializeField] private Cooldown _superThrowCooldown;
        [SerializeField] private int _superThrowParticles;
        [SerializeField] private float _superThrowDelay;
        [SerializeField] private SpawnComponent _throwSpawner;
        [SerializeField] private ForceShieldComponent _forceShield;
        [SerializeField] private LightControllerComponent _candle;
        [SerializeField] private float _dashVelocity = 30f;
        [SerializeField] private float _dashDuration = 0.2f;
    
        [Space] [Header("Interaction")] 
        [SerializeField] private CheckCircleOverlap _interactionCheck;

        [Space] [Header("Animations")] 
        [SerializeField] private RuntimeAnimatorController _unarmed;
        [SerializeField] private RuntimeAnimatorController _armed;

        [Space] [Header("Particles")] 
        [SerializeField] private ParticleSystem _hitParticles;
        [SerializeField] private ParticleSystem _critParticles;

        private bool _allowDoubleJump;
        private bool _isOnWall;
        private bool _allowThrowSword;
        private bool _superThrow;
        private bool _lightIsOn;
        private bool _isDashing;

        private float _defaultJumpSpeed;
        private float _defaultGravityScale;
        private float _dashDirection;

        private readonly Collider2D[] _interactionResult = new Collider2D[1];

        private GameSession _session;
        private CameraShakeEffect _cameraShake;
    
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
        
        // Shortcuts
        private int MaxHp => (int) _session.StatsModel.GetValue(StatId.Hp);
        private int CurrentHp => _session.Data.Hp.Value;
        private int MissingHp => MaxHp - CurrentHp;
        
        private float LightMaxCapacity => _session.StatsModel.GetValue(StatId.LightTime);

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
            _session.StatsModel.OnUpgraded += OnHeroUpgraded;
            _session.Data.Light.Value = LightMaxCapacity;

            _cameraShake = FindObjectOfType<CameraShakeEffect>();
            
            HealthComponent.SetHealth(_session.Data.Hp.Value);
            UpdateHeroWeapon();
        }
        
        private void OnHeroUpgraded(StatId statId)
        {
            switch (statId)
            {
                case StatId.Hp:
                    var health = (int) _session.StatsModel.GetValue(statId);
                    _session.Data.Hp.Value = health;
                    HealthComponent.SetHealth(health);
                    break;
            }
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
            
            if (_session.PerksModel.IsRegenerationSupported && MissingHp > 0)
            {
                HealthComponent.ModifyHealth(1);
                _session.PerksModel.Cooldown.Reset();
            }

            CalculateLight();
        }

        private void CalculateLight()
        {
            if (_lightIsOn && _session.Data.Light.Value > 0f)
            {
                _session.Data.Light.Value -= Time.deltaTime;
                _session.Data.Light.Value = Mathf.Clamp(_session.Data.Light.Value, 0f, LightMaxCapacity);

                float lightCapacityRatio = _session.Data.Light.Value / LightMaxCapacity;
                if (lightCapacityRatio < 0.1)
                    _candle.SetIntensityRatio(lightCapacityRatio * 10);
                else
                    _candle.SetIntensityRatio(1);
                
                if (_session.Data.Light.Value <= 0f)
                {
                    _lightIsOn = false;
                    _candle.gameObject.SetActive(false);
                }
            }
            
            if (!_lightIsOn && _session.Data.Light.Value < LightMaxCapacity)
                _session.Data.Light.Value += Time.deltaTime;
        }

        protected override float CalculateSpeed() =>
            _session.StatsModel.GetValue(StatId.Speed);
        
        protected override float CalculateXVelocity()
        {
            if (_isDashing)
                return _dashDirection * _dashVelocity;
            
            return base.CalculateXVelocity();
        }

        protected override float CalculateYVelocity()
        {
            if (_isDashing)
                return 0f;

            var isJumpPressing = Direction.y > 0;

            if (IsGrounded || _isOnWall)
                _allowDoubleJump = true;

            if (!isJumpPressing && _isOnWall)
                return 0f;

            return base.CalculateYVelocity();
        }

        protected override float CalculateJumpVelocity(float yVelocity)
        {
            if (!IsGrounded && _allowDoubleJump && _session.PerksModel.IsDoubleJumpSupported && !_isOnWall)
            {
                _session.PerksModel.Cooldown.Reset();
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
            _cameraShake?.Shake();
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
                var potionHealAmount= DefsFacade.I.HealPotion.HealAmount;
                HealthComponent.ModifyHealth(Mathf.Min(MissingHp, potionHealAmount));
                _particles.Spawn("PotionEffect");
            }
        }
        
        public void UseRecoveryPotion()
        {
            if (RecoveryPotionCount > 0)
            {
                _session.Data.Inventory.Remove(RecoveryPotionId, 1);
                HealthComponent.ModifyHealth(MissingHp);
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

            var meleeDamage = _attackDamage;
            meleeDamage = ModifyDamageByCrit(meleeDamage);
            _attackRange.GetComponent<HealthChangeComponent>().setValue(-meleeDamage);

            base.Attack();
        }

        private void UpdateHeroWeapon() =>
            Animator.runtimeAnimatorController = SwordCount > 0 ? _armed : _unarmed;
    
        public void OnDoThrow()
        {
            if (_superThrow && _session.PerksModel.IsSuperThrowSupported)
            {
                var throwableCount = _session.Data.Inventory.Count(SelectedItemId);
                var possibleCount = SelectedItemId == SwordId ? throwableCount - 1 : throwableCount;
                var numThrows = Mathf.Min(_superThrowParticles, possibleCount);
                _session.PerksModel.Cooldown.Reset();
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
            var instance = _throwSpawner.SpawnInstance();
            
            var rangeDamage = (int) _session.StatsModel.GetValue(StatId.RangeDamage);
            rangeDamage = ModifyDamageByCrit(rangeDamage);
            instance.GetComponent<HealthChangeComponent>().setValue(-rangeDamage);

            _session.Data.Inventory.Remove(throwableId, 1);
        }

        private int ModifyDamageByCrit(int damage)
        {
            var critChance = (int)_session.StatsModel.GetValue(StatId.CritChance);
            if (Random.value * 100 <= critChance)
            {
                SpawnCritParticle();
                damage *= 2;
            }

            return damage;
        }
        
        private void SpawnCritParticle()
        {
            _critParticles.gameObject.SetActive(true);
            _critParticles.Play();
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

        public void UsePerk()
        {
            if (_session.PerksModel.IsForceShieldSupported)
            {
                _forceShield.Use();
                _session.PerksModel.Cooldown.Reset();
            }
        }

        public void NextItem() =>
            _session.QuickInventory.SetNextItem();
        
        public void AddInInventory(string id, int value) =>
            _session.Data.Inventory.Add(id, value);

        public void UseLight()
        {
            _lightIsOn = !_lightIsOn;
            _candle.gameObject.SetActive(_lightIsOn);
        }
        
        public void Dash(float directionX)
        {
            if (_isDashing || directionX == 0) 
                return;
            
            if (!_session.PerksModel.IsDashSupported)
                return;
            
            StartCoroutine(DoDash(directionX));
        }

        private IEnumerator DoDash(float directionX)
        {
            _session.PerksModel.Cooldown.Reset();
            
            _isDashing = true;
            _dashDirection = Mathf.Sign(directionX);

            var originalGravity = Rigidbody.gravityScale;
            Rigidbody.gravityScale = 0;

            yield return new WaitForSeconds(_dashDuration);

            Rigidbody.gravityScale = originalGravity;
            _isDashing = false;
        }
        
    }
}