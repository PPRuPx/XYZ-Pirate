using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreChangeComponent : MonoBehaviour
{

    [SerializeField] private Score _score;
    [SerializeField] private int value;

    public void ChangeScore()
    {
        _score.addScore(value);
    }
}
