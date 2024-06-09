using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : SingletonBase<ScoreManager>
{
    private int currentScore = 0;
    private int counterValue = 0;
    private int increment = 5;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI levelText;

    private void Start()
    {
        UpdateScoreText(currentScore);
        levelText.text = "Level 1";
    }

    public void UpdateScoreText(int scoreValue)
    {
        scoreText.text = scoreValue.ToString();
    }

    public void AddScore(int value)
    {
        currentScore += value;
        StartCoroutine(CountsScoreRoutine());
    }

    public void SetLevelText(string levelText)
    {
        this.levelText.text = levelText;
    }

    private IEnumerator CountsScoreRoutine()
    {
        int iterations = 0;

        while (counterValue < currentScore && iterations < 1000)
        {
            counterValue += increment;
            UpdateScoreText(counterValue);
            iterations++;
            yield return null;
        }
    }

}
