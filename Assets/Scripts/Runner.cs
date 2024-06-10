using System;
using UnityEngine;
using UnityEngine.UI;

public class Runner : MonoBehaviour
{
    [SerializeField] private Image staminaBar;
    [SerializeField] private Image specialBar;
    [SerializeField] private MatchValueEnum color;
    [SerializeField] private int specialFillRequired = 100;
    private float specialFillAmount = 0;


    private void Awake()
    {
        specialBar.fillAmount = 0;
        staminaBar.fillAmount = 1;
    }

    private void OnEnable()
    {
        GamePiece.OnScorePoints += OnScore;
    }

    private void OnDisable()
    {
        GamePiece.OnScorePoints -= OnScore;
    }

    private void OnScore(MatchValueEnum matchColor, int points)
    {
        if (matchColor != color) { return; }

        specialFillAmount += points / 400f;
        specialBar.fillAmount = specialFillAmount;

        if (specialBar.fillAmount >= 1)
        {
            specialFillAmount = 0;
            specialBar.fillAmount = 0;
            Debug.Log($"**** {color} SOLTOU O ESPECIAL ****");
        }
    }
}
