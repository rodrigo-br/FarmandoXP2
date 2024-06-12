using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : SingletonBase<ScoreManager>
{
    private int currentScore = 0;
    private int counterValue = 0;
    private int increment = 5;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI timer;
    private Image levelProgressBar;
    private Level level;
    private int nextLevelScore;

    public override void Awake()
    {
        base.Awake();
        level = new Level(0, 1000, 2);
    }

    private void Start()
    {
        UpdateScoreText(level.GetCurrentExperience());
        levelText.text = $"{level.GetLevel() + 1}";
        nextLevelScore = level.GetExperienceForNextLevel();
    }

    public void UpdateTimer(float value)
    {
        timer.text = $"{(int)value}";
    }

    public void UpdateScoreText(int scoreValue)
    {
        scoreText.text = $"{scoreValue}";
    }

    public void AddScore(int value)
    {
        int currentLevel = level.GetLevel();
        level.AddExperience(value);
        StartCoroutine(CountsProgressRoutine());
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
        levelProgressBar = GameObject.FindWithTag("LevelProgressBar").GetComponent<Image>();
        StartCoroutine(CountsProgressRoutine());
        levelText.text = $"{level.GetLevel() + 1}";
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

    private IEnumerator CountsProgressRoutine()
    {
        int iterations = 0;

        while (levelProgressBar.fillAmount < level.GetExperiencePercentage() && iterations < 1000)
        {
            levelProgressBar.fillAmount += 0.0002f;
            iterations++;
            yield return null;
        }
    }

    public void PunchTimer()
    {
        timer.transform.DOPunchScale(Vector3.one * 2, 1f, 1, 0);
    }

    public void PunchScore()
    {
        scoreText.transform.DOPunchScale(Vector3.one * 2, 10f, 5, 0);
        scoreText.color = Color.red;
    }

    public void CleanScoreColor()
    {
        scoreText.color = Color.white;
    }

    public string UpdateWinnerScreen()
    {
        int oldLevelScore = nextLevelScore;
        nextLevelScore = level.GetTotalExperienceForLevel();
        return $"{currentScore} / {oldLevelScore}";
    }



}
