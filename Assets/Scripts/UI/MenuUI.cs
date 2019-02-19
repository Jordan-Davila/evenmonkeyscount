using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class MenuUI : MonoBehaviour 
{
    [Header("Main Menu")]
    public RectTransform logo;
    public RectTransform btnTime;
    public RectTransform btnEndless;
    public RectTransform btnAchievements;
    public RectTransform btnLeaderboard;
    public RectTransform btnShare;
    public RectTransform btnNoAds;
    public TextMeshProUGUI bestScore;
    public TextMeshProUGUI totalCoins;
    
    [Header("No Ads Menu")]
    public GameObject noAds;

    [Header("Options Menu")]
    public GameObject optionsMenu;
    public GameObject optionsScreen;
    public GameObject music;
    public GameObject sound;

    [Header("Themes Menu")]
    public GameObject themesMenu;
    public GameObject themePrefab;
    public GameObject themesScreen;
    public GameObject themesContainer;
    public GameObject themesConfirmationPanel;
    public GameObject themesConfirmationScreen;
    public GameObject themesPackButton;
    public TextMeshProUGUI themesConfirmationTitle;
    public TextMeshProUGUI themesConfirmationCoins;
    public TextMeshProUGUI themesConfirmationNeedMoreCoins;
    public TextMeshProUGUI themePackText;
    
    public Button getTheme;
    private Sequence sequence;
    public UIManager ui;
    public float canvasWidth;
    public void StartPosition(float width)
    {
        canvasWidth = width;
    }
    public Sequence ToggleMenu()
    {
        sequence = DOTween.Sequence();

        if (!this.gameObject.activeSelf)
        {
            Reset();

            // Scale
            sequence.Append(this.gameObject.transform.DOLocalMoveX(0f, 0.5f).SetEase(Ease.InOutBack));
            sequence.Join(logo.DOScale(1f, 0.4f));
            sequence.Join(btnTime.DOScale(1f, 0.4f));
            sequence.Join(btnEndless.DOScale(1f, 0.4f));
            sequence.Join(btnAchievements.DOScale(1f, 0.4f));
            sequence.Join(btnLeaderboard.DOScale(1f, 0.4f));
            sequence.Join(btnShare.DOScale(1f, 0.4f));
            sequence.Join(btnNoAds.DOScale(1f, 0.4f));

        }
        else
        {
            sequence.Append(this.gameObject.transform.DOLocalMoveX(-1 * canvasWidth, 0.5f).SetEase(Ease.InOutBack));
            sequence.OnComplete(() => { this.gameObject.SetActive(false); });
        }
        return sequence;
    }

    public Sequence ToggleNoAds()
    {
        sequence = DOTween.Sequence();

        if (!noAds.activeSelf)
        {
            noAds.SetActive(true);
            sequence.Append(noAds.transform.DOLocalMoveY(0, 0.4f).SetEase(Ease.OutBack));
        }
        else
        {
            sequence.Append(noAds.transform.DOLocalMoveY(1000, 0.4f).SetEase(Ease.InBack));
            sequence.OnComplete(() => { noAds.SetActive(false); });
        }
        return sequence;
    }

    public Sequence ToggleThemesMenu()
    {
        sequence = DOTween.Sequence();
        Image screen = themesScreen.GetComponent<Image>();
        
        if (!themesMenu.gameObject.activeSelf)
        {
            themesMenu.SetActive(true);
            themesScreen.SetActive(true);
            sequence.Append(this.gameObject.transform.DOLocalMoveX(400f, 0.4f).SetEase(Ease.OutBack));
            sequence.Append(screen.DOFade(0.3f, 0.2f));
        }
        else
        {
            sequence.Append(screen.DOFade(0f, 0.2f));
            sequence.AppendCallback(() => themesScreen.SetActive(false));
            sequence.Append(this.gameObject.transform.DOLocalMoveX(0, 0.2f).SetEase(Ease.InBack));
            sequence.OnComplete(() => themesMenu.SetActive(false));
        }

        return sequence;
    }

    public Sequence DisplayThemesConfirmationPanel(ThemeProperties theme)
    {
        sequence = DOTween.Sequence();
        Image screen = themesConfirmationScreen.GetComponent<Image>();
        themesConfirmationTitle.text = "BUY " + theme.name;
        themesConfirmationCoins.text = "-" + theme.coins;
        getTheme.onClick.AddListener(delegate{ ui.BuyTheme(theme); });

        if (!themesConfirmationPanel.gameObject.activeSelf)
        {
            themesConfirmationPanel.SetActive(true);
            themesConfirmationScreen.SetActive(true);
            sequence.Append(themesConfirmationPanel.transform.DOLocalMoveX(-400f, 0.4f).SetEase(Ease.OutBack));
            sequence.Append(screen.DOFade(0.3f, 0.3f));
        }
        return sequence;
    }

    public Sequence HideThemesConfirmationPanel()
    {
        sequence = DOTween.Sequence();
        Image screen = themesConfirmationScreen.GetComponent<Image>();
        themesConfirmationNeedMoreCoins.DOFade(0f, 0f);

        if (themesConfirmationPanel.gameObject.activeSelf)
        {
            getTheme.onClick.RemoveAllListeners();
            sequence.Append(screen.DOFade(0f, 0.3f));
            sequence.AppendCallback(() => themesConfirmationScreen.SetActive(false));
            sequence.Append(themesConfirmationPanel.transform.DOLocalMoveX(-800f, 0.4f).SetEase(Ease.InBack));
            sequence.OnComplete(() => themesConfirmationPanel.SetActive(false));
        }
        
        return sequence;
    }

    public Sequence ToggleOptionsMenu()
    {
        sequence = DOTween.Sequence();
        Image screen = optionsScreen.GetComponent<Image>();

        if (!optionsMenu.gameObject.activeSelf)
        {
            optionsMenu.SetActive(true);
            optionsScreen.SetActive(true);
            sequence.Append(this.gameObject.transform.DOLocalMoveX(-400f, 0.4f).SetEase(Ease.OutBack));
            sequence.Append(screen.DOFade(0.3f, 0.3f));
        }
        else
        {
            sequence.Append(screen.DOFade(0f, 0.3f));
            sequence.AppendCallback(() => optionsScreen.SetActive(false));
            sequence.Append(this.gameObject.transform.DOLocalMoveX(0, 0.4f).SetEase(Ease.InBack));
            sequence.OnComplete(() => optionsMenu.SetActive(false));
        }

        return sequence;
    }
    public void Reset() 
    {
        // Scale to 0
        logo.DOScale(0f, 0.0f);
        btnTime.DOScale(0f, 0.0f);
        btnEndless.DOScale(0f, 0.0f);
        btnAchievements.DOScale(0f, 0.0f);
        btnLeaderboard.DOScale(0f, 0.0f);
        btnShare.DOScale(0f, 0.0f);
        btnNoAds.DOScale(0f, 0.0f);
        this.gameObject.SetActive(true);
        noAds.SetActive(false);
    }

    public void UpdateScore(int value) { bestScore.text = value.ToString(); }
    public void UpdateCoins(int value) { totalCoins.text = value.ToString(); }
    public void UpdateThemePackText(string value)
    { 
        themePackText.text = value; 
        RectTransform rt = themePackText.gameObject.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector3(0, 70f, 0);
    }

}