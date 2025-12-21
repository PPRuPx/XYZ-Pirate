using System.Collections;
using Components.ColliderBased;
using UnityEngine;

namespace Creatures.Mobs.Patrolling
{
    public class WallPatrol : Patrol
    {
        [SerializeField] private LayerCheck _wallCheck;
        [SerializeField] private int _direction = 1;
        
        private Creature _creature;
        
        private void Awake()
        {
            _creature = GetComponent<Creature>();
        }
        
        public override IEnumerator DoPatrol()
        {
            while (enabled)
            {
                if (!_wallCheck.IsTouchingLayers)
                {
                    _creature.SetDirection(new Vector2(_direction, 0));
                }
                else
                {
                    _direction = -_direction;
                    _creature.SetDirection(new Vector2(_direction, 0));
                }

                yield return null;
            }
        }
    }
}