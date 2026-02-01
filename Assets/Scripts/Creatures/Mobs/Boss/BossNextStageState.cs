using Components.GameObjectBased;
using UnityEngine;

namespace Creatures.Mobs.Boss
{
    public class BossNextStageState : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var spawner = animator.GetComponent<CircularProjectileSpawner>();
            spawner.Stage++;

            var changeLight = animator.GetComponent<ChangeLightsComponent>();
            changeLight.ApplyNextSetup();
        }
    }
}