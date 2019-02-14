using UnityEngine;

[CreateAssetMenu(fileName = "New Theme", menuName = "Theme")]
public class ThemeProperties : ScriptableObject
{
    public string themeName;
    public string description;
    public Sprite backgroundImage;
    public Color backgroundColor;
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