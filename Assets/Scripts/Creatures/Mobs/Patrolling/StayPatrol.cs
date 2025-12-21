using System.Collections;

namespace Creatures.Mobs.Patrolling
{
    public class StayPatrol : Patrol
    {
        public override IEnumerator DoPatrol()
        {
            yield return null;
        }
    }
}