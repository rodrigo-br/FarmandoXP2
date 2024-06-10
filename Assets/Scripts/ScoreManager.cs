using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreManager : SingletonBase<ScoreManager>
{
    private int currentScore = 0;
    private int counterValue = 0;
    private int increment = 5;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI timer;
    private Level level;

    public override void Awake()
    {
        base.Awake();
        level = new Level(0, 1000, 2);
    }

    private void Start()
    {
        UpdateScoreText(level.GetCurrentExperience());
        levelText.text = $"Level {level.GetLevel() + 1}";
    }

    public void UpdateTimer(float value)
    {
        timer.text = $"{(int)value}";
    }

    public void UpdateScoreText(int scoreValue)
    {
        scoreText.text = scoreValue.ToString();
    }

    public void AddScore(int value)
    {
        int currentLevel = level.GetLevel();
        level.AddExperience(value);
        currentScore += value;
        if (currentLevel < level.GetLevel())
        {
            GameManager.Instance.SetWinner();
        }
        StartCoroutine(CountsScoreRoutine());
    }

    public void UpdateLevelText()
    {
        gameObject.SetActive(true);
        levelText.text = $"Level {level.GetLevel() + 1}";
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
