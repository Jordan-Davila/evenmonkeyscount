using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

// Class is used to Join GameManager with other UI Classes.
// Use this class to change text, color, or initialized tween sequences

public class UIManager : MonoBehaviour
{
    [Header("User Interfaces")]
    public GameObject MenuUI;
    public GameObject GameUI;
    public GameObject TutorialUI;
    public GameObject PreloadUI;
    
    [Header("UI Scripts")]
    [HideInInspector]
    public MenuUI menuUI;
    [HideInInspector]
    public GameUI gameUI;
    [HideInInspector]
    public TutorialUI tutorialUI;
    [HideInInspector]
    public PreloadUI preloadUI;

    [Header("Canvas")]
    public RectTransform canvas;
    private float canvasWidth;

    [Header("Managers")]
    private GameManager gm;
    private AdsManager banner;
    private AudioManager sound;
    private BoardManager board;
    private Sequence sequence;

    private void Awake() 
    {
        // Game Managers
        gm = Object.FindObjectOfType<GameManager>();
        board = Object.FindObjectOfType<BoardManager>();
        sound = Object.FindObjectOfType<AudioManager>();

        // UI Objects
        menuUI = MenuUI.GetComponent<MenuUI>();
        gameUI = GameUI.GetComponent<GameUI>();
        tutorialUI = TutorialUI.GetComponent<TutorialUI>();
        preloadUI = PreloadUI.GetComponent<PreloadUI>();

        // Ad
        banner = Object.FindObjectOfType<AdsManager>();
    }
    private void Start() 
    {
        // Refresh UI
        UpdateGameUI();

        // Get Canvas Width
        canvasWidth = canvas.rect.width;

        // Display All Themes
        menuUI.DisplayAllThemes(gm.themes);

        // Start Positions
        gameUI.StartPosition(canvasWidth);
        menuUI.StartPosition(canvasWidth);
    }
    public void GoToGame(string mode)
    {
        sequence = DOTween.Sequence();
        
        if (gm.isFirstTime)
        {
            if (gm.IsGameState(GameState.MENU))
            {
                gm.SwitchGameMode((mode == "time") ? GameMode.TIME : GameMode.ENDLESS);
                sequence.Append(menuUI.ToggleMenu());
                sequence.Join(tutorialUI.ToggleTutorial());
                gm.SwitchGameState(GameState.TUTORIAL);    
            }
            else if (gm.IsGameState(GameState.TUTORIAL))
            {
                gm.isFirstTime = false;
                sequence.Append(tutorialUI.ToggleTutorial());
                sequence.Join(gameUI.ToggleGame());
                sequence.AppendInterval(1f);
                sequence.OnComplete(() => { gm.GameStart(gm.gameMode); tutorialUI.gameObject.SetActive(false); banner.ShowBannerAd(); });    
            }
        }
        else
        {
            gm.SwitchGameMode((mode == "time") ? GameMode.TIME : GameMode.ENDLESS);
            sequence.Append(menuUI.ToggleMenu());
            sequence.Join(gameUI.ToggleGame());
            sequence.OnComplete(() => { gm.GameStart(gm.gameMode); banner.ShowBannerAd(); });
        }
        
        sequence.Play();   
    }
    public void GoToMenu()
    {
        sequence = DOTween.Sequence();
        
        sequence.Prepend((gm.gameState == GameState.PAUSED) ? gameUI.TogglePauseMenu() : gameUI.ToggleGameOver());
        sequence.Insert(0.3f,board.EmptyBoard().Play());
        sequence.Append(gameUI.ToggleGame());
        sequence.Join(menuUI.ToggleMenu());
        sequence.Play();

        banner.DestroyBannerAd();
        gm.Reset();
    }

