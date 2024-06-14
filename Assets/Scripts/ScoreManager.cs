using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : SingletonBase<ScoreManager>
{
    private int currentScore = 0;
    private int counterValue = 0;
    private int increment = 2;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI timer;
    private Image levelProgressBar;
    private Level level;
    private int nextLevelScore;
    private Vector3 defaultScoreSize;
    private Tweener scoreTextTweener;

    public override void Awake()
    {
        base.Awake();
        level = new Level(0, 1000, 2);
        defaultScoreSize = scoreText.gameObject.transform.localScale;
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
        scoreTextTweener = scoreText.transform.DOPunchScale(Vector3.one * 2, 10f, 5, 0);
        scoreText.color = Color.red;
    }

    public void CleanScoreColor()
    {
        if (scoreTextTweener != null)
        {
            scoreTextTweener.Kill();
            scoreTextTweener = null;
        }
        scoreText.color = Color.white;
        scoreText.transform.DOScale(defaultScoreSize, 0.05f);
    }

    public void UpdateWinnerScreen(References references)
    {
        int oldLevelScore = nextLevelScore;
        nextLevelScore = level.GetTotalExperienceForLevel();
        references.Score.text = $"{currentScore} / {oldLevelScore}";

        float progressPercentage = (float)currentScore / oldLevelScore * 100;
        int starsToActivate = Mathf.Clamp((int)(progressPercentage / 20), 0, 5);
        for (int i = 0; i < references.Stars.Length; i++)
        {
            if (i < starsToActivate)
            {
                references.Stars[i].enabled = true;
            }
            else
            {
                references.Stars[i].enabled = false;
            }
        }

        Messages message = null;
        int index = 0;
        Debug.Log($"starsToActivate {starsToActivate}");
        switch (starsToActivate)
        {
            case 0:
                index = Random.Range(0, references.ZeroStarMessages.Length);
                message = references.ZeroStarMessages[index];
                break;
            case 1:
                index = Random.Range(0, references.OneStarMessages.Length);
                message = references.OneStarMessages[index];
                break;
            case 2:
                index = Random.Range(0, references.TwoStarMessages.Length);
                message = references.TwoStarMessages[index];
                break;
            case 3:
                index = Random.Range(0, references.ThreeStarMessages.Length);
                message = references.ThreeStarMessages[index];
                break;
            case 4:
                index = Random.Range(0, references.FourStarMessages.Length);
                message = references.FourStarMessages[index];
                break;
            case 5:
                index = Random.Range(0, references.FiveStarMessages.Length);
                message = references.FiveStarMessages[index];
                break;

        }
        Debug.Log($"INDEX: {index}");
        Debug.Log($"MESSAGE: {message}");
        references.Message.text = message.message;
        AudioManager.Instance.PlayCongratulations(message.voice);
        Debug.Log("VOICE HERE!");
        Debug.Log(message.voice);
    }

    public int GetLevel()
    {
        return level.GetLevel() + 1;
    }
}
