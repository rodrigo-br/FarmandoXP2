using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum MatchValueEnum
{
    Yellow,
    Green,
    Blue,
    Red,
    Purlple,
    Gray,
    None
}

public class GamePiece : MonoBehaviour
{
    [field: SerializeField] public MatchValueEnum MatchValue { get; private set; }
    [SerializeField] private int scorePoints = 20;
    public int X { get; private set; }
    public int Y { get; private set; }
    public RectTransform RectTransform { get; private set; }
    public bool IsMoving { get; private set; } = false;
    public static event Action<MatchValueEnum, int> OnScorePoints;
    public Image Image_ { get; private set; } 

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        Image_ = gameObject.GetComponent<Image>();
        Debug.Log(Image_);
    }

    public void SetCoord(int x, int y, float time = 0.5f)
    {
        this.X = x;
        this.Y = y;
        Move(0, 0, time);
    }

    public void SetMatchValue(MatchValueEnum matchValue)
    {
        this.MatchValue = matchValue;
    }

    public (int, int) GetCoord()
    {
        return (X, Y);
    }

    public void Move(int destX, int destY, float timeToMove)
    {
        StartCoroutine(MoveRoutine(new Vector3(destX, destY, 0), timeToMove));
    }

    private IEnumerator MoveRoutine(Vector3 destination, float timeToMove)
    {
        IsMoving = true;
        Vector3 startPosition = RectTransform.anchoredPosition;
        bool reachedDestination = false;
        float elapsedTime = 0f;
        while (!reachedDestination)
        {
            if (Vector3.Distance(RectTransform.anchoredPosition, destination) < 0.01f)
            {
                reachedDestination = true;
                RectTransform.anchoredPosition = destination;
                break;
            }

            elapsedTime += Time.deltaTime;
            float time = elapsedTime / timeToMove; // Linear interpolation
            time = time * time * time * (time * (time * 6 - 15) + 10); // SmootherStep interpolation

            RectTransform.anchoredPosition = Vector3.Lerp(startPosition, destination, time);

            yield return null;
        }
        IsMoving = false;
    }

    public void ScorePoints(int multiplier = 1, int bonus = 0)
    {
        int points = (scorePoints * multiplier) + bonus;
        OnScorePoints?.Invoke(MatchValue, points);
        if (ScoreManager.Instance == null) return;

        ScoreManager.Instance.AddScore(points);
    }
}