    public void RestartGame()
    {
        sequence = DOTween.Sequence();
        sequence.Prepend((gm.gameState == GameState.PAUSED) ? gameUI.TogglePauseMenu() : gameUI.ToggleGameOver());
        sequence.OnComplete(() => {
            banner.DestroyBannerAd();
            banner.ShowBannerAd();
            gm.Restart();
        });
        sequence.Play();
    }
    public void TogglePause()
    {
        if (board.isMoving) return;

        gm.SwitchGameState((!gameUI.pauseMenu.activeSelf) ? GameState.PAUSED : GameState.PLAYING);
        sound.Play((gameUI.pauseMenu.activeSelf) ? "pop_out" : "pop_in");
        gameUI.TogglePauseMenu().Play();
    }
    public void UpdateGameUI()
    {
        // Update UI
        if (gm.IsGameMode(GameMode.TIME))
        {
            gameUI.UpdateTime(gm.currentTime);
            gameUI.UpdateScore(gm.score);
            gameUI.UpdateHighScore(gm.timeHighScore);
        }
        else
        {
            gameUI.UpdateTimeEndless();
            gameUI.UpdateScore(gm.score);
            gameUI.UpdateHighScore(gm.endlessHighScore);
        }
    }
    public void UpdateGameOverUI(bool isNewScore, int newScore, int highscore, string title)
    {
        if (isNewScore)
        {
            gameUI.UpdateGameOverTitle("New High\nScore!");
            gameUI.UpdateGameOverScore(newScore.ToString());
            gameUI.UpdateHighScore(newScore);
            gameUI.gameOverShare.SetActive(true);
            gameUI.stars.SetActive(true);
            sound.Play("highscore");
        }
        else
        {
            gameUI.UpdateGameOverTitle(title);
            gameUI.UpdateGameOverScore(newScore + "\n<size=45%><color=#EF6673>best " + highscore);
            gameUI.UpdateHighScore(highscore);
            gameUI.gameOverShare.SetActive(false);
            gameUI.stars.SetActive(false);
            sound.Play("gameover");
        }

        gameUI.ToggleGameOver().Play();
    }
    public void UpdateMenuUI()
    {
        if (!gm.ads)
        {
            menuUI.noAds.gameObject.SetActive(false);
            menuUI.btnNoAds.gameObject.SetActive(false);
        }
    }
    public void ToggleNoAds()
    {
        sound.Play((menuUI.noAds.activeSelf) ? "pop_out" : "pop_in");
        menuUI.ToggleNoAds().Play();
    }
    public void ToggleThemesMenu()
    {
        sound.Play((menuUI.noAds.activeSelf) ? "pop_out" : "pop_in");
        menuUI.ToggleThemesMenu().Play();
    }
    public void ToggleOptionsMenu()
    {
        sound.Play((menuUI.noAds.activeSelf) ? "pop_out" : "pop_in");
        menuUI.ToggleOptionsMenu().Play();
    }
    public void HidePreloadAfterLoading()
    {
        sequence = DOTween.Sequence();
        sequence.PrependInterval(4f);
        sequence.Append(preloadUI.Outro());
        sequence.Append(menuUI.ToggleMenu());
        sequence.Play();
    }
    public void ToggleMusic()
    {
        GameObject onMain = menuUI.music.transform.Find("on").gameObject;
        GameObject onGame = gameUI.music.transform.Find("on").gameObject;
        GameObject offMain = menuUI.music.transform.Find("off").gameObject;
        GameObject offGame = gameUI.music.transform.Find("off").gameObject;

        if (sound.isMusicMute)
        {
            
            onMain.SetActive(true);
            onGame.SetActive(true);
            offMain.SetActive(false);
            offGame.SetActive(false);
        }
        else
        {
            onMain.SetActive(false);
            onGame.SetActive(false);
            offMain.SetActive(true);
            offGame.SetActive(true);
        }

        sound.ToggleAllMusic();
    }
    public void ToggleSFX()
    {
        GameObject onMain = menuUI.sound.transform.Find("on").gameObject;
        GameObject onGame = gameUI.sound.transform.Find("on").gameObject;
        GameObject offMain = menuUI.sound.transform.Find("off").gameObject;
        GameObject offGame = gameUI.sound.transform.Find("off").gameObject;

        if (sound.isSoundMute)
        {
            onMain.SetActive(true);
            onGame.SetActive(true);
            offMain.SetActive(false);
            offGame.SetActive(false);   
        }
        else
        {
            onMain.SetActive(false);
            onGame.SetActive(false);
            offMain.SetActive(true);
            offGame.SetActive(true);
        }

        sound.ToggleAllSound();
    }
}
