using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Dot : MonoBehaviour 
{
    public int xIndex;
    public int yIndex;
    public int number;
    public Color color;
    public SpriteRenderer circle;
    private Sequence sequence;

    public void Init(int x, int y, int dotNum, Color dotColor)
    {
        SetCordinates(x,y);
        SetNumber(dotNum);
        SetColor(dotColor);
        this.transform.DOScale(1f,Random.Range(0.05f,0.8f)).SetEase(Ease.InOutSine);
    }

    public void SetCordinates(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }

    public void SetColor(Color dotColor)
    {
        color = dotColor;
        circle.color = color;
    }

    public void SetNumber(int dotNum)
    {
        number = dotNum;
        GameObject numberText = this.transform.Find("Number").gameObject;
        TextMeshPro text = numberText.GetComponent<TextMeshPro>();
        text.text = number.ToString();
    }

    public Sequence MoveTo(int x, int y, float duration, float delay)
    {
        sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(new Vector3(x, y, 0), duration).OnComplete(() => { this.SetCordinates(x, y); }));
        sequence.PrependInterval(delay);
        return sequence;
    }

    public Sequence MergeTo(int x, int y, float duration, float delay)
    {
        sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(new Vector3(x,y,0), duration).OnComplete(() => { Destroy(this.gameObject); }));
        sequence.PrependInterval(delay);
        return sequence;
    }

    public void Empty()
    {
        this.transform.DOScale(Vector3.zero, Random.Range(0.05f, 0.8f)).SetEase(Ease.InOutSine).OnComplete(() => { Destroy(this.gameObject); });
    }

}