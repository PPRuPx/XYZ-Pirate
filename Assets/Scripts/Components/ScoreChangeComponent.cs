using UnityEngine;

namespace Components
{
    public class ScoreChangeComponent : MonoBehaviour
    {
        [SerializeField] private Score _score;
        [SerializeField] private int value;

        public void ChangeScore()
        {
            _score.addScore(value);
        }
    }
}