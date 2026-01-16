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
            costText.text = data.GetCurrentCost().ToString();
            levelText.text = $"Lvl: {level}/{data.maxLevel}";
            buyButton.interactable = CurrencyManager.Instance.totalShards >= data.GetCurrentCost();
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