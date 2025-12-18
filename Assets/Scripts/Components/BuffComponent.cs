using Creatures.Hero;
using UnityEngine;

namespace Components
{
    public class BuffComponent : MonoBehaviour
    {
        [SerializeField] private float _power;
        [SerializeField] private float _time;

        public void ApplyBuff(GameObject target)
        {
            var hero = target.GetComponent<Hero>();
            if (hero != null)
            {
                hero.ApplyJumpPowerBuff(_power, _time);
            }
        }
    }
}