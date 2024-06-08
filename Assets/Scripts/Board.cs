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
        FillRandom();
    }

    private void SwapGamePieces(GamePiece gp1, GamePiece gp2)
    {
        if (gp1.IsMoving || gp2.IsMoving) { return; }
        StartCoroutine(SwapGamePiecesCoroutine(gp1, gp2));
    }

    private IEnumerator SwapGamePiecesCoroutine(GamePiece gp1, GamePiece gp2)
    {
        var (gp1X, gp1Y) = gp1.GetCoord();
        var (gp2X, gp2Y) = gp2.GetCoord();
        if (Mathf.Abs(gp1X - gp2X) + Mathf.Abs(gp1Y - gp2Y) > 1)
        {
            Debug.Log("Não pode mover");
            yield break;
        }

        RectTransform rectTransform1 = gp1.GetComponent<RectTransform>();
        RectTransform rectTransform2 = gp2.GetComponent<RectTransform>();

        rectTransform1.SetParent(tiles[gp2X, gp2Y].GetComponent<RectTransform>());
        rectTransform2.SetParent(tiles[gp1X, gp1Y].GetComponent<RectTransform>());

        gamePieces[gp1X, gp1Y] = gp2;
        gamePieces[gp2X, gp2Y] = gp1;
        gp1.SetCoord(gp2X, gp2Y, moveTime);
        gp2.SetCoord(gp1X, gp1Y, moveTime);

        yield return new WaitForSeconds(moveTime + 0.1f);

        HighlightMatchesAt(gp1.X, gp1.Y);
        HighlightMatchesAt(gp2.X, gp2.Y);
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

        gamePiece.GetComponent<RectTransform>().SetParent(tiles[x,y].GetComponent<RectTransform>(), false);
        gamePiece.SetCoord(x, y);
    }

    private void FillRandom()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject gamePiece = Instantiate(GetRandomGamePiece()) as GameObject;

                if (gamePiece == null) { return; }

                gamePieces[x, y] = gamePiece.GetComponent<GamePiece>();
                PlaceGamePiece(gamePiece.GetComponent<GamePiece>(), x, y);
            }
        }
    }

    public void ClickTile(Tile tile)
    {
        if (clickedTile == null)
        {
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
            if (nextPiece.MatchValue != startPiece.MatchValue || matches.Contains(nextPiece))
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
}
