using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Runner : MonoBehaviour
{
    private enum SpecialSkill
    {
        ExplodeColumn,
        ExplodeRow,
        CreateTwoBombs,
        DoubleSpecialBoost,
        IncreaseTimer
    }

    [SerializeField] private Image staminaBar;
    [SerializeField] private Image specialBar;
    [SerializeField] private Image runnerImage;
    [SerializeField] private MatchValueEnum color;
    [SerializeField] private int specialFillRequired = 100;
    [SerializeField] private SpecialSkill specialSkill;
    private Board board;
    private float currentSpecial = 0;
    private static bool onBoost;
    private Coroutine onBoostCoroutine;


    private void Awake()
    {
        specialBar.fillAmount = 0;
        staminaBar.fillAmount = 1;
    }

    private void Start()
    {
        onBoost = false;
    }

    private void OnEnable()
    {
        GamePiece.OnScorePoints += OnScore;
        board = GameObject.FindWithTag("Board").GetComponent<Board>();
    }

    private void OnDisable()
    {
        GamePiece.OnScorePoints -= OnScore;
    }

    private void OnScore(MatchValueEnum matchColor, int points)
    {
        if (matchColor != color) { return; }

        if (onBoost)
        {
            if (specialSkill != SpecialSkill.DoubleSpecialBoost)
            {
                points = (int)((float)points * 1.5f);
            }
            else
            {
                points = 4;
            }
        }

        currentSpecial += points / 4f;
        StartCoroutine(CountsProgressRoutine());
    }

    private IEnumerator CountsProgressRoutine()
    {
        int iterations = 0;

        float targetFillAmount = Mathf.Clamp(currentSpecial / specialFillRequired, 0, 1);
        while (specialBar.fillAmount < targetFillAmount && iterations < 1000)
        {
            specialBar.fillAmount += 0.0005f;
            yield return null;
        }

        if (specialBar.fillAmount >= 1)
        {
            runnerImage.rectTransform.DOShakeAnchorPos(1f, 20, 10, randomnessMode: ShakeRandomnessMode.Harmonic);
            Invoke(specialSkill.ToString(), 0.5f);
            currentSpecial = 0;
            specialBar.fillAmount = 0;
            yield break;
        }
        iterations++;
        yield return null;
    }

    private void ExplodeColumn()
    {
        board.ExplodeRandomColumn();
    }

    private void ExplodeRow()
    {
        board.ExplodeRandomRow();
    }

    private void CreateTwoBombs()
    {
        board.CreateBomb();
    }

    private void DoubleSpecialBoost()
    {
        if (onBoostCoroutine != null)
        {
            StopCoroutine(onBoostCoroutine);
        }
        onBoostCoroutine = StartCoroutine(TurnBoostOn());
    }

    private IEnumerator TurnBoostOn()
    {
        onBoost = true;
        ScoreManager.Instance.PunchScore();
        yield return new WaitForSeconds(6);
        ScoreManager.Instance.CleanScoreColor();
        onBoost = false;
        onBoostCoroutine = null;
    } 

    private void IncreaseTimer()
    {
        GameManager.Instance.IncreaseTimer();
    }
}
