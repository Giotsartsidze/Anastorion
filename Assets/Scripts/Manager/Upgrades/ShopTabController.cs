using UnityEngine;
using UnityEngine.UI;

// Switches the shop between its two upgrade categories (Survival / Power) by
// toggling the two group containers and recoloring the tab buttons.
// Wire each tab button's OnClick to ShowSurvival() / ShowPower().
public class ShopTabController : MonoBehaviour
{
    public GameObject survivalGroup;
    public GameObject powerGroup;
    public Button survivalTab;
    public Button powerTab;

    public Color activeColor = new Color(0.878f, 0.659f, 0.204f);  // gold  #E0A834
    public Color inactiveColor = new Color(0.102f, 0.118f, 0.212f); // navy #1A1E36

    void OnEnable() => ShowSurvival();

    public void ShowSurvival() => Select(true);
    public void ShowPower() => Select(false);

    void Select(bool survival)
    {
        if (survivalGroup != null) survivalGroup.SetActive(survival);
        if (powerGroup != null) powerGroup.SetActive(!survival);
        SetTab(survivalTab, survival);
        SetTab(powerTab, !survival);
    }

    void SetTab(Button tab, bool active)
    {
        if (tab == null) return;
        var img = tab.GetComponent<Image>();
        if (img != null) img.color = active ? activeColor : inactiveColor;
    }
}
