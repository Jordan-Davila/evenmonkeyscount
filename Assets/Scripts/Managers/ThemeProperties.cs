using UnityEngine;

[CreateAssetMenu(fileName = "New Theme", menuName = "Theme")]
public class ThemeProperties : ScriptableObject
{
    public string themeName;
    public int coins;
    public bool isSelected;
    public bool hasPurchased;
    public Sprite backgroundImage;
    public Sprite thumbnailImage;
    public Color backgroundColor;
    public Color thumbnailColor;
    public DotProperties[] Dots = new DotProperties[20];

    private void OnEnable() 
    {
        InitDots();
    }

    public void InitDots()
    {
        for (int i = 0; i < Dots.Length; i++)
        {
            Dots[i].name = "Dot #" + (i+1);
            Dots[i].number = i + 1;
        }
    }
}