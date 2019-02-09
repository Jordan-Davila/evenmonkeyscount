using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class GameUI : MonoBehaviour 
{
    [Header("Board")]
    public GameObject board;
    public GameObject dots;

	[Header("Game Stats")]
    public TextMeshProUGUI time;
    public TextMeshProUGUI gameModeTitle;
    public TextMeshProUGUI score;
    public TextMeshProUGUI highscore;
	public GameObject addedTime;

	[Header("Pause Menu")]
    public GameObject pauseMenu;

    [Header("GameOver")]
    public GameObject gameOver;
    public TextMeshProUGUI gameOverTitle;
    public TextMeshProUGUI gameOverScore;
    public GameObject gameOverShare;
    public GameObject stars;

    [Header("Confirmation")]
	public GameObject confirmation;
	public TextMeshProUGUI confirmationTitle;
    public Sequence sequence;

	public void UpdateTime(float value) {  time.text = Mathf.RoundToInt(value).ToString(); time.fontSize = 40; }
    public void UpdateTimeEndless() { time.text = "∞"; time.fontSize = 70; }
    public void UpdateScore(int value) { score.text = value.ToString(); }
    public void UpdateHighScore(int value) { highscore.text = value.ToString(); }
    public void UpdateGameOverTitle(string value) { gameOverTitle.text = value; }
    public void UpdateGameOverScore(string value) { gameOverScore.text = value; }
    public void UpdateGameOverHighScore(int value) { gameOverScore.text = value.ToString(); }
    public void UpdateCofirmationTitle(string value) { confirmationTitle.text = value; }
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
		sequence.Insert(0.3f,text.DOFade(0f,0.3f));
        sequence.Play();
	}

    public Sequence ToggleGame()
    {
        if (!this.gameObject.activeSelf)
        {
            Reset();
            sequence.Append(this.gameObject.transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBack));
            sequence.Join(board.transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBack));
            sequence.Join(dots.transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBack));
        }
        else
        {
            sequence.Append(this.gameObject.transform.DOLocalMoveX(600, 0.5f).SetEase(Ease.InBack));
            sequence.Join(board.transform.DOLocalMoveX(600, 0.5f).SetEase(Ease.InBack));
            sequence.Join(dots.transform.DOLocalMoveX(600, 0.5f).SetEase(Ease.InBack));
            sequence.OnComplete(() => 
            { 
                this.gameObject.SetActive(false);
                board.SetActive(false);
                dots.SetActive(false);
            });
        }

        return sequence;
    }

	public Sequence TogglePauseMenu()
	{	
        sequence = DOTween.Sequence();

        if (!pauseMenu.activeSelf)
        {
            pauseMenu.SetActive(true);
            sequence.Append(pauseMenu.transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBack));
        }
        else
        {
            sequence.Append(pauseMenu.transform.DOLocalMoveX(600, 0.3f).SetEase(Ease.InBack));
            sequence.OnComplete(() => { pauseMenu.SetActive(false); });
        }

        return sequence;
	}

    public Sequence ToggleGameOver()
    {
        sequence = DOTween.Sequence();

        if (!gameOver.activeSelf)
        {
            gameOver.SetActive(true);
            sequence.Append(gameOver.transform.DOLocalMoveX(0, 0.5f));
        }
        else
        {
            sequence.Append(gameOver.transform.DOLocalMoveX(600, 0.3f));
            sequence.OnComplete(() => { gameOver.SetActive(false); });
        }

        return sequence;
    }

    public Sequence ToggleConfirmation()
    {
        sequence = DOTween.Sequence();

        if (!confirmation.activeSelf)
        {
            confirmation.SetActive(true);
            sequence.Append(confirmation.transform.DOLocalMoveX(0, 0.5f));
        }
        else
        {
            sequence.Append(confirmation.transform.DOLocalMoveX(600, 0.3f));
            sequence.OnComplete(() => { confirmation.SetActive(false); });
        }

        return sequence;
    }

    private void Reset() 
    {
        this.gameObject.SetActive(true);
        board.SetActive(true);
        dots.SetActive(true);

        this.gameObject.transform.DOLocalMoveX(700f, 0.0f);
        board.transform.DOLocalMoveX(10f, 0.0f);
        dots.transform.DOLocalMoveX(10f, 0.0f);

        pauseMenu.SetActive(false);
        confirmation.SetActive(false);
        gameOver.SetActive(false);
    }
	
}