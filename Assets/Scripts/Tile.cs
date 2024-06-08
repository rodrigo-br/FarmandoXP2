using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
{
    public int X { get; private set; }
    public int Y { get; private set; }
    private Board board;
    //private bool isPressingMouse = false;
    private RectTransform rectTransform;
    private Vector2 originalScale;

    private void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
    }

    public void Init(int x, int y, Board board)
    {
        this.X = x;
        this.Y = y;
        this.board = board;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (board == null) { return; }
        board.ClickTile(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (board == null) { return; }
        board.DragToTile(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        board.ReleaseTile();
    }
}
