using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerUpHandler
{
    public int x {  get; private set; }
    public int y { get; private set; }
    private Board board;

    private void Start()
    {
        
    }

    public void Init(int x, int y, Board board)
    {
        this.x = x;
        this.y = y;
        this.board = board;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (board == null) { return; }
        board.ClickTile(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (board == null) { return; }
        board.DragToTile(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        board.ReleaseTile();
    }
}
