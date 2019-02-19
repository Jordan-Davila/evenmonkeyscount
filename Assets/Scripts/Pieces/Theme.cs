using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Theme : MonoBehaviour 
{
	public ThemeProperties themeProps;

    // GameObjects
    public GameObject selector;
    public new GameObject name;
    public GameObject price;
    public GameObject coins;

    // Components
    public Image themeImage;
    public Image selectorImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI coinsText;
    public Button selectButton;

    //Colors
    public Color selected;
    public Color unselected;

    private void Awake() 
    {
        // Objects
        selector = this.gameObject.transform.Find("selector").gameObject;
        name = selector.transform.Find("name").gameObject;
        price = this.gameObject.transform.Find("price").gameObject;
        coins = price.transform.Find("coins").gameObject;

        //Components
        themeImage = this.gameObject.GetComponent<Image>();
        selectorImage = selector.GetComponent<Image>();
        nameText = name.GetComponent<TextMeshProUGUI>();
        coinsText = coins.GetComponent<TextMeshProUGUI>();
        selectButton = this.gameObject.GetComponent<Button>();
    }

	public void UpdateUI()
	{
        // Set Name and Selection
        nameText.text = themeProps.name;
        
        if (themeProps.isSelected) selectorImage.color = selected;
        else selectorImage.color = unselected;

        // Set Price
        if (!themeProps.hasPurchased) coinsText.text = themeProps.coins.ToString();
        else price.SetActive(false);

        // Thumbnail
        if (themeProps.thumbnailImage == null) themeImage.color = themeProps.thumbnailColor;
        else themeImage.sprite = themeProps.thumbnailImage;
	}
}