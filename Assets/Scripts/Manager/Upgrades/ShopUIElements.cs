using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public UpgradeData1 data;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI levelText;
    public Image levelPips;        // gold pip bar (Image type = Filled, Horizontal); fillAmount = level/maxLevel
    public Button buyButton;

    void Start() => UpdateUI();

  public void UpdateUI()
{
    if (data == null) return; // ← დამატება

    nameText.text = data.upgradeName;
    if (descriptionText != null) descriptionText.text = data.description;
    int level = data.GetCurrentLevel();

    // level progress as gold pips
    if (levelPips != null)
        levelPips.fillAmount = data.maxLevel > 0 ? (float)level / data.maxLevel : 0f;

    if (level >= data.maxLevel)
    {
        costText.text = "MAX";
        if (levelText != null) levelText.text = $"Lvl: {level}/{data.maxLevel}";
        buyButton.interactable = false;
    }
    else
    {
        int cost = data.GetCurrentCost();
        costText.text = cost.ToString();
        if (levelText != null) levelText.text = $"Lvl: {level}/{data.maxLevel}";

        // ✅ null check დამატებულია
        if (CurrencyManager.Instance != null)
            buyButton.interactable = CurrencyManager.Instance.totalShards >= cost;
    }
}

    public void OnBuyClicked()
    {
        if (CurrencyManager.Instance.SpendShards(data.GetCurrentCost()))
        {
            data.Upgrade();
            UpdateUI();
            // აქ დავამატოთ ShopManager-ის განახლება, რომ სხვა ღილაკებიც შემოწმდეს
            FindObjectOfType<ShopManager>().RefreshAllItems();
        }
    }
}