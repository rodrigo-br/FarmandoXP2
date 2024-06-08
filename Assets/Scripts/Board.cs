using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Range(1, 7)][SerializeField] private int width;
    [Range(1, 5)][SerializeField] private int height;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject[] gamePiecePrefabs;
    private RectTransform boardRectTransform;
    private float xOffset;
    private float yOffset;
    private Vector2 tileSize;
    private Tile[,] tiles;
    private GamePiece[,] gamePieces;

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

                PlaceGamePiece(gamePiece.GetComponent<GamePiece>(), x, y);
            }
        }
    }
}
