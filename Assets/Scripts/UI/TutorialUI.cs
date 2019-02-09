using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;

public class TutorialUI : MonoBehaviour
{
	public GameObject slides;
    public GameObject btnBack;
    public GameObject btnNext;
    public GameObject btnPlay;
    private RectTransform slidesRT;
    public Sequence sequence;
	private int _totalSlides;
    private int _currentSlide = 1;
	private void Awake() 
	{
		slidesRT = slides.GetComponent<RectTransform>();
		foreach (Transform slide in slides.transform)
			_totalSlides++;
	}
    public Sequence ToggleTutorial()
    {
        if (!this.gameObject.activeSelf)
        {
            Reset();
            sequence.Append(this.gameObject.transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBack));
        }
        else
        {
            sequence.Append(this.gameObject.transform.DOLocalMoveX(-600, 0.5f));
            sequence.OnComplete(() => { this.gameObject.SetActive(false); });
        }

        return sequence;
    }

    public void MoveLeft()
    {
        _currentSlide--;
		int width = (int)slidesRT.rect.width + 100;
        slides.transform.DOLocalMoveX(-((_currentSlide * width) - width), 0.5f).SetEase(Ease.OutBack);
        UpdateButtons();
    }

    public void MoveRight()
    {
        _currentSlide++;
        int width = (int)slidesRT.rect.width + 100;
        slides.transform.DOLocalMoveX(-((_currentSlide * width) - width), 0.5f).SetEase(Ease.OutBack);
        UpdateButtons();
    }

	public void UpdateButtons()
	{
        if (_currentSlide == 1)
		{
            btnBack.SetActive(false);
            btnNext.SetActive(true);
            btnPlay.SetActive(false);
		}
        else if (_currentSlide == _totalSlides)
		{
            btnBack.SetActive(true);
            btnNext.SetActive(false);
            btnPlay.SetActive(true);
		}
        else
		{
            btnBack.SetActive(true);
            btnNext.SetActive(true);
            btnPlay.SetActive(false);
		}
            
	}

    private void Reset()
    {
        this.gameObject.SetActive(true);
        this.gameObject.transform.DOLocalMoveX(700f, 0.0f);
    }

}