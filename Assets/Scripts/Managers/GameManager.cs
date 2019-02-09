using UnityEngine;
using DG.Tweening;
using EasyMobile;

public enum GameState { MENU, PAUSED, PLAYING, GAMESTART, GAMEOVER, TUTORIAL };
public enum GameMode { TIME, ENDLESS };
public class GameManager : MonoBehaviour
{
    public GameState gameState;
    public GameMode gameMode;
    public int score;
    public int timeHighScore;
    public int endlessHighScore;
    public bool ads;
    public float maxTime;
    public float currentTime;
    public bool isFirstTime = true;
    private BoardManager board;
    private UIManager ui;
    private DataManager data;
    private GSManager gs;
 
    private void Awake()
    {
        gs = Object.FindObjectOfType<GSManager>();
        data = Object.FindObjectOfType<DataManager>();
        board = Object.FindObjectOfType<BoardManager>();
        ui = Object.FindObjectOfType<UIManager>();
        
        // Application.targetFrameRate = 60;
        // Init EM runtime if needed (useful in case only this scene is built).
        if (!RuntimeManager.IsInitialized()) RuntimeManager.Init();
    }
 
    private void Start() 
    {
        ui.menuUI.ToggleMenu().Play();
        currentTime = maxTime;
    }
 
    private void Update() 
    {
        if (IsGameState(GameState.PLAYING))
        {
            if (gameMode == GameMode.TIME)
            {
                StartTimer();
 
                if (currentTime < 0)
                    GameOver("A Monkey Can Count Better");
            }                
             
        }
    }
     
    public void SwitchGameState(GameState state)
    {
        gameState = state;
    }
 
    public void SwitchGameMode(GameMode mode)
    {
        gameMode = mode;
    }

    public bool IsGameState(GameState state)
    {
        return (gameState == state) ? true : false;
    }

    public bool IsGameMode(GameMode mode)
    {
        return (gameMode == mode) ? true : false;
    }

    public void GameStart(GameMode mode)
    {
        SwitchGameState(GameState.GAMESTART);
        SwitchGameMode(mode);

        ui.UpdateGameUI();
        board.FillBoard();
        ui.sound.Play("pop");
    }
 
    public void GameOver(string title)
    {
        SwitchGameState(GameState.GAMEOVER);
 
        int highscore = (IsGameMode(GameMode.TIME)) ? timeHighScore : endlessHighScore;
 
        if (score > highscore)
        {
            // Save Scores
            if (IsGameMode(GameMode.TIME)) timeHighScore = score;
            else endlessHighScore = score;

            // Save to Cloud
            data.SaveData();
        }

        // Leaderboard
        gs.ReportToLeaderboard((IsGameMode(GameMode.TIME) ? EM_GameServicesConstants.Leaderboard_Time : EM_GameServicesConstants.Leaderboard_Endless), score);

        // Update UI Animate
        ui.UpdateGameOverUI(score > highscore, score, highscore, title);
    }
 
    public void Restart()
    {
        SwitchGameState(GameState.GAMESTART);
        Reset();
        ui.UpdateGameUI();
        ui.sound.Play("pop");
        board.FillBoard();
    }
 
    public void Reset()
    {
        score = 0;
        currentTime = maxTime;
        board.EmptyBoard();
    }
 
    public void StartTimer()
    {
        currentTime = currentTime - Time.deltaTime;
        ui.gameUI.UpdateTime(currentTime);
    }  
 
    public void AddTime(int time)
    {
        currentTime += time;
        ui.gameUI.UpdateTime(currentTime);
        ui.gameUI.DisplayAddedTime(time);
    }
 
    public void AddScore(int points)
    {
        score += points;
        ui.gameUI.UpdateScore(score);
    }
    public void RemoveAds()
    {
        ads = false;
        data.SaveData();
        ui.ToggleNoAds();

        // Update UI
        ui.UpdateMenuUI();
    }
}