using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Hero : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _jumpSpeed;
    [SerializeField] private LayerCheck _groundCheck;

    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private SpriteRenderer _renderer;
    
    private Vector3 _directrion;
    private bool _isGrounded;
    private bool _allowDoubleJump;

    private static readonly int IsRunningKey = Animator.StringToHash("is-running"); 
    private static readonly int IsGroundedKey = Animator.StringToHash("is-grounded"); 
    private static readonly int VerticalVelocityKey = Animator.StringToHash("vertical-velocity"); 
    

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void SetDirection(Vector3 direction)
    {
        _directrion = direction;
    }

    private void Update()
    {
        _isGrounded = _groundCheck.IsTouchingLayer;
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
            yVelocity += _jumpSpeed;
        }
        else if (_allowDoubleJump)
        {
            // Full strength double jump (compensate falling velocity)
            yVelocity = _jumpSpeed;
            _allowDoubleJump = false;
        }

        return yVelocity;
    }

    private void UpdateSpriteDirection()
    {
        if (_directrion.x > 0)
            _renderer.flipX = false;
        else if (_directrion.x < 0)
            _renderer.flipX = true;
    }

    public void SaySomething()
    {
        Debug.Log("Hello!");
    }
}