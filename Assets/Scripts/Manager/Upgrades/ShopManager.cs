using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public TextMeshProUGUI totalShardsText;
    public ShopItemUI[] shopItems;

    void OnEnable() => RefreshAllItems();

    public void RefreshAllItems()
    {
        totalShardsText.text = "Shards: " + CurrencyManager.Instance.totalShards;
        foreach (var item in shopItems) item.UpdateUI();
    }
}