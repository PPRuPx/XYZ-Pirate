using System.Collections;
using Components.ColliderBased;
using Creatures.Mobs.Patrolling;
using UnityEngine;

namespace Creatures.Mobs.Boss.DaddyShark
{
    public class DaddySharkAI : MonoBehaviour
    {
        [SerializeField] private LayerCheck _vision;
        [SerializeField] private LayerCheck _canAttack;      // Зона укуса
        [SerializeField] private LayerCheck _rangeAttackArea; // Зона стрельбы
        
        [Header("Timings")]
        [SerializeField] private float _alarmDelay = 0.5f;
        [SerializeField] private float _attackCooldown = 1.5f;
        [SerializeField] private float _shootingCooldown = 1f;
        [SerializeField] private float _missHeroCooldown = 2f;

        private Coroutine _current;
        private GameObject _target;
        private bool _isDead;
        private bool _needsQuakeRestart; // Флаг для перехвата урона в 3 фазе

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

        // --- БЛОК ПОДПИСКИ И ОТПИСКИ ---
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
        // -------------------------------

        private void OnBossHit()
        {
            // Если босса ударили в 3 фазе, взводим флаг перезапуска ульты
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
                // ПРИОРИТЕТ 1: Фаза 3 (Ульта -> Цикл стрельбы до урона)
                if (_creature.IsStage3)
                {
                    yield return StartCoroutine(UltimateCycle());
                }
                // ПРИОРИТЕТ 2: Ближний бой (если подошел вплотную)
                else if (_canAttack.IsTouchingLayers)
                {
                    yield return StartCoroutine(AttackAction());
                }
                // ПРИОРИТЕТ 3: Стрельба (Фаза 2)
                else if (_creature.IsStage2 && _rangeAttackArea.IsTouchingLayers)
                {
                    yield return StartCoroutine(ShootAction());
                }
                // ПРИОРИТЕТ 4: Просто идти к герою
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
            _needsQuakeRestart = false; // Сбрасываем флаг перед началом
            _creature.SetDirection(Vector2.zero);

            // 1. Сама Ульта
            _creature.ActivateAgroVisuals();
            _creature.Quake();
            yield return new WaitForSeconds(3f); // Время ярости и падения блоков

            // 2. Ведёт себя как во 2 фазе, пока не получит урон
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
            // Если вышли из цикла по _needsQuakeRestart == true, 
            // UltimateCycle завершится и GoToHero запустит его снова.
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