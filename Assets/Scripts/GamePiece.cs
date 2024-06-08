using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    public int x { get; private set; }
    public int y { get; private set; }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void SetCoord(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
