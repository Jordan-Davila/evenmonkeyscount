using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
    
    private Sequence sequence;
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

    public void DisplayAllThemes(ThemeProperties[] themes)
    {
        foreach (ThemeProperties theme in themes)
        {
            GameObject themeItem = Instantiate(themePrefab, Vector3.zero, Quaternion.identity) as GameObject;
            themeItem.name = theme.name;
            themeItem.transform.SetParent(themesContainer.transform,false);

            Image themeImage = themeItem.GetComponent<Image>();

            if (theme.thumbnailImage == null)
                themeImage.color = theme.thumbnailColor;
            else
                themeImage.sprite = theme.thumbnailImage;
        }
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

}