using System.Collections;
using Components.ColliderBased;
using Creatures.Mobs.Patrolling;
using UnityEngine;

namespace Creatures.Mobs.Boss.DaddyShark
{
    public class DaddySharkAI : MonoBehaviour
    {
        [SerializeField] private LayerCheck _vision;
        [SerializeField] private LayerCheck _canAttack;
        [SerializeField] private LayerCheck _rangeAttackArea;
        
        [Header("Timings")]
        [SerializeField] private float _alarmDelay = 0.5f;
        [SerializeField] private float _attackCooldown = 1.5f;
        [SerializeField] private float _shootingCooldown = 1f;
        [SerializeField] private float _missHeroCooldown = 2f;

        private Coroutine _current;
        private GameObject _target;
        private bool _isDead;
        private bool _needsQuakeRestart;

        private DaddyShark _creature;
        private Animator _animator;
        private Patrol _patrol;

        private static readonly int IsDeadKey = Animator.StringToHash("is-dead");

        private void Awake()
        {
            _creature = GetComponent<DaddyShark>();
            _animator = GetComponent<Animator>();
            _patrol = GetComponent<Patrol>();
        }

        private void OnEnable()
        {
            if (_creature != null)
                _creature.OnTakeDamage += OnBossHit;
        }

        private void OnDisable()
        {
            if (_creature != null)
                _creature.OnTakeDamage -= OnBossHit;
        }

        private void OnBossHit()
        {
            if (_creature.IsStage3) 
                _needsQuakeRestart = true;
        }

        private void Start() => StartState(_patrol.DoPatrol());

        public void OnHeroInVision(GameObject go)
        {
            if (_isDead) return;
            _target = go;
            StartState(AgroToHero());
        }
        
        private IEnumerator AgroToHero()
        {
            LookAtHero();
            yield return new WaitForSeconds(_alarmDelay);
            StartState(GoToHero());
        }

        private IEnumerator GoToHero()
        {
            while (_vision.IsTouchingLayers)
            {
                if (_creature.IsStage3)
                {
                    yield return StartCoroutine(UltimateCycle());
                }
                else if (_canAttack.IsTouchingLayers)
                {
                    yield return StartCoroutine(AttackAction());
                }
                else if (_creature.IsStage2 && _rangeAttackArea.IsTouchingLayers)
                {
                    yield return StartCoroutine(ShootAction());
                }
                else
                {
                    _creature.SetDirection(GetDirectionToTarget());
                    yield return null;
                }
            }
            
            _creature.SetDirection(Vector2.zero);
            yield return new WaitForSeconds(_missHeroCooldown);
            StartState(_patrol.DoPatrol());
        }

        private IEnumerator UltimateCycle()
        {
            _needsQuakeRestart = false;
            _creature.SetDirection(Vector2.zero);

            _creature.ActivateAgroVisuals();
            _creature.Quake();
            yield return new WaitForSeconds(3f);

            while (!_needsQuakeRestart && _vision.IsTouchingLayers)
            {
                if (_canAttack.IsTouchingLayers)
                {
                    yield return StartCoroutine(AttackAction());
                }
                else if (_rangeAttackArea.IsTouchingLayers)
                {
                    LookAtHero();
                    _creature.Shoot();
                    yield return new WaitForSeconds(_shootingCooldown);
                }
                else
                {
                    _creature.SetDirection(GetDirectionToTarget());
                    yield return null;
                }
            }
        }

        private IEnumerator AttackAction()
        {
            LookAtHero();
            _creature.Attack();
            yield return new WaitForSeconds(_attackCooldown);
        }

        private IEnumerator ShootAction()
        {
            LookAtHero();
            _creature.Shoot();
            yield return new WaitForSeconds(_shootingCooldown);
        }

        private void LookAtHero()
        {
            _creature.SetDirection(Vector2.zero);
            _creature.UpdateSpriteDirection(GetDirectionToTarget());
        }

        private Vector2 GetDirectionToTarget()
        {
            if (_target == null) return Vector2.zero;
            return new Vector2(_target.transform.position.x - transform.position.x, 0).normalized;
        }

        private void StartState(IEnumerator coroutine)
        {
            if (_current != null) StopCoroutine(_current);
            _current = StartCoroutine(coroutine);
        }

        public void OnDie()
        {
            _isDead = true;
            _animator.SetBool(IsDeadKey, true);
            _creature.SetDirection(Vector2.zero);
            if (_current != null) StopCoroutine(_current);
        }
    }
}