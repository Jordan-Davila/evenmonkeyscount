using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Dot : MonoBehaviour 
{
    public int x;
    public int y;
    public float posX;
    public float posY;
    public int number;
    public Color color;
    private Sequence sequence;

    public void Init(int x, int y, Vector2 position, int dotNum, Color dotColor)
    {
        SetCordinates(x,y);
        SetPosition(position);
        SetNumber(dotNum);
        SetColor(dotColor);
    }

    public Sequence SpawnFall(float duration, float delay = 0, TweenCallback action = null)
    {
        transform.localPosition = new Vector3(posX, 700, 0);
        transform.localScale = new Vector3(1f, 1f, 1f);
        sequence = DOTween.Sequence();
        sequence.PrependInterval(delay);
        sequence.Append(transform.DOLocalMove(new Vector2(posX,posY), duration).SetEase(Ease.OutBounce));
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

    public void SetPosition(Vector2 position)
    {
        posX = position.x;
        posY = position.y;
    }

    public void SetCordinates(int xIndex, int yIndex)
    {
        this.x = xIndex;
        this.y = yIndex;
    }

    public void SetColor(Color dotColor)
    {
        color = dotColor;
        this.gameObject.GetComponent<Image>().color = color;
    }

    public void SetNumber(int dotNum)
    {
        number = dotNum;
        GameObject numberText = this.transform.Find("Number").gameObject;
        TextMeshProUGUI text = numberText.GetComponent<TextMeshProUGUI>();
        text.text = number.ToString();
    }

    public Sequence MoveTo(int xIndex, int yIndex, Vector2 position, float duration, float delay)
    {
        sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMove(position, duration).SetEase(Ease.OutBounce).OnComplete(() => {
            SetPosition(position);
            SetCordinates(xIndex, yIndex);
        }));
        sequence.PrependInterval(delay);
        return sequence;
    }

    public Sequence MergeTo(int xIndex, int yIndex, Vector2 position, float duration, float delay)
    {
        sequence = DOTween.Sequence();

        // circle.sortingOrder = 0;

        sequence.PrependInterval(delay);
        sequence.Append(transform.DOLocalMove(position, duration));
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