using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoardDeadlock))]
public class Board : MonoBehaviour
{
    [System.Serializable]
    public class StartingObject
    {
        public GameObject prefab;
        public int x;
        public int y;
    }

    [SerializeField] private StartingObject[] startingGamePieces;
    [Range(1, 7)][SerializeField] private int width;
    [Range(1, 5)][SerializeField] private int height;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject[] gamePiecePrefabs;
    [SerializeField] private GameObject[] adjacentBombPrefabs;
    [SerializeField] private GameObject[] thunderBombPrefabs;
    [SerializeField] private GameObject coloredBombPrefab;
    [SerializeField] private float moveTime = 0.2f;
    private RectTransform boardRectTransform;
    private float xOffset;
    private float yOffset;
    private Vector2 tileSize;
    private Tile[,] tiles;
    private GamePiece[,] gamePieces;
    private Tile clickedTile;
    private Tile targetTile;
    private GameObject clickedTileBomb;
    private GameObject targetTileBomb;
    private bool playerInputEnabled = true;
    private bool playerInputBusy = false;
    private ParticleManager particleManager;
    private int scoreMultiplier = 0;
    private BoardDeadlock boardDeadlock;
    [SerializeField] private RectTransform screenCanvas;
    private Vector2 defaultCanvasPosition;

    private void Awake()
    {
        boardDeadlock = GetComponent<BoardDeadlock>();
        particleManager = GameObject.FindWithTag("ParticleManager").GetComponent<ParticleManager>();
        tiles = new Tile[width, height];
        gamePieces = new GamePiece[width, height];
        boardRectTransform = GetComponent<RectTransform>();
        Rect tileRect = tilePrefab.GetComponent<RectTransform>().rect;
        tileSize = new Vector2(tileRect.width, tileRect.height);

        xOffset = (boardRectTransform.rect.width - (tileSize.x * width)) / 2 - boardRectTransform.rect.width / 2;
        yOffset = (boardRectTransform.rect.height - (tileSize.y * height)) / 2 - boardRectTransform.rect.height / 2;
        defaultCanvasPosition = screenCanvas.anchoredPosition;
    }

    private void Start()
    {
        AudioManager.Instance.PlayCheerSound();
        ScoreManager.Instance.CleanScoreColor();
    }

    public void SetupBoard()
    {
        SetupTiles();
        SetupGamePieces();
        FillBoard(-5);
        if (boardDeadlock.IsDeadlocked(gamePieces, 3))
        {
            ClearBoard();
            StartCoroutine(RefilRoutine());
        }
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

        if (tiles[gp2X, gp2Y] == null || tiles[gp1X, gp1Y] == null || rectTransform1 == null || rectTransform2 == null)
        {
            ReleaseTile();
            yield break;
        }
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
        List<GamePiece> colorMatches = new List<GamePiece>();

        if (IsColorBomb(gp1) && !IsColorBomb(gp2))
        {
            gp1.SetMatchValue(gp2.MatchValue);
            colorMatches = FindAllMatchValue(gp1.MatchValue);
        }
        else if (!IsColorBomb(gp1) && IsColorBomb(gp2))
        {
            gp2.SetMatchValue(gp1.MatchValue);
            colorMatches = FindAllMatchValue(gp2.MatchValue);
        }
        else if (IsColorBomb(gp1) && IsColorBomb(gp2))
        {
            foreach (GamePiece piece in gamePieces)
            {
                if (!colorMatches.Contains(piece))
                {
                    colorMatches.Add(piece);
                }
            }
        }

        if (gp1Matches.Count + gp2Matches.Count + colorMatches.Count > 0)
        {
            clickedTileBomb = DropBomb(gp1X, gp1Y, gp1Matches);
            targetTileBomb = DropBomb(gp2X, gp2Y, gp2Matches);
            ClearAndRefillBoard(gp1Matches.Union(gp2Matches).ToList());
            if (colorMatches.Count > 0)
            {
                AudioManager.Instance.PlayBombAdjacent();
                AudioManager.Instance.PlayBombThunder();
                screenCanvas.DOShakeAnchorPos(0.6f, 20, 10, randomnessMode: ShakeRandomnessMode.Harmonic);
                StartCoroutine(ResetCanvasPositionRoutine(0.61f));
            }
            ClearAndRefillBoard(colorMatches, isAllBombed: true);
            yield break;
        }
        StartCoroutine(SwapGamePiecesCoroutine(gp1, gp2, false));
    }

    private IEnumerator ResetCanvasPositionRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        screenCanvas.anchoredPosition = defaultCanvasPosition;
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

