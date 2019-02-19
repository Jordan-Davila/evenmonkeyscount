using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ThemeManager : MonoBehaviour 
{
    private GameManager gm;
    private UIManager ui;
    private List<Theme> AllThemes = new List<Theme>();

    private void Awake() 
    {
        gm = UnityEngine.Object.FindObjectOfType<GameManager>();
        ui = UnityEngine.Object.FindObjectOfType<UIManager>();
    }
    private void Start() 
    {
        DisplayAllThemes();
    }
    public void DisplayAllThemes()
    {
        // SelectTheme First then Display
        SelectTheme(GetSelectedTheme());

        // Then Instantiate
        foreach(ThemeProperties themeProps in gm.themes)
        {
            // Instantiate
            GameObject themeItem = Instantiate(ui.menuUI.themePrefab, Vector3.zero, Quaternion.identity) as GameObject;
            themeItem.name = themeProps.name;
            themeItem.transform.SetParent(ui.menuUI.themesContainer.transform, false);

            Theme theme = themeItem.GetComponent<Theme>();
            AllThemes.Add(theme);
            theme.themeProps = themeProps;
            theme.selectButton.onClick.AddListener(delegate { HandleThemeSelectorClick(themeProps); });
            theme.UpdateUI();
        }
    }

    public ThemeProperties GetSelectedTheme()
    {
        return gm.themes[gm.selectedTheme];
    }

    public void HandleThemeSelectorClick(ThemeProperties theme)
    {
        Debug.Log(theme.name);

        if (theme.isSelected) return;
        else 
        {
            if (theme.hasPurchased)
                SelectTheme(theme);
            else
                ui.DisplayThemesConfirmation(theme);
        }

        UpdateUI();
    }

    public void SelectTheme(ThemeProperties selectedTheme)
    {
        for (int i = 0; i < gm.themes.Length; i++)
        {
            if (gm.themes[i] == selectedTheme)
            {
                gm.themes[i].isSelected = true;
                gm.selectedTheme = i;
            }
            else
                gm.themes[i].isSelected = false;
        }

        ui.UpdateThemeBackground();
    }

    public void UpdateUI()
    {
        foreach (Theme theme in AllThemes)
        {
            theme.UpdateUI();
        }
    }
}