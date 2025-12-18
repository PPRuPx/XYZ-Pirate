using Creatures.Hero;
using UnityEngine;

namespace Components.Collectables
{
    public class ArmHeroComponent : MonoBehaviour
    {
        public void ArmHero(GameObject target)
        {
            var hero = target.GetComponent<Hero>();
            if (hero != null)
                hero.ArmHero();
        }
    }
}
