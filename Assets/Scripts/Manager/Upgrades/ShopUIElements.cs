using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public UpgradeData1 data;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI levelText;
    public Button buyButton;

    void Start() => UpdateUI();

  public void UpdateUI()
{
    if (data == null) return; // ← დამატება

    nameText.text = data.upgradeName;
    int level = data.GetCurrentLevel();

    if (level >= data.maxLevel)
    {
        costText.text = "MAX";
        levelText.text = $"Lvl: {level}/{data.maxLevel}";
        buyButton.interactable = false;
    }
    else
    {
        int cost = data.GetCurrentCost();
        costText.text = cost.ToString();
        levelText.text = $"Lvl: {level}/{data.maxLevel}";

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