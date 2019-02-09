using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

// Class is used to Join GameManager with other UI Classes.
// Use this class to change text, color, or initialized tween sequences

public class UIManager : MonoBehaviour
{
    public GameObject MenuUI;
    public GameObject GameUI;
    public GameObject TutorialUI;
    public GameObject PreloadUI;
    public MenuUI menuUI;
    public GameUI gameUI;
    public TutorialUI tutorialUI;
    public PreloadUI preloadUI;
    private GameManager gm;
    private AdsManager banner;
    public AudioManager sound;
    private Sequence sequence;
    private string _confirmationState;

    private void Awake() 
    {
        gm = Object.FindObjectOfType<GameManager>();
        sound = Object.FindObjectOfType<AudioManager>();
        menuUI = MenuUI.GetComponent<MenuUI>();
        gameUI = GameUI.GetComponent<GameUI>();
        tutorialUI = TutorialUI.GetComponent<TutorialUI>();
        preloadUI = PreloadUI.GetComponent<PreloadUI>();
        banner = Object.FindObjectOfType<AdsManager>();
    }

    private void Start() 
    {
        // Refresh UI
        UpdateGameUI();
    }

    public void DisplayConfirmation(string callback)
    {
        sound.Play("pop_in");
        _confirmationState = callback;

        if (_confirmationState == "restart")
            gameUI.UpdateCofirmationTitle("Are you sure you\nwant to restart?");
        else if (_confirmationState == "menu")
            gameUI.UpdateCofirmationTitle("Are you sure you\nwant to exit?");

        gameUI.TogglePauseMenu().Play();
        gameUI.ToggleConfirmation().SetDelay(0.3f).Play();
    }

    public void HideConfirmation()
    {
        sound.Play("pop_out");
        gameUI.ToggleConfirmation().Play();
        gameUI.TogglePauseMenu().Play();
    }

    public void Confirm()
    {
        if (_confirmationState == "restart")
            RestartGame();
        else if (_confirmationState == "menu")
            GoToMenu();
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
        sequence.Prepend((gm.gameState == GameState.PAUSED) ? gameUI.ToggleConfirmation() : gameUI.ToggleGameOver());
        sequence.Append(gameUI.ToggleGame());
        sequence.Append(menuUI.ToggleMenu());
        sequence.Play();

        banner.DestroyBannerAd();
        gm.Reset();
    }

    public void RestartGame()
    {
        sequence = DOTween.Sequence();
        sequence.Prepend((gm.gameState == GameState.PAUSED) ? gameUI.ToggleConfirmation() : gameUI.ToggleGameOver());
        sequence.AppendInterval(0.2f);
        sequence.Play();

        banner.DestroyBannerAd();
        banner.ShowBannerAd();

        gm.Restart();
    }

    public void TogglePause()
    {
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
            gameUI.UpdateGameOverScore(newScore + "\n<size=50%><color=#B7B7B7>best " + highscore);
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

    public void HidePreloadAfterLoading()
    {
        sequence = DOTween.Sequence();
        sequence.PrependInterval(4f);
        sequence.Append(preloadUI.Outro());
        sequence.Append(menuUI.ToggleMenu());
        sequence.Play();
    }

    public void SwitchMusic()
    {
        GameObject btn = EventSystem.current.currentSelectedGameObject;
        GameObject on = btn.gameObject.transform.Find("on").gameObject;
        GameObject off = btn.gameObject.transform.Find("off").gameObject;

        on.SetActive(!on.activeSelf);
        off.SetActive(!off.activeSelf);

        sound.ToggleAllMusic();
    }

    public void SwitchSFX()
    {
        GameObject btn = EventSystem.current.currentSelectedGameObject;
        GameObject on = btn.gameObject.transform.Find("on").gameObject;
        GameObject off = btn.gameObject.transform.Find("off").gameObject;

        on.SetActive(!on.activeSelf);
        off.SetActive(!off.activeSelf);

        sound.ToggleAllSound();
    }
}
