using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private int _score;

    public int getScore() => _score;

    public void addScore(int value)
    {
        _score += value;
        Debug.Log("Score: " + _score);
    }
}