using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonBase<GameManager>
{
    [SerializeField] private int movesLeft = 30;
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
        Debug.Log("ExecuteGameLoop");
        yield return StartCoroutine("StartGameRoutine");
        if (board == null) yield break;
        yield return StartCoroutine("PlayGameRoutine");
        yield return StartCoroutine("EndGameRoutine");
    }

    private IEnumerator StartGameRoutine()
    {
        Debug.Log("StartGameRoutine");
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
        Debug.Log("PlayGameRoutine");
        while (!isGameOver)
        {
            timeLeft -= Time.deltaTime;
            ScoreManager.Instance.UpdateTimer(timeLeft);
            //if (timeLeft <= 0)
            //{
            //    isGameOver = true;
            //    isWinner = false;
            //}
            yield return null;
        }
        ScreenFader.Instance.FadeOn();
    }

    private IEnumerator EndGameRoutine()
    {
        Debug.Log("EndGameRoutine");
        board = null;
        if (isWinner)
        {
            NextScene();
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
        Debug.Log("LoadNextSceneRoutine");
        ScreenFader.Instance.FadeOn();
        yield return new WaitForSeconds(1f);
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex -= 2;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }
}
