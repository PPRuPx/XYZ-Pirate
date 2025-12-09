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
    
    private void FixedUpdate()
    {
        _rigidbody.velocity = new Vector2(_directrion.x * _moveSpeed, _rigidbody.velocity.y);

        var isJumping = _directrion.y > 0;
        var isGrounded = _groundCheck.IsTouchingLayer;
        
        if (isJumping)
        {
            if (isGrounded)
            {
                _rigidbody.AddForce(Vector2.up * _jumpSpeed, ForceMode2D.Impulse);     
            }
        }
        else if (_rigidbody.velocity.y > 0)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y * 0.5f);
        }
        
        _animator.SetBool(IsRunningKey, _directrion.x != 0);
        _animator.SetBool(IsGroundedKey, isGrounded);
        _animator.SetFloat(VerticalVelocityKey, _rigidbody.velocity.y);

        UpdateSpriteDirection();
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