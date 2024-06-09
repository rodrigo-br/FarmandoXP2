using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BombType
{
    None,
    Thunder,
    Bomb,
    Color
}

public class Bomb : GamePiece
{
    [field: SerializeField] public BombType BombType { get; private set; }
}
