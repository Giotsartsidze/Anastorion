using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public TextMeshProUGUI totalShardsText;
    public ShopItemUI[] shopItems;

    void OnEnable() => RefreshAllItems();

    public void RefreshAllItems()
    {
        if (totalShardsText != null && CurrencyManager.Instance != null)
            totalShardsText.text = CurrencyManager.Instance.totalShards.ToString();
        if (shopItems == null) return;
        foreach (var item in shopItems)
            if (item != null) item.UpdateUI();
    }
}