using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Dot : MonoBehaviour 
{
    public int x;
    public int y;
    public int number;
    public Color color;
    public SpriteRenderer circle;
    private Sequence sequence;

    public void Init(int x, int y, int dotNum, Color dotColor)
    {
        SetCordinates(x,y);
        SetNumber(dotNum);
        SetColor(dotColor);
    }

    public Sequence SpawnFall(float duration, float delay = 0, TweenCallback action = null)
    {
        transform.localPosition = new Vector3(x, 10, 0);
        transform.localScale = new Vector3(1f, 1f, 1f);
        sequence = DOTween.Sequence();
        sequence.PrependInterval(delay);
        sequence.Append(transform.DOMove(new Vector3(x, y, 0), duration).SetEase(Ease.OutBounce));
        sequence.OnComplete(action);
        return sequence;
    }

    public Sequence SpawnPop(float duration, float delay = 0, TweenCallback action = null)
    {
        
        sequence = DOTween.Sequence();
        sequence.PrependInterval(delay);
        sequence.Append(transform.DOScale(1f, duration).SetEase(Ease.InOutSine));
        sequence.OnComplete(action);
        return sequence;
    }

    public Sequence DestroyFall(float duration, float delay = 0, TweenCallback action = null)
    {
        sequence = DOTween.Sequence();
        sequence.PrependInterval(delay);
        sequence.Append(transform.DOMove(new Vector3(x, -10, 0), duration).SetEase(Ease.InOutBounce));
        sequence.OnComplete(() => Destroy(gameObject));
        return sequence;
    }

    public void SetCordinates(int xIndex, int yIndex)
    {
        this.x = xIndex;
        this.y = yIndex;
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

    public Sequence MoveTo(int xIndex, int yIndex, float duration, float delay)
    {
        sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(new Vector3(xIndex, yIndex, 0), duration).SetEase(Ease.OutBounce).OnComplete(() => SetCordinates(xIndex, yIndex)));
        sequence.PrependInterval(delay);
        return sequence;
    }

    public Sequence MergeTo(int xIndex, int yIndex, float duration, float delay)
    {
        sequence = DOTween.Sequence();

        circle.sortingOrder = 0;

        sequence.PrependInterval(delay);
        sequence.Append(transform.DOMove(new Vector3(xIndex,yIndex,0), duration));
        sequence.OnComplete(() => Empty().Play());
        return sequence;
    }

    public Sequence Empty()
    {
        sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(Vector3.zero, 0.07f).SetEase(Ease.InOutSine));
        sequence.OnComplete(() => Destroy(this.gameObject));
        return sequence;
    }

}