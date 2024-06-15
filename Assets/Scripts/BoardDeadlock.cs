using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BoardDeadlock : MonoBehaviour
{
    private List<GamePiece> GetRowOrColumnList(GamePiece[,] gamePieces, int x, int y, int listLength = 3, bool checkRow = true)
    {
        int width = gamePieces.GetLength(0);
        int height = gamePieces.GetLength(1);

        List<GamePiece> piecesList = new();

        for (int i = 0; i < listLength; i++)
        {
            if (checkRow)
            {
                if (x + i < width && y < height)
                {
                    piecesList.Add(gamePieces[x + i, y]);
                }
            }
            else
            {
                if (x < width && y + i < height)
                {
                    piecesList.Add(gamePieces[x, y + i]);
                }
            }
        }
        return piecesList;
    }

    private List<GamePiece> GetMinimumMatches(List<GamePiece> gamePieces, int minForMatch = 2)
    {
        if (gamePieces == null)
        {
            return new List<GamePiece>();
        }

        List<GamePiece> matches = new();

        var nonNullGamePieces = gamePieces.Where(n => n != null);

        var groups = nonNullGamePieces.GroupBy(n => n.MatchValue);

        foreach (var group in groups)
        {
            if (group.Count() >= minForMatch && group.Key != MatchValueEnum.None)
            {
                matches = group.ToList();
            }
        }

        return matches;
    }

    private List<GamePiece> GetNeighbors(GamePiece[,] gamePieces, int x, int y)
    {
        int width = gamePieces.GetLength(0);
        int height = gamePieces.GetLength(1);

        List<GamePiece> neighbors = new();

        Vector2[] searchDirections = new Vector2[4]
        {
            Vector2.left, Vector2.right, Vector2.up, Vector2.down
        };

        foreach (Vector2 direction in searchDirections)
        {
            if (x + (int)direction.x >= 0 && x + (int)direction.x < width && y + (int)direction.y >= 0 && y + (int)direction.y < height)
            {
                if (gamePieces[x + (int)direction.x, y + (int)direction.y] != null)
                {
                    if (!neighbors.Contains(gamePieces[x + (int)direction.x, y + (int)direction.y]))
                    {
                        neighbors.Add(gamePieces[x + (int)direction.x, y + (int)direction.y]);
                    }
                }
            }
        }

        return neighbors;
    }

    private (bool, List<GamePiece>) HasMoveAt(GamePiece[,] gamePieces, int x, int y, int listLength = 3, bool checkRow = true)
    {
        List<GamePiece> pieces = GetRowOrColumnList(gamePieces, x, y, listLength, checkRow);

        List<GamePiece> matches = GetMinimumMatches(pieces, listLength - 1);

        GamePiece unmatchedPiece = null;

        if (pieces == null || matches == null) return (false, null);
        
        if (pieces.Count == listLength && matches.Count == listLength - 1)
        {
            unmatchedPiece = pieces.Except(matches).FirstOrDefault();
        }

        if (unmatchedPiece != null)
        {
            List<GamePiece> neighbors = GetNeighbors(gamePieces, unmatchedPiece.X, unmatchedPiece.Y);
            neighbors = neighbors.Except(matches).ToList();
            neighbors = neighbors.FindAll(n => n.MatchValue == matches[0].MatchValue);

            matches = matches.Union(neighbors).ToList();
        }

        bool hasMatch = matches.Count >= listLength;        
        return (hasMatch, matches);
    }

    public (bool isDeadlocked, List<GamePiece> matches) IsDeadlocked(GamePiece[,] gamePieces, int listLength = 3)
    {
        int width = gamePieces.GetLength(0);
        int height = gamePieces.GetLength(1);

        for(int i = 0; i < width;  i++)
        {
            for (int j = 0; j < height; j++)
            {
                (bool hasMoveRow, List<GamePiece> matchesRow) = HasMoveAt(gamePieces, i, j, listLength, true);
                if (hasMoveRow)
                {
                    return (false, matchesRow);
                }

                (bool hasMoveColumn, List<GamePiece> matchesColumn) = HasMoveAt(gamePieces, i, j, listLength, false);
                if (hasMoveColumn)
                {
                    return (false, matchesColumn);
                }
            }
        }
        return (true, null);
    }
}
