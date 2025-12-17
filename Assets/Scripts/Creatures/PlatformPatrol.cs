using System.Collections;
using UnityEngine;

namespace Creatures
{
    public class PlatformPatrol : Patrol
    {
        [SerializeField] private LayerCheck _groundCheck;
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
                if (_groundCheck.IsTouchingLayers)
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