using Components.GameObjectBased;
using UnityEngine;

namespace Creatures.Mobs.Boss
{
    public class BossNextStageState : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var spawner = animator.GetComponent<CircularProjectileSpawner>();
            if (spawner != null)
                spawner.Stage++;

            var changeLight = animator.GetComponent<ChangeLightsComponent>();
            if (changeLight != null)
                changeLight.ApplyNextSetup();
        }
    }
}