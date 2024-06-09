using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonBase<GameManager>
{
    [SerializeField] private int movesLeft = 30;
    [SerializeField] private int scoreGoal = 5000;
    private float timeLeft = 5;
    public Observer Observer { get; private set; }

    private bool isReadyToBegin = false;
    private bool isGameOver = false;
    private bool isWinner = false;

    public override void Awake()
    {
        base.Awake();
        Observer = new Observer();
    }

    public void Start()
    {
        StartCoroutine("ExecuteGameLoop");
    }

    private IEnumerator ExecuteGameLoop()
    {
        yield return StartCoroutine("StartGameRoutine");
        yield return StartCoroutine("PlayGameRoutine");
        yield return StartCoroutine("EndGameRoutine");
    }

    private IEnumerator StartGameRoutine()
    {
        while (!isReadyToBegin)
        {
            yield return null;
            yield return new WaitForSeconds(1f);
            isReadyToBegin = true;
        }

        if (ScreenFader.Instance != null)
        {
            ScreenFader.Instance.FadeOff();
            yield return new WaitForSeconds(0.5f);
        }

        Board board = GameObject.FindWithTag("Board").GetComponent<Board>();

        if (board != null)
        {
            board.SetupBoard();
        }
    }

    private IEnumerator PlayGameRoutine()
    {
        while (!isGameOver)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                isGameOver = true;
                isWinner = false;
                ScreenFader.Instance.FadeOn();
            }
            yield return null;
        }
    }

    private IEnumerator EndGameRoutine()
    {
        if (isWinner)
        {
            Debug.Log("You win");
        }
        else
        {
            Debug.Log("You lose");
        }
        yield return null;
        
    }
}
