using Components.GameObjectBased;
using UnityEngine;

namespace Creatures.Mobs.Boss
{
    public class BossBombState : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var spawner = animator.GetComponent<DropLootComponent>();
            spawner.DropLoot();
        }
    }
}