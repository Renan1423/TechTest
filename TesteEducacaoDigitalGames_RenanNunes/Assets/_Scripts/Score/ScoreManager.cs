using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _scoreText;

    private int _scoreCount = 0;

    public void AddScore(int score)
    {
        _scoreCount += score * 100;
        _scoreText.text = _scoreCount.ToString("D7");
    }
}
