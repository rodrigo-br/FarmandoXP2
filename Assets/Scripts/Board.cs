using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [Range(1, 7)][SerializeField] private int width;
    [Range(1, 5)][SerializeField] private int height;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject[] gamePiecePrefabs;
    [SerializeField] private float moveTime = 0.4f;
    private RectTransform boardRectTransform;
    private float xOffset;
    private float yOffset;
    private Vector2 tileSize;
    private Tile[,] tiles;
    private GamePiece[,] gamePieces;
    private Tile clickedTile;
    private Tile targetTile;
    private bool playerInputEnabled = true;
    private bool playerInputBusy = false;

    private void Awake()
    {
        tiles = new Tile[width, height];
        gamePieces = new GamePiece[width, height];
        boardRectTransform = GetComponent<RectTransform>();
        Rect tileRect = tilePrefab.GetComponent<RectTransform>().rect;
        tileSize = new Vector2(tileRect.width, tileRect.height);

        xOffset = (boardRectTransform.rect.width - (tileSize.x * width)) / 2 - boardRectTransform.rect.width / 2;
        yOffset = (boardRectTransform.rect.height - (tileSize.y * height)) / 2 - boardRectTransform.rect.height / 2;
    }

    private void Start()
    {
        SetupTiles();
        FillBoard(-5);
    }

    private void SwapGamePieces(GamePiece gp1, GamePiece gp2)
    {
        if (gp1 == null || gp2 == null || gp1.IsMoving || gp2.IsMoving || !playerInputEnabled) { return; }
        StartCoroutine(SwapGamePiecesCoroutine(gp1, gp2));
    }

    private IEnumerator SwapGamePiecesCoroutine(GamePiece gp1, GamePiece gp2, bool matchCheck = true)
    {
        int gp1X = gp1.X;
        int gp1Y = gp1.Y;
        int gp2X = gp2.X;
        int gp2Y = gp2.Y;
        if (Mathf.Abs(gp1X - gp2X) + Mathf.Abs(gp1Y - gp2Y) > 1)
        {
            yield break;
        }

        RectTransform rectTransform1 = gp1.RectTransform;
        RectTransform rectTransform2 = gp2.RectTransform;

        rectTransform1.SetParent(tiles[gp2X, gp2Y].RectTransform);
        rectTransform2.SetParent(tiles[gp1X, gp1Y].RectTransform);

        gamePieces[gp1X, gp1Y] = gp2;
        gamePieces[gp2X, gp2Y] = gp1;
        gp1.SetCoord(gp2X, gp2Y, moveTime);
        gp2.SetCoord(gp1X, gp1Y, moveTime);

        yield return new WaitForSeconds(moveTime);

        if (!matchCheck) { yield break; }

        List<GamePiece> gp1Matches = FindMacthesAt(gp1X, gp1Y);
        List<GamePiece> gp2Matches = FindMacthesAt(gp2X, gp2Y);

        if (gp1Matches.Count + gp2Matches.Count > 0)
        {
            ClearAndRefillBoard(gp1Matches.Union(gp2Matches).ToList());
            yield break;
        }
        StartCoroutine(SwapGamePiecesCoroutine(gp1, gp2, false));
    }

    private bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < height);
    }

    private void SetupTiles()
    {

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject tile = Instantiate(tilePrefab, transform) as GameObject;
                tile.name = $"Tile ({x},{y})";

                RectTransform tileRectTransform = tile.GetComponent<RectTransform>();
                tileRectTransform.SetParent(boardRectTransform, false);

                float xPos = tileSize.x * x + tileSize.x / 2 + xOffset;
                float yPos = tileSize.y * y + tileSize.y / 2 + yOffset;
                tileRectTransform.anchoredPosition = new Vector2(xPos, yPos);

                tiles[x,y] = tile.GetComponent<Tile>();

                tile.transform.SetParent(transform);
                tiles[x, y].Init(x, y, this);
            }
        }
    }

    private GameObject GetRandomGamePiece()
    {
        int index = Random.Range(0, gamePiecePrefabs.Length);

        if (gamePiecePrefabs[index] == null)
        {
            Debug.LogWarning($"BOARD: {index} does not contain a valid GamePiece prefab!");
        }

        return gamePiecePrefabs[index];
    }

    private void PlaceGamePiece(GamePiece gamePiece, int x, int y)
    {
        if (gamePiece == null)
        {
            Debug.LogWarning("BOARD: Invalid GamePiece!");
            return;
        }

        gamePiece.RectTransform.SetParent(tiles[x, y].RectTransform, false);
        gamePiece.SetCoord(x, y);
    }

    private void FillBoard(int falseOffset = 0)
    {
        int maxIterations = 50;
        int currentIterations;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (gamePieces[x, y] != null) { continue; }
                GamePiece piece = FillRandomAt(x, y, falseOffset);
                currentIterations = 0;

                while (HasMatchOnFill(x, y))
                {
                    currentIterations++;
                    if (currentIterations >= maxIterations)
                    {
                        break;
                    }
                    ClearPieceAt(x, y);
                    piece = FillRandomAt(x, y);
                }
            }
        }
    }

    private bool HasMatchOnFill(int x, int y, int minLength = 3)
    {
        List<GamePiece> leftMatches = FindMatches(x, y, Vector2.left, minLength);
        List<GamePiece> downwardMatches = FindMatches(x, y, Vector2.down, minLength);

        leftMatches ??= new();
        downwardMatches ??= new();

        return (leftMatches.Count + downwardMatches.Count > 0);
    }

    private GamePiece FillRandomAt(int x, int y, int falseOffset = 0)
    {
        GameObject gamePieceGO = Instantiate(GetRandomGamePiece());

        if (gamePieceGO == null) { return null; }

        GamePiece gamePiece = gamePieceGO.GetComponent<GamePiece>();
        gamePieces[x, y] = gamePiece;
        PlaceGamePiece(gamePiece, x, y);

        if (falseOffset != 0)
        {
            gamePiece.RectTransform.anchoredPosition += new Vector2(0, (y + falseOffset) * yOffset);
            gamePiece.Move(0, 0, moveTime);
        }
        return gamePiece;
    }

    public void ClickTile(Tile tile)
    {
        if (clickedTile == null && !playerInputBusy && playerInputEnabled)
        {
            playerInputBusy = true;
            clickedTile = tile;
        }
    }

    public void DragToTile(Tile tile)
    {
        if (clickedTile != null)
        {
            targetTile = tile;
        }
    }

    public void ReleaseTile()
    {
        if (clickedTile != null && targetTile != null)
        {
            SwitchTiles(clickedTile, targetTile);
        }

        clickedTile = null;
        targetTile = null;
        StartCoroutine(FreePlayerInputCoroutine());
    }

    private IEnumerator FreePlayerInputCoroutine()
    {
        yield return new WaitForSeconds(moveTime);
        playerInputBusy = false;
    }

    private void SwitchTiles(Tile checkClickedTile, Tile checkTargetTile)
    {
        SwapGamePieces(checkClickedTile.GetComponentInChildren<GamePiece>(), checkTargetTile.GetComponentInChildren<GamePiece>());
    }

    private List<GamePiece> FindMatches(int startX, int startY, Vector2 searchDirection, int minLength = 3)
    {
        List<GamePiece> matches = new List<GamePiece>();
        GamePiece startPiece = null;

        if (IsWithinBounds(startX, startY))
        {
            startPiece = gamePieces[startX, startY];
        }

        if (startPiece == null) { return null; }

        matches.Add(startPiece);

        int nextX;
        int nextY;

        int maxValue = (width > height) ? width : height;

        for (int i = 1; i < maxValue - 1; i++)
        {
            nextX = startX + (int)Mathf.Clamp(searchDirection.x, -1, 1) * i;
            nextY = startY + (int)Mathf.Clamp(searchDirection.y, -1, 1) * i;

            if (!IsWithinBounds(nextX, nextY)) { break; }

            GamePiece nextPiece = gamePieces[nextX, nextY];
            if (nextPiece == null || nextPiece.MatchValue != startPiece.MatchValue || matches.Contains(nextPiece))
            {
                break;
            }
            matches.Add(nextPiece);
        }

        if (matches.Count >= minLength)
        {
            return matches;
        }
        return null;
    }

    private List<GamePiece> FindVerticalMatches(int startX, int startY, int minLength = 3)
    {
        List<GamePiece> upwardMatches = FindMatches(startX, startY, Vector2.up, 2);
        List<GamePiece> downwardMatches = FindMatches(startX, startY, Vector2.down, 2);

        upwardMatches ??= new();
        downwardMatches ??= new();

        List<GamePiece> combinedMatches = upwardMatches.Union(downwardMatches).ToList();

        return combinedMatches.Count >= minLength ? combinedMatches : null;
    }

    private List<GamePiece> FindHorizontalMatches(int startX, int startY, int minLength = 3)
    {
        List<GamePiece> rightMatches = FindMatches(startX, startY, Vector2.right, 2);
        List<GamePiece> leftMatches = FindMatches(startX, startY, Vector2.left, 2);

        rightMatches ??= new();
        leftMatches ??= new();

        List<GamePiece> combinedMatches = rightMatches.Union(leftMatches).ToList();

        return combinedMatches.Count >= minLength ? combinedMatches : null;
    }

    private void HighlightTileOff(int x, int y)
    {
        Image image = tiles[x, y].GetComponent<Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
    }

    private void HighlightTileOn(int x, int y, Color color)
    {
        Image image = tiles[x, y].GetComponent<Image>();
        image.color = color;
    }

    private void HighlightMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                HighlightMatchesAt(i, j);
            }
        }
    }

    private void HighlightMatchesAt(int x, int y)
    {
        HighlightTileOff(x, y);

        List<GamePiece> combinedMatches = FindMacthesAt(x, y);

        if (combinedMatches.Count > 0)
        {
            foreach (GamePiece piece in combinedMatches)
            {
                HighlightTileOn(piece.X, piece.Y, piece.GetComponent<Image>().color);
            }
        }
    }

    private List<GamePiece> FindMacthesAt(int x, int y, int minLength = 3)
    {
        List<GamePiece> horizontalMatches = FindHorizontalMatches(x, y, minLength);
        List<GamePiece> verticalMatches = FindVerticalMatches(x, y, minLength);

        horizontalMatches ??= new();
        verticalMatches ??= new();

        var combinedMatches = horizontalMatches.Union(verticalMatches).ToList();
        return combinedMatches;
    }

    private List<GamePiece> FindMacthesAt(List<GamePiece> pieces, int minLength = 3)
    {
        List<GamePiece> matches = new();
        foreach(GamePiece piece in pieces)
        {
            matches = matches.Union(FindMacthesAt(piece.X, piece.Y, minLength)).ToList();
        }

        return matches;
    }

    private List<GamePiece> FindAllMatches()
    {
        List<GamePiece> combinedMatches = new();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                List<GamePiece> matches = FindMacthesAt(i, j);
                combinedMatches = combinedMatches.Union(matches).ToList();
            }
        }
        return combinedMatches;
    }

    private void ClearPieceAt(int x, int y)
    {
        GamePiece pieceToClear = gamePieces[x, y];
        if (pieceToClear == null) { return; }

        gamePieces[x, y] = null;
        Destroy(pieceToClear.gameObject);

        HighlightTileOff(x, y);
    }

    private void ClearPieceAt(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                ClearPieceAt(piece.X, piece.Y);
            }
        }
    }

    private void ClearBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                ClearPieceAt(i, j);
            }
        }
    }

    private List<GamePiece> CollapseColumn(int column, float collapseTime = 0.1f)
    {
        List<GamePiece> movingPieces = new();

        for (int i = 0; i < height - 1; i++)
        {
            if (gamePieces[column, i] == null)
            {
                for (int j = i + 1; j < height; j++)
                {
                    GamePiece gamePiece = gamePieces[column, j];
                    if (gamePiece != null)
                    {
                        RectTransform rectTransform1 = gamePiece.RectTransform;

                        rectTransform1.SetParent(tiles[column, i].RectTransform);

                        gamePieces[column, i] = gamePieces[column, j];
                        gamePiece.SetCoord(column, i, collapseTime * (j - i));

                        if (!movingPieces.Contains(gamePieces[column, i]))
                        {
                            movingPieces.Add(gamePieces[column, i]);
                        }
                        gamePieces[column, j] = null;

                        break;
                    }
                }
            }
        }

        return movingPieces;
    }

    private List<GamePiece> CollapseColumn(List<GamePiece> pieces, float collapseTime = 0.1f)
    {
        List<GamePiece> movingPieces = new();
        List<int> columnsToCollapse = GetColumns(pieces);

        foreach (int column in columnsToCollapse)
        {
            movingPieces = movingPieces.Union(CollapseColumn(column)).ToList();
        }

        return movingPieces;
    }

    private List<int> GetColumns(List<GamePiece> pieces)
    {
        List<int> columns = new();

        foreach (GamePiece piece in pieces)
        {
            if (!columns.Contains(piece.X))
            {
                columns.Add(piece.X);
            }
        }

        return columns;
    }

    private void ClearAndRefillBoard(List<GamePiece> pieces)
    {
        StartCoroutine(ClearAndRefillBoardRoutine(pieces));
    }

    private IEnumerator ClearAndRefillBoardRoutine(List<GamePiece> pieces)
    {
        playerInputEnabled = false;
        playerInputBusy = true;

        List<GamePiece> matches = pieces;


        do
        {
            yield return StartCoroutine(ClearAndCollapseRoutine(matches));
            yield return null;

            yield return StartCoroutine(RefilRoutine());
            matches = FindAllMatches();
            yield return new WaitForSeconds(0.15f);
        }
        while (matches.Count != 0);
        

        playerInputEnabled = true;
        playerInputBusy = false;
    }

    private IEnumerator ClearAndCollapseRoutine(List<GamePiece> pieces)
    {
        List<GamePiece> movingPieces = new();
        List<GamePiece> matches = new();

        yield return new WaitForSeconds(0.25f);

        bool isFinished = false;
        while (!isFinished)
        {
            ClearPieceAt(pieces);
            yield return new WaitForSeconds(0.25f);
            movingPieces = CollapseColumn(pieces);
            yield return new WaitUntil(() => IsCollapsed(movingPieces));
            yield return new WaitForSeconds(0.15f);
            matches = FindMacthesAt(movingPieces);
            if (matches.Count == 0)
            {
                isFinished = true;
                break;
            }
            yield return StartCoroutine(ClearAndCollapseRoutine(matches));
        }
        yield return null;
    }

    private IEnumerator RefilRoutine()
    {
        FillBoard(-6);
        yield return null;
    }

    private bool IsCollapsed(List<GamePiece> pieces)
    {
        foreach (GamePiece piece in pieces)
        {
            if (piece == null) continue;
            if (piece.IsMoving || piece.RectTransform.anchoredPosition != Vector2.zero) return false;
        }
        return true;
    }
}
