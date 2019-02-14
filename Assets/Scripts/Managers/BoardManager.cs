using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using EasyMobile;
using TMPro;

public class BoardManager : MonoBehaviour
{
    [Header("Board Settings")]
    public int width;
    public int height;
    public int borderSize;

    [Header("Prefabs")]
    public GameObject tilePrefab;
    public GameObject dotPrefab;
    public GameObject linePrefab;
    public GameObject dotManager;
    public GameObject board;

    [Header("Board Tools")]
    private PrefabPool linePool;
    private Tile[,] AllTiles; // Use to call tiles from board
    private Dot[,] AllDots; // Use to track down specific dots on board.
    private List<Tile> ActiveTiles = new List<Tile>(); // All selected tiles
    private List<LineRenderer> ActiveLines = new List<LineRenderer>();

    [Header("Sequences")]
    private Sequence sequence;
    private Sequence collapseSequence;
    
    [Header("Managers")]
    private GameManager gm;
    private AudioManager sound;
    private void Awake()
    {
        // Time.timeScale = 0.1f;
        gm = Object.FindObjectOfType<GameManager>();
        sound = Object.FindObjectOfType<AudioManager>();
        linePool = new PrefabPool(linePrefab, board.transform, 5);
    }
    private void Start()
    {
        Tile.DotSelected += HandleDotConnections;
        Tile.SelectionEnded += HandleDotRelease;

        AllTiles = new Tile[width, height]; // Assign array size
        AllDots = new Dot[width, height]; // Assign array size

        SetupBoard();
        SetupCamera();
    }
    private void Update() 
    {
        if (Input.GetMouseButtonUp(0))
        {
            ActiveTiles.Clear();
        }

        var connections = ActiveTiles;
        // Get/return lines from pool until we are at the correct amount
        while (connections.Count > ActiveLines.Count)
        {
            ActiveLines.Add(linePool.Get().GetComponent<LineRenderer>());
        }
        while (connections.Count < ActiveLines.Count)
        {
            // TODO optimize to use last line instead of first
            // using [0] due to time consuming bug
            ReturnLine(ActiveLines[0]);
        }

        if (connections.Count > 0)
        {
            DrawConnections(connections);
        }
    }
    private void ReturnLine(LineRenderer line)
    {
        line.startColor = Color.clear;
        line.endColor = Color.clear;
        line.SetPosition(0, Vector3.zero);
        line.SetPosition(1, Vector3.zero);
        linePool.Return(line.gameObject);
        ActiveLines.Remove(line);
    }
    private void DrawConnections(List<Tile> connections)
    {
        // Keeps a reference to the last line
        // For usage outside of for loop
        LineRenderer line = null;


        for (var i = 0; i < connections.Count; i++)
        {
            line = ActiveLines[i];
            line.startColor = AllDots[connections[i].x, connections[i].y].color;
            line.endColor = AllDots[connections[i].x, connections[i].y].color;
            line.SetPosition(0, connections[i].transform.position);

            // If we're not at the last connection
            if (i != connections.Count - 1)
            {
                // Set second position to next connection
                line.SetPosition(1, connections[i + 1].transform.position);
            }
        }

        // Set the last line to draw it's final point to the pointer
        var pointer = GetPointerWorldPosition();
        line.SetPosition(1, pointer);
    }
    private Vector2 GetPointerWorldPosition()
    {
        var screen = Vector2.zero;
        if (Application.isEditor)
        {
            screen = Input.mousePosition;
        }
        else if (Input.touchCount > 0)
        {
            screen = Input.GetTouch(0).position;
        }
        else
        {
            return Vector2.zero;
        }
        return Camera.main.ScreenToWorldPoint(screen);
    }
    public void SetupBoard()
    {

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                tile.name = "Tile [" + x + "," + y + "]";
                AllTiles[x, y] = tile.GetComponent<Tile>();
                tile.transform.parent = board.transform;

                // Initialize Tile Component values
                AllTiles[x, y].Init(x, y);
            }
        }

    }
    public void SetupCamera()
    {
        Camera.main.transform.position = new Vector3((float)(width - 1) / 2f, (float)(height - 1) / 2f, -10f);
        float aspectRatio = (float)Screen.width / (float)Screen.height;
        float verticalSize = (float)height / 2f + (float)borderSize;
        float horizontalSize = ((float)width / 2f + (float)borderSize) / aspectRatio;
        Camera.main.orthographicSize = (verticalSize > horizontalSize) ? verticalSize : horizontalSize;
    }
    private DotProperties GetRandomDot()
    {
        return gm.themes[gm.selectedTheme].Dots[GetRandomIndex()];
    }
    public int GetRandomIndex()
    {
        int probability = Random.Range(0, 100);
        int[] probList;

        if (gm.IsGameMode(GameMode.TIME))
            probList = new int[] { 45, 20, 7 };
        else
            probList = new int[] { 45, 28, 17 };

        if (probability <= probList[2]) return 3;
        if (probability <= probList[1]) return 2;
        if (probability <= probList[0]) return 1;

        return 0;
    }
    private DotProperties GetDotByNumber(int number)
    {
        return gm.themes[gm.selectedTheme].Dots[number - 1];
    }
    private Dot PlaceDotOnBoard(int x, int y, DotProperties props, TweenCallback action = null)
    {
        GameObject randomDot = Instantiate(dotPrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
        randomDot.name = "Dot #" + props.number + " [" + x + "," + y + "]";
        randomDot.transform.parent = dotManager.transform;

        // Store Dots Instanceses into 2D Array
        AllDots[x, y] = randomDot.GetComponent<Dot>();
        AllDots[x, y].Init(x, y, props.number, props.color);
        return AllDots[x,y];
    }
    public void FillBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                PlaceDotOnBoard(x, y, GetRandomDot())
                    .SpawnFall(0.4f)
                    .SetDelay((0.075f * y) + 0.05f)
                    .OnComplete(()=>sound.Play("pop"))
                    .Play();
            }
        }
    }
    private void FillEmptyTiles()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (AllDots[x, y] == null)
                {
                    PlaceDotOnBoard(x, y, GetRandomDot()).SpawnFall(0.4f, 0.0f).Play();
                }
            }
        }
    }
    public void EmptyBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Destroy(AllDots[x, y].gameObject);
            }
        }
    }
    public void HandleDotConnections(Tile tile)
    {
        // If player hasn't started playing
        if (gm.IsGameState(GameState.GAMESTART))
            gm.SwitchGameState(GameState.PLAYING);

        bool isValid = false;

        if (ActiveTiles.Count == 0)
            isValid = (GetDot(tile).number == 1) ? true : false;
        else if (ActiveTiles[Mathf.Clamp(ActiveTiles.Count - 2, 0, int.MaxValue)] == tile)
            ActiveTiles.RemoveAt(ActiveTiles.Count - 1); // Undo Connections
        else
            isValid = IsValidNeighbor(ActiveTiles[ActiveTiles.Count - 1], tile);

        if (isValid) ActiveTiles.Add(tile);
    }
    public bool IsValidNeighbor(Tile tile1, Tile tile2)
    {
        if (tile1 != tile2)
        {   
            // ABS because we don't care about direction, easier compare
            var rowDiff = Mathf.Abs(tile2.y - tile1.y);
            var columnDiff = Mathf.Abs(tile2.x - tile1.x);

            // Not directly next to dot // If Vertical Horizontal and Diagnol
            if (rowDiff > 1 || columnDiff > 1) return false;

            if (GetDot(tile1).number != GetDot(tile2).number - 1)
                return (ActiveTiles.Count == 1 && GetDot(tile2).number == 1) ? true : false;
            
            if (ActiveTiles.Count > 1 && GetDot(tile1).number == 1) return false;

            return true;
        }

        return false;
    }
    public Dot GetDot(Tile tile)
    {
        return AllDots[tile.x, tile.y];
    }
    public void HandleDotRelease()
    {
        // If there aren't enough tiles, then just exit
        if (ActiveTiles.Count < 2) return;

        // Set Variables
        int[] dotsRemovedInColumns = new int[height];
        int targetIndex = ActiveTiles.Count - 1;
        Dot targetDot = GetDot(ActiveTiles[targetIndex]);
        sequence = DOTween.Sequence();

        // Add Scores and Time
        gm.AddScore(GetScoreByNumber(GetDot(ActiveTiles[targetIndex]).number));
        gm.AddTime(GetDot(ActiveTiles[targetIndex]).number);

        // 1. Mark all connected dots
        for(int i = 0; i < targetIndex; i++)
        {
            dotsRemovedInColumns[GetDot(ActiveTiles[i]).x]++;
            sequence.Append(GetDot(ActiveTiles[i]).MergeTo(ActiveTiles[i + 1].x, ActiveTiles[i + 1].y, 0.07f, 0.07f));
            AllDots[ActiveTiles[i].x, ActiveTiles[i].y] = null;
        }

        // 2. Empty last dot and create a new dot follwing the sequence.
        sequence.Append(GetDot(ActiveTiles[targetIndex]).Empty());
        sequence.OnComplete(()=>{ 
            PlaceDotOnBoard(targetDot.x, targetDot.y, GetDotByNumber(targetDot.number + 1)).SpawnPop(0.07f, 0, () =>
            {
                collapseSequence = DOTween.Sequence();
                CollapseColumn(dotsRemovedInColumns);
                collapseSequence.OnComplete(() => FillEmptyTiles());
                collapseSequence.Play();
            }).Play();
        }).Play();

        // Sound
        sound.Play((GetDot(ActiveTiles[targetIndex]).number >= 6) ? "mergePlus" : "merge");

        // Achievements
        gm.HandleAchivements(GetDot(ActiveTiles[targetIndex]).number);

        // Clean Up
        ActiveTiles.Clear();
    }
    public void CollapseColumn(int[] affectedColumns)
    {
        for (int x = 0; x < width; x++)
        {
            if (affectedColumns[x] == 0) continue;

            for (int y = 0; y < height; y++)
            {
                if (AllDots[x, y] == null)
                {
                    for (int i = y + 1; i < height; i++)
                    {
                        if (AllDots[x, i] != null)
                        {
                            collapseSequence.Insert(0, AllDots[x, i].MoveTo(x, y, 0.4f, 0.0f));
                            AllDots[x, y] = AllDots[x, i];
                            AllDots[x, i] = null;
                            break;
                        }
                    }
                }
            }
        }
    }
    public int GetScoreByNumber(int number)
    {
        return Mathf.RoundToInt((number ^ 2) * (number / 2));
    }
}