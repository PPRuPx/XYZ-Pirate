using Components.Audio;
using Components.ColliderBased;
using Components.GameObjectBased;
using Components.Health;
using UnityEngine;

namespace Creatures
{
    public class Creature : MonoBehaviour
    {
        [SerializeField] private bool _invertScale;
        [SerializeField] private float _sizeMultiplier = 1;
        
        [Space] [Header("Movement")] 
        [SerializeField] protected float _moveSpeed;
        [SerializeField] protected float _jumpSpeed;
        [SerializeField] protected float _damageVelocity;
        [SerializeField] protected LayerCheck _groundCheck;
        [SerializeField] protected float _jumpLockTime;

        [Space] [Header("Attack")] 
        [SerializeField] protected int _attackDamage;
        [SerializeField] protected CheckCircleOverlap _attackRange;

        [Space] [Header("Animation Particles")] 
        [SerializeField] protected SpawnListComponent _particles;

        protected Rigidbody2D Rigidbody;
        protected Animator Animator;
        protected HealthComponent HealthComponent;
        protected PlaySoundComponent Sounds;

        protected Vector3 Direction;
        protected bool IsGrounded;
        protected bool IsJumping;
        protected bool IsJumpLocked;

        protected static readonly int IsRunningKey = Animator.StringToHash("is-running");
        protected static readonly int IsGroundedKey = Animator.StringToHash("is-grounded");
        protected static readonly int VerticalVelocityKey = Animator.StringToHash("vertical-velocity");
        protected static readonly int HitKey = Animator.StringToHash("hit");
        protected static readonly int AttackKey = Animator.StringToHash("attack");

        protected virtual void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
            HealthComponent = GetComponent<HealthComponent>();
            Sounds = GetComponent<PlaySoundComponent>();
        }

        public void SetDirection(Vector3 direction) =>
            Direction = direction;

        protected virtual void Update()
        {
            IsGrounded = _groundCheck.IsTouchingLayers;
        }

        private void FixedUpdate()
        {
            var xVelocity = CalculateXVelocity();
            var yVelocity = CalculateYVelocity();
            Rigidbody.velocity = new Vector2(xVelocity, yVelocity);

            Animator.SetBool(IsRunningKey, Direction.x != 0);
            Animator.SetBool(IsGroundedKey, IsGrounded);
            Animator.SetFloat(VerticalVelocityKey, Rigidbody.velocity.y);

            UpdateSpriteDirection(Direction);
        }

        protected virtual float CalculateXVelocity()
        {
            return Direction.x * CalculateSpeed();
        }
        
        protected virtual float CalculateSpeed()
        {
            return _moveSpeed;
        }

        protected virtual float CalculateYVelocity()
        {
            var yVelocity = Rigidbody.velocity.y;
            var isJumpPressing = Direction.y > 0;

            if (IsGrounded)
                IsJumping = false;

            if (IsJumpLocked)
                return yVelocity;

            if (isJumpPressing)
            {
                IsJumping = true;

                var isFalling = Rigidbody.velocity.y <= 0.001f;
                yVelocity = isFalling ? CalculateJumpVelocity(yVelocity) : yVelocity;
            }
            else if (Rigidbody.velocity.y > 0)
            {
                yVelocity *= 0.5f;
            }

            return yVelocity;
        }

        protected virtual float CalculateJumpVelocity(float yVelocity)
        {
            if (IsGrounded)
            {
                _particles?.Spawn("Jump");
                Sounds.Play("Jump");
                IsJumpLocked = true;
                Invoke(nameof(UnlockJump), _jumpLockTime);
                return _jumpSpeed;
            }

            return yVelocity;
        }

        protected void UnlockJump() =>
            IsJumpLocked = false;

        public void UpdateSpriteDirection(Vector2 direction)
        {
            var multiplier = _invertScale ? -1 : 1;
            if (direction.x > 0)
                transform.localScale = new Vector3(multiplier * _sizeMultiplier, 1 * _sizeMultiplier, 1);
            else if (direction.x < 0)
                transform.localScale = new Vector3(-1 * multiplier * _sizeMultiplier, 1 * _sizeMultiplier, 1);
        }

        public virtual void TakeDamage()
        {
            if (HealthComponent.IsInvulnerable())
                return;

            Sounds.Play("Hurt");
            IsJumpLocked = true;
            Animator.SetTrigger(HitKey);
            Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, 0);
            Rigidbody.AddForce(Vector2.up * _damageVelocity, ForceMode2D.Impulse);
            Invoke(nameof(UnlockJump), _jumpLockTime);
        }

        public virtual void Attack()
        {
            Animator.SetTrigger(AttackKey);
            Sounds.Play("Melee");
        }

        public void OnDoAttack()
        {
            _attackRange.Check();
            _particles?.Spawn("Slash");
        }
    }
}