    private void SetupGamePieces()
    {
        foreach (StartingObject sPiece in startingGamePieces)
        {
            if (sPiece == null) return;
            GameObject piece = Instantiate(sPiece.prefab) as GameObject;
            MakeGamePiece(sPiece.x, sPiece.y, piece, -6);
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

    private void PlaceBomb(Bomb bomb, int x, int y)
    {
        if (bomb == null)
        {
            Debug.LogWarning("BOARD: Invalid GamePiece!");
            return;
        }

        bomb.RectTransform.SetParent(tiles[x, y].RectTransform, false);
        bomb.SetCoord(x, y);
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
        return MakeGamePiece(x, y, gamePieceGO, falseOffset);
    }

    private GamePiece MakeGamePiece(int x, int y, GameObject gamePieceGO, int falseOffset = 0)
    {
        if (gamePieceGO == null || !IsWithinBounds(x, y)) { return null; }

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

    private GameObject MakeBomb(GameObject prefab, int x, int y)
    {
        if (prefab == null || !IsWithinBounds(x, y)) return null;

        GameObject bombGO = Instantiate(prefab);
        Bomb bomb = bombGO.GetComponent<Bomb>();
        PlaceBomb(bomb, x, y);

        return bombGO;
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

        //HighlightTileOff(x, y);
    }

    private void ClearPieceAt(List<GamePiece> gamePieces, List<GamePiece> bombedPieces, bool isAllBombed = false)
    {
        if (gamePieces.Count > 0 && bombedPieces.Count == 0)
        {
            AudioManager.Instance.PlayBombSimple();
        }
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                int bonus = 0;

                ClearPieceAt(piece.X, piece.Y);
                if (gamePieces.Count >= 4)
                {
                    bonus = 20;
                }
                piece.ScorePoints(scoreMultiplier, bonus);
                if (particleManager == null) continue;
                if (bombedPieces.Contains(piece) || isAllBombed)
                {
                    particleManager.ClearBombFXAt(tiles[piece.X, piece.Y]);
                    screenCanvas.DOShakeAnchorPos(0.4f, 10, 10, randomnessMode: ShakeRandomnessMode.Harmonic);
                    StartCoroutine(ResetCanvasPositionRoutine(0.41f));
                }
                else
                {
                    particleManager.ClearPieceFXAt(tiles[piece.X, piece.Y]);
                }
            }
        }
    }

    private void ClearBoard()
    {
        screenCanvas.DOShakeAnchorPos(0.7f, 20, 15, randomnessMode: ShakeRandomnessMode.Harmonic);
        StartCoroutine(ResetCanvasPositionRoutine(0.71f));
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                ClearPieceAt(i, j);

                if (particleManager != null)
                {
                    particleManager.ClearBombFXAt(tiles[i, j]);
                }
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

    private void ClearAndRefillBoard(List<GamePiece> pieces, bool isAllBombed = false)
    {
        StartCoroutine(ClearAndRefillBoardRoutine(pieces, isAllBombed));
    }

    private IEnumerator ClearAndRefillBoardRoutine(List<GamePiece> pieces, bool isAllBombed = false)
    {
        playerInputEnabled = false;
        playerInputBusy = true;

        List<GamePiece> matches = pieces;
        scoreMultiplier = 0;
        do
        {
            scoreMultiplier++;
            yield return StartCoroutine(ClearAndCollapseRoutine(matches, isAllBombed));
            yield return null;

            yield return StartCoroutine(RefilRoutine());
            matches = FindAllMatches();
            yield return new WaitForSeconds(0.15f);
        }
        while (matches.Count != 0);
        
        if (boardDeadlock.IsDeadlocked(gamePieces, 3))
        {
            Debug.Log("DEAD LOCKEOU");
            yield return new WaitForSeconds(2.5f);
            Debug.Log("VAI EXPLODIR");
            ClearBoard();
            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(RefilRoutine());
        }

        playerInputEnabled = true;
        playerInputBusy = false;
    }

    private IEnumerator ClearAndCollapseRoutine(List<GamePiece> pieces, bool isAllBombed = false)
    {
        List<GamePiece> movingPieces = new();
        List<GamePiece> matches = new();

        bool isFinished = false;
        while (!isFinished)
        {
            List<GamePiece> bombedPieces = GetBombedPieces(pieces);
            pieces = pieces.Union(bombedPieces).ToList();

            bombedPieces = GetBombedPieces(pieces);
            pieces = pieces.Union(bombedPieces).ToList();

            ClearPieceAt(pieces, bombedPieces, isAllBombed);

            if (clickedTileBomb != null)
            {
                ActivateBomb(clickedTileBomb);
                clickedTileBomb = null;
            }
            if (targetTileBomb != null)
            {
                ActivateBomb(targetTileBomb);
                targetTileBomb = null;
            }

            yield return new WaitForSeconds(0.1f);
            movingPieces = CollapseColumn(pieces);
            yield return new WaitUntil(() => IsCollapsed(movingPieces));
            yield return new WaitForSeconds(0.15f);
            matches = FindMacthesAt(movingPieces);
            if (matches.Count == 0)
            {
                isFinished = true;
                break;
            }
            yield return new WaitForSeconds(0.15f);
            scoreMultiplier++;
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

    private List<GamePiece> GetRowPieces(int row)
    {
        List<GamePiece> pieces = new();

        for (int i = 0; i < width; i++)
        {
            if (gamePieces[i, row] == null) continue;
            pieces.Add(gamePieces[i, row]);
        }
        return pieces;
    }

    private List<GamePiece> GetColumnPieces(int column)
    {
        List<GamePiece> pieces = new();

        for (int i = 0; i < height; i++)
        {
            if (gamePieces[column, i] == null) continue;
            pieces.Add(gamePieces[column, i]);
        }
        return pieces;
    }

    public void ExplodeRandomColumn()
    {
        List<GamePiece> piecesToExplode = GetColumnPieces(Random.Range(0, width));

        ClearAndRefillBoard(piecesToExplode, true);
        AudioManager.Instance.PlayBombThunder();
    }

    public void ExplodeRandomRow()
    {
        List<GamePiece> piecesToExplode = GetRowPieces(Random.Range(0, height));

        ClearAndRefillBoard(piecesToExplode, true);
        AudioManager.Instance.PlayBombThunder();
    }

    private List<GamePiece> GetAdjacentPieces(int x, int y, int offset = 1)
    {
        List<GamePiece> pieces = new();

        for (int i = x - offset; i <= x + offset; i++)
        {
            for (int j = y - offset; j <= y + offset; j++)
            {
                if (IsWithinBounds(i, j))
                {
                    pieces.Add(gamePieces[i, j]);
                }
            }
        }
        return pieces;
    }

    private List<GamePiece> GetBombedPieces(List<GamePiece> pieces)
    {
        List<GamePiece> allPiecesToClear = new();
        foreach (GamePiece piece in pieces)
        {
            if (piece == null) { continue; }

            List<GamePiece> piecesToClear = new();

            Bomb bomb = piece.GetComponent<Bomb>();
            if (bomb == null) { continue; }

            switch (bomb.BombType)
            {
                case BombType.Thunder:
                    piecesToClear = GetColumnPieces(bomb.X);
                    piecesToClear = piecesToClear.Union(GetRowPieces(bomb.Y)).ToList();
                    AudioManager.Instance.PlayBombThunder();
                    break;
                case BombType.Bomb:
                    piecesToClear = GetAdjacentPieces(bomb.X, bomb.Y, 1);
                    AudioManager.Instance.PlayBombAdjacent();
                    break;
                case BombType.Color:
                    break;
                default:
                    break;
            }

            allPiecesToClear = allPiecesToClear.Union(piecesToClear).ToList();
        }

        return allPiecesToClear;
    }

    private bool IsCornerMatch(List<GamePiece> pieces)
    {
        bool vertical = false;
        bool horizontal = false;

        int xStart = -1;
        int yStart = -1;

        foreach (GamePiece piece in pieces)
        {
            if (piece == null) { continue; }

            if (xStart == -1 || yStart == -1)
            {
                xStart = piece.X;
                yStart = piece.Y;
                continue;
            }

            if (piece.X != xStart && piece.Y == yStart)
            {
                horizontal = true;
            }

            if (piece.X == xStart && piece.Y != yStart)
            {
                vertical = true;
            }
        }

        return horizontal && vertical;
    }

    private GameObject DropBomb(int x, int y, List<GamePiece> pieces)
    {
        GameObject bomb = null;
        if (pieces.Count < 4) return bomb;

        if (IsCornerMatch(pieces))
        {
            bomb = MakeBomb(adjacentBombPrefabs[(int)pieces[0].MatchValue], x, y);
        }
        else
        {
            if (pieces.Count >= 5)
            {
                bomb = MakeBomb(coloredBombPrefab, x, y);
                return bomb;
            }
            bomb = MakeBomb(thunderBombPrefabs[(int)pieces[0].MatchValue], x, y);
        }

        return bomb;
    }

    private void ActivateBomb(GameObject bombGO)
    {
        Bomb bomb = bombGO.GetComponent<Bomb>();
        int x = bomb.X;
        int y = bomb.Y;

        if (IsWithinBounds(x, y))
        {
            gamePieces[x, y] = bomb.GetComponent<GamePiece>();
        }
    }

    private List<GamePiece> FindAllMatchValue(MatchValueEnum matchValue)
    {
        List<GamePiece> foundPieces = new();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (gamePieces[i, j] == null) continue;
                if (gamePieces[i, j].MatchValue == matchValue)
                {
                    foundPieces.Add(gamePieces[i, j]);
                }
            }
        }
        return foundPieces;
    }

    private bool IsColorBomb(GamePiece gamePiece)
    {
        if (gamePiece == null) return false;
        Bomb bomb = gamePiece.GetComponent<Bomb>();

        if (bomb != null)
        {
            return (bomb.BombType == BombType.Color);
        }
        return false;
    }

    internal void CreateTwoBombs()
    {
        List<GameObject> bombOptions = new List<GameObject>
        {
            adjacentBombPrefabs[(int)MatchValueEnum.Purlple],
            coloredBombPrefab,
            thunderBombPrefabs[(int)MatchValueEnum.Purlple],
            thunderBombPrefabs[(int)MatchValueEnum.Purlple],
            adjacentBombPrefabs[(int)MatchValueEnum.Purlple],
        };

        int x = Random.Range(0, width);
        int y = Random.Range(0, height);
        ClearPieceAt(x, y);
        GameObject bomb = MakeBomb(bombOptions[Random.Range(0, bombOptions.Count)], x, y);
        ActivateBomb(bomb);
    }
}
