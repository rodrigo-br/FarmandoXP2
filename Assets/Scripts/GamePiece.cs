
using System.Collections;
using UnityEngine;

public enum MatchValueEnum
{
    Yellow,
    Green,
    Blue,
    Red,
    Purlple,
    Gray
}

public class GamePiece : MonoBehaviour
{
    [field: SerializeField] public MatchValueEnum MatchValue { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public RectTransform RectTransform { get; private set; }
    public bool IsMoving { get; private set; } = false;

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
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
}
