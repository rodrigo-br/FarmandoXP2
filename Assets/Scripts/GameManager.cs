using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonBase<GameManager>
{
    private float timeLeft = 60;
    public Observer Observer { get; private set; }

    private bool isReadyToBegin = false;
    private bool isGameOver = false;
    private bool isWinner = false;
    private Board board;

    public override void Awake()
    {
        board = null;
        base.Awake();
        Observer = new Observer();
    }

    public void Start()
    {
        StartCoroutine("ExecuteGameLoop");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        StartCoroutine("ExecuteGameLoop");
    }

    private IEnumerator ExecuteGameLoop()
    {
        yield return StartCoroutine("StartGameRoutine");
        if (board == null) yield break;
        yield return StartCoroutine("PlayGameRoutine");
        yield return StartCoroutine("EndGameRoutine");
    }

    private IEnumerator StartGameRoutine()
    {
        ScoreManager.Instance.gameObject.SetActive(false);
        while (!isReadyToBegin)
        {
            yield return null;
            yield return new WaitForSeconds(1f);
            isReadyToBegin = true;
        }

        if (ScreenFader.Instance != null)
        {
            ScreenFader.Instance.FadeOff(0.45f);
            yield return new WaitForSeconds(0.5f);
        }

        board = GameObject.FindWithTag("Board")?.GetComponent<Board>();

        if (board != null)
        {
            timeLeft = 60;
            board.SetupBoard();
            ScoreManager.Instance.UpdateLevelText();
        }
    }

    private IEnumerator PlayGameRoutine()
    {
        while (!isGameOver)
        {
            timeLeft = Mathf.Clamp(timeLeft - Time.deltaTime, 0, float.MaxValue);
            ScoreManager.Instance.UpdateTimer(timeLeft);
            if (timeLeft == 0)
            {
                SetWinner();
            }
            yield return null;
        }
        ScreenFader.Instance.FadeOn();
    }

    private IEnumerator EndGameRoutine()
    {
        board = null;
        if (isWinner)
        {
            NextScene();
            AudioManager.Instance.PlayVictoryScreenMusic();
        }
        else
        {
            SceneManager.LoadScene(0);
        }
        yield return null;
        
    }

    public void NextScene()
    {
        StartCoroutine("LoadNextSceneRoutine");
    }

    public void SetWinner()
    {
        isWinner = true;
        isGameOver = true;
    }

    private IEnumerator LoadNextSceneRoutine()
    {
        isWinner = false;
        isGameOver = false;
        timeLeft = 60;
        ScreenFader.Instance.FadeOn();
        yield return new WaitForSeconds(1f);
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        ScoreManager.Instance.CleanScoreColor();
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex -= 2;
            AudioManager.Instance.PlayMainMusic();
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    public void IncreaseTimer()
    {
        timeLeft += 3;
        ScoreManager.Instance.PunchTimer();
    }
}
