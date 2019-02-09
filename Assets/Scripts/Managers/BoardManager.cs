using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using EasyMobile;

public class BoardManager : MonoBehaviour 
{
	[Header("Board Settings")]
	public int width;
	public int height;
	public int borderSize;
	private Tile StartTile;
	private List<Tile> TargetTiles = new List<Tile>();

    [Header("Dot Settings")]
    public DotList[] dotList; // List of all types of dots.

    [Header("Prefabs")]
	public GameObject tilePrefab;
	public GameObject dotPrefab;
	public GameObject dotManager;
    public GameObject cancelDrag;
    public GameObject board;
    public LineRenderer line;
    public Sequence mergingSequence;
    public Sequence collapsingSequence;
    private Tile[,] AllTiles; // Use to call tiles from board
    private Dot[,] AllDots; // Use to track down specific dots on board.
    private GameManager gm;
    private GSManager gs;
    private AudioManager sound;
    private void Awake() 
    {
        gm = Object.FindObjectOfType<GameManager>();
        gs = Object.FindObjectOfType<GSManager>();
        sound = Object.FindObjectOfType<AudioManager>();
    }
    private void Start() 
	{
        AllTiles = new Tile[width, height]; // Assign array size
        AllDots = new Dot[width, height]; // Assign array size

        SetupBoard();
		SetupCamera();
        SetupLineRenderer();
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
				AllTiles[x,y].Init(x,y);
            }
        }

	}
	public void SetupCamera()
	{
		Camera.main.transform.position = new Vector3((float) (width - 1) / 2f, (float) (height - 1) / 2f, -10f);
		float aspectRatio = (float) Screen.width / (float) Screen.height;
		float verticalSize = (float) height / 2f + (float) borderSize;
        float horizontalSize = ((float) width / 2f + (float) borderSize) / aspectRatio;
		Camera.main.orthographicSize = (verticalSize > horizontalSize) ? verticalSize : horizontalSize;
	}
    public void SetupLineRenderer()
    {
        // Add a Line Renderer to the GameObject
        // line = this.GetComponent<LineRenderer>();
        AnimationCurve curve = new AnimationCurve();
        
        // Width Curve
        curve.AddKey(0.0f, 0.0f);
        line.widthCurve = curve;
        line.widthMultiplier = 0.3f;

        // Set the width of the Line Renderer
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        
        // Set the number of vertex fo the Line Renderer
        line.startColor = Color.white;
        line.positionCount = 0;
    }
    public int GetScoreByNumber(int number)
    {
        return Mathf.RoundToInt((number ^ 2) * (number / 2));
    }
	private DotList GetRandomDot()
	{
        int index;
        int probability = Random.Range(0, 100);

        if (probability < 7)
            index = 3;
        else if (probability < 20)
            index = 2;
        else if (probability < 45)
            index = 1;    
        else
            index = 0;

		return dotList[index];
	}
    private DotList GetDotByNumber(int number)
    {
        return dotList[number - 1];
    }
    private void PlaceDotOnBoard(int x, int y, DotList props)
    {
        GameObject randomDot = Instantiate(dotPrefab, new Vector3(x,y,0), Quaternion.identity) as GameObject;
		randomDot.name = "Dot #" + props.number + " [" + x + "," + y + "]";
        randomDot.transform.parent = dotManager.transform;

		// Store Dots Instanceses into 2D Array
        AllDots[x,y] = randomDot.GetComponent<Dot>();
        AllDots[x,y].Init(x, y, props.number, props.color);
    }
	public void FillBoard()
	{
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
                PlaceDotOnBoard(x,y,GetRandomDot());
			}
		}
	}
    private void FillEmptyTiles()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (AllDots[x,y] == null)
                {
                    PlaceDotOnBoard(x, y, GetRandomDot());
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
    public void ClickTile(Tile tile)
    {
        int clickedNumber = (gm.IsGameState(GameState.GAMESTART) || gm.IsGameState(GameState.PLAYING)) ? AllDots[tile.xIndex, tile.yIndex].number : 0;

        if (StartTile == null && clickedNumber == 1)
        {
            // If player hasn't started playing
            if (gm.IsGameState(GameState.GAMESTART))
                gm.SwitchGameState(GameState.PLAYING);                
            
            StartTile = tile;
            line.positionCount++;
            line.SetPosition(line.positionCount - 1, tile.gameObject.transform.position);
            ToggleCancelDrag(true);
        }   
    }    
    public void DragTile(Tile currentTile)
    {
        if (currentTile.cancelTile == true)
            TargetTiles.Clear();

        if (StartTile != null)
        {
            int targetTileCount = TargetTiles.Count;
            int currentNumber = AllDots[currentTile.xIndex, currentTile.yIndex].number;
            
            // If this is the first dragTile, then make the prevTile the startTile
            Tile prevTile = (targetTileCount == 0) ? StartTile : TargetTiles[targetTileCount - 1];
            int prevNumber = AllDots[prevTile.xIndex, prevTile.yIndex].number;

            if (IsNextTo(prevTile,currentTile))
            {
                if (targetTileCount == 0)
                {
                    if ((currentNumber == 1 && prevNumber == 1) || (currentNumber == 2 && prevNumber == 1))
                    {
                        TargetTiles.Add(currentTile);
                        line.positionCount++;
                        line.SetPosition(line.positionCount - 1, currentTile.gameObject.transform.position);
                        sound.Play("drag");
                    }
                }
                else 
                {
                    if ((currentNumber == prevNumber + 1) && (currentNumber != 2 && prevNumber != 1))
                    {
                        TargetTiles.Add(currentTile);
                        line.positionCount++;
                        line.SetPosition(line.positionCount - 1, currentTile.gameObject.transform.position);
                        sound.Play("drag");
                    }
                }
            }
        }
    }
    public void ReleaseTile()
    {
        if (StartTile != null && TargetTiles.Count != 0 && gm.gameState == GameState.PLAYING)
            MergeToTargetDot(StartTile);

        // Reset
        StartTile = null; 
        TargetTiles.Clear();
        line.positionCount = 0;
        ToggleCancelDrag(false);
    }
    private void MergeToTargetDot(Tile clickedTile)
    {
        int targetTilesCount = TargetTiles.Count;

        Dot clickedDot = AllDots[clickedTile.xIndex,clickedTile.yIndex];
        Dot targetDot = AllDots[TargetTiles[targetTilesCount - 1].xIndex, TargetTiles[targetTilesCount - 1].yIndex];
        int newDot = targetDot.number + 1;

        mergingSequence = DOTween.Sequence();
        mergingSequence.Append(clickedDot.MergeTo(TargetTiles[0].xIndex, TargetTiles[0].yIndex, 0.05f, 0.0f));
        
        AllDots[clickedTile.xIndex, clickedTile.yIndex] = null;

        for (int i = 0; i < targetTilesCount-1; i++)
        {
            mergingSequence.Append(AllDots[TargetTiles[i].xIndex, TargetTiles[i].yIndex].MergeTo(TargetTiles[i+1].xIndex, TargetTiles[i+1].yIndex, 0.05f, 0.05f));
            AllDots[TargetTiles[i].xIndex, TargetTiles[i].yIndex] = null;
        }

        mergingSequence.OnComplete(() => {
            
            // Remove last target dot on board
            Destroy(targetDot.gameObject);
            // When Completed place dot on board
            PlaceDotOnBoard(targetDot.xIndex, targetDot.yIndex, GetDotByNumber(newDot));

            // Add Points and Time
            if (gm.gameMode == GameMode.TIME)
                gm.AddTime(targetDot.number);    

            gm.AddScore(GetScoreByNumber(targetDot.number + 1));

            // Start Sequence
            collapsingSequence = DOTween.Sequence();

            // Fill Empty Tiles
            foreach (int column in GetColumns(AllDots)) { CollapseColumn(column); }
            collapsingSequence.OnComplete(() => { FillEmptyTiles(); CanMove(); });
            collapsingSequence.Play();
        });

        // Sound
        sound.Play((newDot >= 6) ? "mergePlus" : "merge");

        // Achievements
        if (gm.IsGameMode(GameMode.TIME))
        {
            if (newDot == 6)
                gs.UnlockAchievement(EM_GameServicesConstants.Achievement_Connect_to_6);
            else if (newDot == 9)
                gs.UnlockAchievement(EM_GameServicesConstants.Achievement_Connect_to_9);
            else if (newDot == 13)
                gs.UnlockAchievement(EM_GameServicesConstants.Achievement_Connect_to_13);
            else if (newDot > 13)
                gs.UnlockAchievement(EM_GameServicesConstants.Achievement_Connect_Beyond_13);

            if (gm.score >= 250)
                gs.UnlockAchievement(EM_GameServicesConstants.Achievement_250_Points);
            if (gm.score >= 500)
                gs.UnlockAchievement(EM_GameServicesConstants.Achievement_500_Points);
            if (gm.score >= 1000)
                gs.UnlockAchievement(EM_GameServicesConstants.Achievement_1000_Points);
            if (gm.score >= 1500)
                gs.UnlockAchievement(EM_GameServicesConstants.Achievement_1500_Points);
            if (gm.score >= 2000)
                gs.UnlockAchievement(EM_GameServicesConstants.Achievement_2000_Points);
            if (gm.score > 2000)
                gs.UnlockAchievement(EM_GameServicesConstants.Achievement_2000_Plus_Points);
        }

        mergingSequence.Play();
        
    }
    public bool IsNextTo(Tile prevTile, Tile currentTile)
    {
        if (Mathf.Abs(prevTile.yIndex - currentTile.yIndex) == 1 && prevTile.xIndex == currentTile.xIndex) // Y Index Check
            return true;

        if (Mathf.Abs(prevTile.xIndex - currentTile.xIndex) == 1 && prevTile.yIndex == currentTile.yIndex) // X Index Check
            return true;

        if (Mathf.Abs(prevTile.yIndex - currentTile.yIndex) == 1 && Mathf.Abs(prevTile.xIndex - currentTile.xIndex) == 1) // Diagnol Check
            return true;
        
        return false;
    }
    public void CollapseColumn(int x)
    {
        for (int y = 0; y < height; y++)
        {
            if (AllDots[x,y] == null)
            {
                for (int i = y + 1; i < height; i++)
                {
                    if (AllDots[x,i] != null)
                    {
                        collapsingSequence.Append(AllDots[x,i].MoveTo(x, y, 0.05f,0.05f));
                        AllDots[x,y] = AllDots[x, i];
                        AllDots[x,i] = null;
                        break;
                    }
                }
            }
        }
    }
    public List<int> GetColumns(Dot[,] allDots)
    {
        List<int> columns = new List<int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (AllDots[x,y] == null)
                {
                    if (!columns.Contains(x))
                    {
                        columns.Add(x);
                    }
                }
            }
        }

        return columns;
    }
    public void ToggleCancelDrag(bool toggle = false)
    {
        Image cancelDragImage =  cancelDrag.GetComponent<Image>();
        TextMeshProUGUI cancelDragText = cancelDrag.GetComponentInChildren<TextMeshProUGUI>();

        if (toggle)
        {
            cancelDrag.SetActive(toggle);
            cancelDragImage.DOFade(0.3f, 0.2f);
            cancelDragText.DOFade(1f, 0.2f);    
        }
        else 
        {
            cancelDragImage.DOFade(0, 0.2f);
            cancelDragText.DOFade(0, 0.2f).OnComplete(() => {
                cancelDrag.SetActive(toggle);
            });
        }

        
    }
    public void CanMove()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (AllDots[x, y].number == 1)
                {
                    for (int x2 = 0; x2 < width; x2++)
                    {
                        for (int y2 = 0; y2 < height; y2++)
                        {
                            if ((Mathf.Abs(AllDots[x, y].xIndex - AllDots[x2, y2].xIndex) == 1 && Mathf.Abs(AllDots[x, y].yIndex - AllDots[x2, y2].yIndex) == 1) ||
                                (Mathf.Abs(AllDots[x, y].xIndex - AllDots[x2, y2].xIndex) == 1 && AllDots[x, y].yIndex == AllDots[x2, y2].yIndex) ||
                                (Mathf.Abs(AllDots[x, y].yIndex - AllDots[x2, y2].yIndex) == 1 && AllDots[x, y].xIndex == AllDots[x2, y2].xIndex))
                            {
                                if (AllDots[x2,y2].number == 2 || AllDots[x2,y2].number == 1)
                                {
                                    if (x != x2 || y != y2)
                                    {
                                        return;
                                    }
                                }
                            }               
                        }
                    }
                }
            }
        }

        gm.GameOver("You ran out of Moves");
    }
}

[System.Serializable]
public class DotList
{
    public string name;
    public int number;
    public Color color;
    public enum DotType
    {
        normal,
        bomb,
        swiper
    };
    public DotType dotType;
}