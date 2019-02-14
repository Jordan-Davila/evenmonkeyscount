using UnityEngine;

[System.Serializable]
public class DotProperties
{
    [HideInInspector]
    public string name;
    public int number;
    public Color color;
    public enum DotType
    {
        normal,
        bomb,
        swiper
    };
    public DotType dotType;
}