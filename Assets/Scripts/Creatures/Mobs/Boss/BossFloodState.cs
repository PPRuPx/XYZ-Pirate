using Components.GameObjectBased;
using UnityEngine;

namespace Creatures.Mobs.Boss
{
    public class BossFloodState: StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var controller = animator.GetComponent<FloodController>();
            controller.StartFlooding();
        }
    }
}