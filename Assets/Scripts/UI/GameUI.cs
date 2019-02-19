using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class GameUI : MonoBehaviour 
{
	[Header("Game Stats")]
    public GameObject timeCircle;
    public TextMeshProUGUI time;
    public TextMeshProUGUI score;
    public TextMeshProUGUI highscore;
	public GameObject addedTime;

    [Header("GameOver")]
    public GameObject gameOver;
    public TextMeshProUGUI gameOverTitle;
    public TextMeshProUGUI gameOverScore;
    public TextMeshProUGUI coins;
    public GameObject gameOverShare;
    public GameObject gameOverScreen;
    public GameObject stars;

    [Header("Pause Menu")]
    public GameObject pauseMenu;
    public GameObject pauseScreen;
    public GameObject music;
    public GameObject sound;
    public Sequence sequence;
    public float canvasWidth;
    public void StartPosition(float width) 
    {
        canvasWidth = width;
        gameObject.transform.localPosition = new Vector3(canvasWidth, 0, 0);
    }
    public Sequence ToggleGame()
    {
        sequence = DOTween.Sequence();

        if (!gameObject.activeSelf)
        {
            Reset();
            sequence.Append(this.gameObject.transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.InOutBack));
        }
        else
        {
            sequence.Append(this.gameObject.transform.DOLocalMoveX(canvasWidth, 0.5f).SetEase(Ease.InOutBack));
            sequence.OnComplete(() => { this.gameObject.SetActive(false); });
        }

        return sequence;
    }
    public Sequence TogglePauseMenu()
    {
        sequence = DOTween.Sequence();
        Image screen = pauseScreen.GetComponent<Image>();

        if (!pauseMenu.gameObject.activeSelf)
        {
            pauseMenu.SetActive(true);
            pauseScreen.SetActive(true);
            sequence.Append(this.gameObject.transform.DOLocalMoveX(-400f, 0.4f).SetEase(Ease.OutBack));
            sequence.Append(screen.DOFade(0.3f, 0.3f));
        }
        else
        {
            sequence.Append(screen.DOFade(0f, 0.3f));
            sequence.AppendCallback(() => pauseScreen.SetActive(false));
            sequence.Append(this.gameObject.transform.DOLocalMoveX(0, 0.4f).SetEase(Ease.InBack));
            sequence.OnComplete(() => pauseMenu.SetActive(false));
        }

        return sequence;
    }
    public Sequence ToggleGameOver()
    {
        sequence = DOTween.Sequence();
        Image screen = gameOverScreen.GetComponent<Image>();

        if (!gameOver.activeSelf)
        {
            gameOver.SetActive(true);
            gameOverScreen.SetActive(true);
            sequence.Append(gameOver.transform.DOLocalMoveY(0, 0.4f).SetEase(Ease.OutBack));
            sequence.Append(screen.DOFade(0.3f, 0.3f));
        }
        else
        {
            sequence.Append(screen.DOFade(0f, 0.3f));
            sequence.AppendCallback(() => gameOverScreen.SetActive(false));
            sequence.Append(gameOver.transform.DOLocalMoveY(1000, 0.4f).SetEase(Ease.InBack));
            sequence.OnComplete(() => { gameOver.SetActive(false); });
        }

        return sequence;
    }
    private void OnDisable() 
    {
        pauseMenu.SetActive(false);
        gameOver.SetActive(false);
    }
    private void Reset() 
    {
        this.gameObject.SetActive(true);
    }
    public void UpdateBackground(ThemeProperties themeProps)
    {
        Image image = this.gameObject.GetComponent<Image>();

        // Thumbnail
        if (themeProps.backgroundImage == null)
        {
            image.sprite = null;
            image.color = themeProps.backgroundColor;
        }
        else 
        {
            image.color = themeProps.backgroundColor;
            image.sprite = themeProps.backgroundImage;
        }
    }
    public void DisplayAddedTime(int value)
    {
        TextMeshProUGUI text = addedTime.GetComponent<TextMeshProUGUI>();
        RectTransform transform = addedTime.GetComponent<RectTransform>();

        text.DOFade(0f, 0f);
        transform.DOAnchorPos(new Vector2(0f, -245f), 0f);
        text.text = "+" + value.ToString();

        Sequence sequence = DOTween.Sequence();
        sequence.Append(text.DOFade(1f, 0.3f));
        sequence.Join(transform.DOAnchorPos(new Vector2(0f, -175f), 0.8f));
        sequence.Insert(0.3f, text.DOFade(0f, 0.3f));
        sequence.Play();
    }
    public void UpdateTime(float value) { time.text = Mathf.RoundToInt(value).ToString(); time.fontSize = 40; }
    public void UpdateTimeEndless() { time.text = "∞"; time.fontSize = 70; }
    public void UpdateScore(int value) { score.text = value.ToString(); }
    public void UpdateHighScore(int value) { highscore.text = value.ToString(); }
    public void UpdateGameOverTitle(string value) { gameOverTitle.text = value; }
    public void UpdateGameOverScore(string value) { gameOverScore.text = value; }
    public void UpdateGameOverHighScore(int value) { gameOverScore.text = value.ToString(); }
    public void UpdateGameOverCoins(int value) { coins.text = "+" + value.ToString(); }
    
}