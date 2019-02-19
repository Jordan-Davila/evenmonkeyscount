using UnityEngine;
using DG.Tweening;
using EasyMobile;

public enum GameState { MENU, PAUSED, PLAYING, GAMESTART, GAMEOVER, TUTORIAL };
public enum GameMode { TIME, ENDLESS };
public class GameManager : MonoBehaviour
{
    [Header("Game Properties")]
    public GameState gameState;
    public GameMode gameMode;
    public float maxTime;
    public float currentTime;
    public int perGameCoins = 0;

    [Header("Save Data")]
    public int score;
    public int timeHighScore;
    public int endlessHighScore;
    public int totalCoins = 0;
    public int selectedTheme = 0;
    public bool themePack = false;
    public bool ads;
    public bool isFirstTime = true; // for tutorial
    public bool[] purchasedThemes;

    [Header("Themes")]
    public ThemeProperties[] themes;

    [Header("Managers")]
    private BoardManager board;
    private UIManager ui;
    private DataManager data;
    private GSManager gs;
 
    private void Awake()
    {
        PlayerPrefs.DeleteAll();
        gs = Object.FindObjectOfType<GSManager>();
        data = Object.FindObjectOfType<DataManager>();
        board = Object.FindObjectOfType<BoardManager>();
        ui = Object.FindObjectOfType<UIManager>();

        // Setup Purchase Thems
        purchasedThemes = new bool[themes.Length];
        purchasedThemes[0] = true;

        // Set Tween
        DOTween.Init().SetCapacity(200, 125);
        // DOTween.SetTweensCapacity(200, 125);

        // Init EM runtime if needed (useful in case only this scene is built).
        if (!RuntimeManager.IsInitialized()) RuntimeManager.Init();
    }
 
    private void Start() 
    {
        // ui.menuUI.ToggleMenu().Play();
        currentTime = maxTime;

        // Check if Purchased ThemePack
        if (themePack) UnlockAllThemes(); 
    }
 
    private void Update() 
    {
        if (IsGameState(GameState.PLAYING))
        {
            if (gameMode == GameMode.TIME)
            {
                StartTimer();
 
                if (currentTime < 0)
                    GameOver("Out of Time");
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

        // ui.UpdateGameUI();
        board.FillBoard().Play();
    }
 
    public void GameOver(string title)
    {
        SwitchGameState(GameState.GAMEOVER);

        totalCoins += perGameCoins;
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
        ui.UpdateGameOverUI(score > highscore, score, highscore, perGameCoins, title);
    }
 
    public void Restart()
    {
        SwitchGameState(GameState.GAMESTART);
        Reset();
        // ui.UpdateGameUI();
        board.EmptyBoard().Play();
        board.FillBoard().SetDelay(0.4f).Play();
    }
 
    public void Reset()
    {
        score = 0;
        currentTime = maxTime;
        perGameCoins = 0;
    }
 
    public void StartTimer()
    {
        currentTime = currentTime - Time.deltaTime;
        ui.gameUI.UpdateTime(currentTime);
    }  
 
    public void AddTime(int targetNumber)
    {
        float addedTime = 0;

        if (IsGameMode(GameMode.TIME))
        {
            if (targetNumber <= 3)
                addedTime = 2;
           else if (targetNumber <= 6)
                addedTime = 3;
            else if (targetNumber <= 9)
                addedTime = 5;
            else
                addedTime = 7;

            currentTime += addedTime;
            ui.gameUI.UpdateTime(currentTime);
            ui.gameUI.DisplayAddedTime((int)addedTime);
        } 
    }
 
    public void AddScore(int points)
    {
        score += points;
        ui.gameUI.UpdateScore(score);
    }

    public void AddCoins(int coins)
    {
        perGameCoins += coins;
    }

    public void SaveData()
    {
        data.SaveData();
    }

    public void UnlockAllThemes()
    {
        themePack = true;

        for (int i = 0; i < purchasedThemes.Length; i++)
        {
            purchasedThemes[i] = true;
            themes[i].hasPurchased = true;
        }      

        ui.menuUI.themesPackButton.SetActive(false);
        ui.menuUI.UpdateThemePackText("Theme Pack is Active\n All themes are unlock");
        // ui,menuUI.theme
        SaveData();
    }

    public void HandleAchivements(int newDotNumber)
    {
        if (IsGameMode(GameMode.TIME))
        {
            if (newDotNumber == 6)
                gs.UnlockAchievement(EM_GameServicesConstants.Achievement_Connect_to_6);
            else if (newDotNumber == 9)
                gs.UnlockAchievement(EM_GameServicesConstants.Achievement_Connect_to_9);
            else if (newDotNumber == 13)
                gs.UnlockAchievement(EM_GameServicesConstants.Achievement_Connect_to_13);
            else if (newDotNumber > 13)
                gs.UnlockAchievement(EM_GameServicesConstants.Achievement_Connect_Beyond_13);

            if (score >= 250)
                gs.UnlockAchievement(EM_GameServicesConstants.Achievement_250_Points);
            if (score >= 500)
                gs.UnlockAchievement(EM_GameServicesConstants.Achievement_500_Points);
            if (score >= 1000)
                gs.UnlockAchievement(EM_GameServicesConstants.Achievement_1000_Points);
            if (score >= 1500)
                gs.UnlockAchievement(EM_GameServicesConstants.Achievement_1500_Points);
            if (score >= 2000)
                gs.UnlockAchievement(EM_GameServicesConstants.Achievement_2000_Points);
            if (score > 2000)
                gs.UnlockAchievement(EM_GameServicesConstants.Achievement_2000_Plus_Points);
        }
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