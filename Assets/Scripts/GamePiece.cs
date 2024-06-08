using System.Collections;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    public enum MatchValueEnum
    {
        Yellow,
        Green,
        Blue,
        Red,
        Purlple,
        Gray
    }
    [field: SerializeField] public MatchValueEnum MatchValue { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }
    private RectTransform rectTransform;
    public bool IsMoving { get; private set; } = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
    }

    public void SetCoord(int x, int y, float time = 0.5f)
    {
        this.X = x;
        this.Y = y;
        Move(0, 0, time);
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
        Vector3 startPosition = rectTransform.anchoredPosition;
        bool reachedDestination = false;
        float elapsedTime = 0f;
        while (!reachedDestination)
        {
            if (Vector3.Distance(rectTransform.anchoredPosition, destination) < 0.01f)
            {
                reachedDestination = true;
                rectTransform.anchoredPosition = destination;
                break;
            }

            elapsedTime += Time.deltaTime;
            float time = elapsedTime / timeToMove; // Linear interpolation
            time = time * time * time * (time * (time * 6 - 15) + 10); // SmootherStep interpolation

            rectTransform.anchoredPosition = Vector3.Lerp(startPosition, destination, time);

            yield return null;
        }
        IsMoving = false;
    }
}
