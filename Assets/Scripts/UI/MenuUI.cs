using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuUI : MonoBehaviour 
{
    public RectTransform logo;
    public RectTransform btnTime;
    public RectTransform btnEndless;
    public RectTransform btnAchievements;
    public RectTransform btnLeaderboard;
    public RectTransform btnShare;
    public RectTransform btnNoAds;
    public GameObject noAds;
    public Sequence sequence;

    public Sequence ToggleMenu()
    {
        sequence = DOTween.Sequence();

        if (!this.gameObject.activeSelf)
        {
            Reset();

            // Scale
            sequence.Append(this.gameObject.transform.DOLocalMoveX(0f, 0.5f).SetEase(Ease.OutBack));
            sequence.Join(logo.DOScale(1f, 0.5f));
            sequence.Join(btnTime.DOScale(1f, 0.5f));
            sequence.Join(btnEndless.DOScale(1f, 0.5f));
            sequence.Join(btnAchievements.DOScale(1f, 0.5f));
            sequence.Join(btnLeaderboard.DOScale(1f, 0.5f));
            sequence.Join(btnShare.DOScale(1f, 0.5f));
            sequence.Join(btnNoAds.DOScale(1f, 0.5f));

        }
        else
        {
            sequence.Append(this.gameObject.transform.DOLocalMoveX(-700f, 0.5f));
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
            sequence.Append(noAds.transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBack));
        }
        else
        {
            sequence.Append(noAds.transform.DOLocalMoveX(600, 0.3f).SetEase(Ease.InBack));
            sequence.OnComplete(() => { noAds.SetActive(false); });
        }
        return sequence;
    }

    public void Reset() 
    {
        // Scale to 0
        this.gameObject.transform.DOLocalMoveX(-700f, 0.0f);
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