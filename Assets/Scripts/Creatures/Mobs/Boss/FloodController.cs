using System.Collections;
using UnityEngine;

namespace Creatures.Mobs.Boss
{
    public class FloodController : MonoBehaviour
    {
        [SerializeField] private Animator _floodAnimator;
        [SerializeField] private float _floodTime;

        private static readonly int IsFlooding = Animator.StringToHash("is-flooding");

        private Coroutine _coroutine;
        
        public void StartFlooding()
        {
            if (_coroutine != null)
                return;
            
            StartCoroutine(Animate());
        }

        private IEnumerator Animate()
        {
            _floodAnimator.SetBool(IsFlooding, true);
            yield return new WaitForSeconds(_floodTime);
            _floodAnimator.SetBool(IsFlooding, false);
        }
    }
}