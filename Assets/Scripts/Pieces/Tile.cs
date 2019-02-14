using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Tile : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
{
	public int x;
	public int y;
    private static bool hasSelected;

	public void Init(int xIndex, int yIndex)
	{
		this.x = xIndex;
		this.y = yIndex;
	}

    // On Click
    public delegate void OnSelectionStarted();
    public static event OnSelectionStarted SelectionStarted;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!hasSelected)
        {
            hasSelected = true;
            // Debug.Log("OnSelectionStarted");
            if (SelectionStarted != null)
            {
                SelectionStarted();
            }
            OnPointerEnter(eventData);
        }
    }

    // On Hover
    public delegate void OnDotSelected(Tile tile);
    public static event OnDotSelected DotSelected;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hasSelected)
        {
            // Debug.Log("OnDotSelected " + this.name);
            if (DotSelected != null)
            {
                DotSelected(this);
            }
        }
    }

    // On UnClick
    public delegate void OnSelectionEnded();
    public static event OnSelectionEnded SelectionEnded;
    public void OnPointerUp(PointerEventData eventData)
    {
        if (hasSelected)
        {
            hasSelected = false;
            // Debug.Log("OnSelectionEnded");
            if (SelectionEnded != null)
            {
                SelectionEnded();
            }
        }
    }
}
