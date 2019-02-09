using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class PreloadUI : MonoBehaviour 
{
    public GameObject preload;
	public GameObject logo;
	public Image bg;
	public Sequence sequence;
	private void Awake() 
	{
        Intro().Play();
	}

    public Sequence Outro()
    {
        sequence = DOTween.Sequence();
        sequence.PrependInterval(1.5f);
        sequence.Append(logo.transform.DOScale(0f,0.5f));
        sequence.Join(bg.DOFade(0f,0.5f));
        sequence.OnComplete(() => { this.gameObject.SetActive(false); } );
		return sequence;
    }

	public Sequence Intro()
	{
        //Intro
        this.gameObject.SetActive(true);
        sequence = DOTween.Sequence();
        sequence.Append(logo.transform.DOScale(1f,0.5f));
		return sequence;
	}
}