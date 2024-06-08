using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
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
}
