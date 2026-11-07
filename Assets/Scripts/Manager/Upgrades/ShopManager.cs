using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public TextMeshProUGUI totalShardsText;
    public ShopItemUI[] shopItems;

    bool rerollAdded;
    UpgradeData1 rerollSO;

    void OnEnable()
    {
        EnsureRerollItem();   // inject the shop-bought Reroll upgrade (once)
        RefreshAllItems();
    }

    // The "Reroll" upgrade isn't a hand-placed scene asset — it's created in code and
    // its row is cloned from an existing Power-tab item, so the whole feature needs no
    // scene edits. Persistence is automatic: UpgradeData1 keys PlayerPrefs by name
    // ("Upgrade_Reroll"), which UpgradeManager reads to grant rerolls each level-up.
    void EnsureRerollItem()
    {
        if (rerollAdded || shopItems == null || shopItems.Length == 0) return;

        // Prefer a template from the Power tab so the new row lands there.
        ShopItemUI template = null;
        foreach (var it in shopItems)
        {
            if (it == null) continue;
            if (it.transform.parent != null && it.transform.parent.name == "PowerGroup") { template = it; break; }
        }
        if (template == null) template = System.Array.Find(shopItems, s => s != null);
        if (template == null) return;

        rerollAdded = true;

        rerollSO = ScriptableObject.CreateInstance<UpgradeData1>();
        rerollSO.upgradeName = "Reroll";
        rerollSO.description = "Reroll a level-up. +1 reroll per tier.";
        rerollSO.baseCost = 200;
        rerollSO.costMultiplier = 2f;   // 200 / 400 / 800
        rerollSO.maxLevel = 3;

        var clone = Instantiate(template.gameObject, template.transform.parent);
        clone.name = "UpgradeButton_Reroll";
        clone.transform.SetAsLastSibling();

        var ui = clone.GetComponent<ShopItemUI>();
        ui.data = rerollSO;

        var list = new List<ShopItemUI>(shopItems) { ui };
        shopItems = list.ToArray();

        // A 6th row makes the force-expand layout squeeze every row (~75px) so they
        // overlap. We can't just grow the box — the shop panel is center-stacked and
        // would crowd the tabs/CLOSE. Instead, reclaim space from the header's big 30px
        // gaps (down to 14px) and feed it into the row box, keeping the overall stack
        // the same height while giving each row ~85px.
        var panelVlg = GetComponent<UnityEngine.UI.VerticalLayoutGroup>();
        if (panelVlg != null && panelVlg.spacing > 14f) panelVlg.spacing = 14f;
        GrowGroupToFit(template.transform.parent as RectTransform, 85f);
    }

    // Resize the group's parent "Box" so a VerticalLayoutGroup that force-expands its
    // children keeps them at rowHeight instead of shrinking to fit a fixed box.
    static void GrowGroupToFit(RectTransform group, float rowHeight)
    {
        if (group == null) return;
        var box = group.parent as RectTransform;
        if (box == null) return;
        int rows = group.childCount;
        var vlg = group.GetComponent<UnityEngine.UI.VerticalLayoutGroup>();
        float spacing = vlg != null ? vlg.spacing : 10f;
        float padding = vlg != null ? vlg.padding.top + vlg.padding.bottom : 0f;
        float needed = rows * rowHeight + (rows - 1) * spacing + padding;
        if (box.sizeDelta.y < needed)
        {
            var s = box.sizeDelta; s.y = needed; box.sizeDelta = s;
        }
    }

    public void RefreshAllItems()
    {
        if (totalShardsText != null && CurrencyManager.Instance != null)
            totalShardsText.text = CurrencyManager.Instance.totalShards.ToString();
        if (shopItems == null) return;
        foreach (var item in shopItems)
            if (item != null) item.UpdateUI();
    }
}
