using System;
using Components;
using DefaultNamespace;
using DefaultNamespace.Utils;
using Model.State;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Serialization;

public class Hero : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _jumpSpeed;
    [SerializeField] private LayerCheck _groundCheck;
    [SerializeField] private float _hardLandSpeedThreshold;

    [SerializeField] private float _damageJumpSpeed;
    [SerializeField] private float _damageJumpLockTime;

    [SerializeField] private float _interactionRadius;
    [SerializeField] private LayerMask _interactionLayer;

    [SerializeField] private SpawnComponent _runParticles;
    [SerializeField] private SpawnComponent _jumpParticles;
    [SerializeField] private SpawnComponent _fallParticles;
    [SerializeField] private SpawnComponent _attackParticles;

    [SerializeField] private AnimatorController _unarmed;
    [SerializeField] private AnimatorController _armed;
    
    [SerializeField] private int _attackDamage;
    [SerializeField] private CheckCircleOverlap _attackRange;

    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private HealthComponent _health;
    private CoinsComponent _coins;
    
    private Vector3 _directrion;
    private bool _wasGrounded;
    private bool _isGrounded;
    private bool _allowDoubleJump;
    private bool _isJumpLocked;
    private float _fallVelocity;
    private float _currentJumpSpeed;

    private Collider2D[] _interactionResult = new Collider2D[1];

    private static readonly int IsRunningKey = Animator.StringToHash("is-running");
    private static readonly int IsGroundedKey = Animator.StringToHash("is-grounded");
    private static readonly int VerticalVelocityKey = Animator.StringToHash("vertical-velocity");
    private static readonly int HitKey = Animator.StringToHash("hit");
    private static readonly int AttackKey = Animator.StringToHash("attack");

    private GameSession _session;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<HealthComponent>();
        _coins = GetComponent<CoinsComponent>();
    }

    private void Start()
    {
        _session = FindObjectOfType<GameSession>();
        
        _currentJumpSpeed = _jumpSpeed;
        _health.SetHealth(_session.Data.Hp);
        _coins.SetCoins(_session.Data.Coins);
        UpdateHeroWeapon();
    }
    
    public void OnHealthChanged(int currentHealth) =>
        _session.Data.Hp = currentHealth;
    
    public void OnCoinsChanged(int currentCoins) =>
        _session.Data.Coins = currentCoins;

    public void SetDirection(Vector3 direction)
    {
        _directrion = direction;
    }
    
    private void Update()
    {
        _wasGrounded = _isGrounded;
        _isGrounded = _groundCheck.IsTouchingLayer;

        if (!_wasGrounded && _isGrounded)
            SpawnFallParticle();

        if (!_isGrounded && _rigidbody.velocity.y < 0)
            _fallVelocity = _rigidbody.velocity.y;
        else if (_isGrounded)
            _fallVelocity = 0;
    }

    private void FixedUpdate()
    {
        var xVelocity = _directrion.x * _moveSpeed;
        var yVelocity = CalculateYVelocity();
        _rigidbody.velocity = new Vector2(xVelocity, yVelocity);

        _animator.SetBool(IsRunningKey, _directrion.x != 0);
        _animator.SetBool(IsGroundedKey, _isGrounded);
        _animator.SetFloat(VerticalVelocityKey, _rigidbody.velocity.y);

        UpdateSpriteDirection();
    }

    private float CalculateYVelocity()
    {
        var yVelocity = _rigidbody.velocity.y;
        var isJumpPressing = _directrion.y > 0;

        if (_isGrounded)
            _allowDoubleJump = true;

        if (_isJumpLocked)
            return yVelocity;

        if (isJumpPressing)
        {
            yVelocity = CalculateJumpVelocity(yVelocity);
        }
        else if (_rigidbody.velocity.y > 0)
        {
            yVelocity *= 0.5f;
        }

        return yVelocity;
    }

    private float CalculateJumpVelocity(float yVelocity)
    {
        var isFalling = _rigidbody.velocity.y <= 0.001f;
        if (!isFalling)
            return yVelocity;

        if (_isGrounded)
        {
            yVelocity += _currentJumpSpeed;
            SpawnJumpParticle();
            _isJumpLocked = true;
            Invoke(nameof(UnlockJump), _damageJumpLockTime);
        }
        else if (_allowDoubleJump)
        {
            // Full strength double jump (compensate falling velocity)
            yVelocity = _currentJumpSpeed;
            SpawnJumpParticle();
            _allowDoubleJump = false;
            _isJumpLocked = true;
            Invoke(nameof(UnlockJump), _damageJumpLockTime);
        }

        return yVelocity;
    }

    private void UpdateSpriteDirection()
    {
        if (_directrion.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (_directrion.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    public void SaySomething()
    {
        Debug.Log("Hello!");
    }

    public void TakeHeal()
    {
        Debug.Log("Nice!");
    }

    public void TakeDamage()
    {
        if (_health.IsInvulnerable())
            return;

        Debug.Log("Ouch!");
        _isJumpLocked = true;
        _animator.SetTrigger(HitKey);
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
        _rigidbody.AddForce(Vector2.up * _damageJumpSpeed, ForceMode2D.Impulse);
        Invoke(nameof(UnlockJump), _damageJumpLockTime);
    }

    public void TakeJumpPower(float multiplier, float time)
    {
        Debug.Log("Oh my!");
        _currentJumpSpeed *= multiplier;
        Invoke(nameof(ResetBuff), time);
    }

    private void ResetBuff() =>
        _currentJumpSpeed = _jumpSpeed;

    private void UnlockJump() =>
        _isJumpLocked = false;

    public void Interact()
    {
        var size = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            _interactionRadius,
            _interactionResult,
            _interactionLayer);

        for (int i = 0; i < size; i++)
        {
            var interactable = _interactionResult[i].GetComponent<InteractableComponent>();
            if (interactable != null)
                interactable.Interact();
        }
    }

    public void SpawnRunParticle()
    {
        if (_isGrounded)
            _runParticles.Spawn();
    }

    public void SpawnJumpParticle()
    {
        _jumpParticles.Spawn();
    }

    public void SpawnFallParticle()
    {
        if (_fallVelocity < _hardLandSpeedThreshold)
            _fallParticles.Spawn();
    }
    
    public void SpawnAttackParticle()
    {
        _attackParticles.Spawn();
    }

    public void Attack()
    {
        if (_session.Data.IsArmed)
            _animator.SetTrigger(AttackKey);
    }
    
    public void OnAttack()
    {
        foreach (var go in _attackRange.getObjectsInRange())
        {
            if (go.CompareTag("Player"))
                continue;

            var hp = go.GetComponent<HealthComponent>();
            if (hp != null)
                hp.ModifyHealth(-_attackDamage);
        }
    }

    public void ArmHero()
    {
        _session.Data.IsArmed = true;
        UpdateHeroWeapon();
    }

    private void UpdateHeroWeapon() =>
        _animator.runtimeAnimatorController = _session.Data.IsArmed ? _armed : _unarmed;
}