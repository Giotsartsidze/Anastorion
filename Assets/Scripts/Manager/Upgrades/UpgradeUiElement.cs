using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// One of the three level-up choice cards. Keeps its original serialized refs
/// (title / description / icon / button) and the pick wiring, but reskins itself
/// in code to the shared gold-on-navy look: gold card frame, icon medallion,
/// gold title, rarity pips, and a "NEW" badge on first unlock.
/// </summary>
public class UpgradeUIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Image iconImage;
    public Button button;

    [Header("Hover")]
    public float hoverScale = 1.06f;
    public float scaleSpeed = 10f;
    Vector3 originalScale = Vector3.one;
    Coroutine scaleCoroutine;

    static readonly Color Gold = new Color(0.93f, 0.78f, 0.42f);
    static readonly Color Body = new Color(0.80f, 0.84f, 0.93f);
    static readonly Color PipOff = new Color(0.20f, 0.24f, 0.34f);

    bool built;
    Image rarityBar;
    Image[] pips;
    GameObject newBadge;

    void BuildVisuals(UpgradeManager mgr)
    {
        if (built) return; built = true;

        var bg = GetComponent<Image>();
        if (bg == null) bg = gameObject.AddComponent<Image>();
        bg.sprite = UiSkin.CardSprite(); bg.type = Image.Type.Sliced; bg.color = Color.white;

        // rarity accent strip along the top
        rarityBar = NewImg("RarityBar", transform, null);
        var rb = rarityBar.rectTransform;
        rb.anchorMin = new Vector2(0.14f, 1f); rb.anchorMax = new Vector2(0.86f, 1f); rb.pivot = new Vector2(0.5f, 1f);
        rb.sizeDelta = new Vector2(0, 4); rb.anchoredPosition = new Vector2(0, -12);

        // medallion + reseat the icon inside it
        var medallion = NewImg("Medallion", transform, UiSkin.Medallion());
        var mrt = medallion.rectTransform;
        mrt.anchorMin = mrt.anchorMax = new Vector2(0.5f, 1f); mrt.pivot = new Vector2(0.5f, 1f);
        mrt.sizeDelta = new Vector2(74, 74); mrt.anchoredPosition = new Vector2(0, -22);
        if (iconImage != null)
        {
            iconImage.transform.SetParent(medallion.transform, false);
            var irt = iconImage.rectTransform;
            irt.anchorMin = irt.anchorMax = new Vector2(0.5f, 0.5f); irt.pivot = new Vector2(0.5f, 0.5f);
            irt.sizeDelta = new Vector2(42, 42); irt.anchoredPosition = Vector2.zero;
        }

        if (titleText != null)
        {
            if (mgr != null && mgr.uiFont != null) titleText.font = mgr.uiFont;
            titleText.fontSize = 18; titleText.color = Gold; titleText.alignment = TextAlignmentOptions.Center;
            titleText.enableWordWrapping = true; titleText.fontStyle = FontStyles.Normal; titleText.raycastTarget = false;
            var trt = titleText.rectTransform;
            trt.anchorMin = trt.anchorMax = new Vector2(0.5f, 1f); trt.pivot = new Vector2(0.5f, 1f);
            trt.sizeDelta = new Vector2(186, 40); trt.anchoredPosition = new Vector2(0, -98);
        }

        pips = new Image[3];
        for (int i = 0; i < 3; i++)
        {
            var p = NewImg("Pip" + i, transform, UiSkin.Disc());
            var prt = p.rectTransform;
            prt.anchorMin = prt.anchorMax = new Vector2(0.5f, 1f); prt.pivot = new Vector2(0.5f, 1f);
            prt.sizeDelta = new Vector2(10, 10); prt.anchoredPosition = new Vector2((i - 1) * 16, -140);
            pips[i] = p;
        }

        if (descriptionText != null)
        {
            if (mgr != null && mgr.pixelFont != null) descriptionText.font = mgr.pixelFont;
            descriptionText.color = Body; descriptionText.alignment = TextAlignmentOptions.Top;
            descriptionText.enableWordWrapping = true; descriptionText.raycastTarget = false;
            // Big box + auto-size so even the longest description stays inside the card.
            descriptionText.enableAutoSizing = true;
            descriptionText.fontSizeMin = 9.5f; descriptionText.fontSizeMax = 13.5f;
            var drt = descriptionText.rectTransform;
            drt.anchorMin = drt.anchorMax = new Vector2(0.5f, 1f); drt.pivot = new Vector2(0.5f, 1f);
            drt.sizeDelta = new Vector2(184, 132); drt.anchoredPosition = new Vector2(0, -150);
        }

        newBadge = NewPill("NewBadge", transform, "NEW", mgr);
        var nrt = (RectTransform)newBadge.transform;
        nrt.anchorMin = nrt.anchorMax = new Vector2(1f, 1f); nrt.pivot = new Vector2(1f, 1f);
        nrt.sizeDelta = new Vector2(52, 22); nrt.anchoredPosition = new Vector2(-8, -10);
        newBadge.SetActive(false);
    }

    public void Setup(UpgradeData data, UpgradeManager manager)
    {
        BuildVisuals(manager);

        bool firstPick = manager.IsFirstPick(data);
        titleText.text = (firstPick && !string.IsNullOrEmpty(data.unlockName)) ? data.unlockName : data.upgradeName;
        descriptionText.text = (firstPick && !string.IsNullOrEmpty(data.unlockDescription)) ? data.unlockDescription : data.description;
        if (iconImage != null && data.icon != null) iconImage.sprite = data.icon;

        Color rc = RarityColor(data.rarity);
        int tier = (int)data.rarity; // 0 common, 1 rare, 2 legendary
        if (rarityBar != null) rarityBar.color = rc;
        if (pips != null)
            for (int i = 0; i < pips.Length; i++) pips[i].color = (i <= tier) ? rc : PipOff;
        if (newBadge != null) newBadge.SetActive(firstPick);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => manager.ApplyUpgrade(data));

        originalScale = Vector3.one;
        transform.localScale = originalScale;
    }

    static Color RarityColor(UpgradeData.Rarity r)
    {
        switch (r)
        {
            case UpgradeData.Rarity.Rare: return new Color(0.45f, 0.68f, 1f);
            case UpgradeData.Rarity.Legendary: return new Color(1f, 0.82f, 0.35f);
            default: return new Color(0.86f, 0.66f, 0.30f);
        }
    }

    static Image NewImg(string name, Transform parent, Sprite sp)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var im = go.AddComponent<Image>(); im.sprite = sp; im.color = Color.white; im.raycastTarget = false;
        return im;
    }

    GameObject NewPill(string name, Transform parent, string label, UpgradeManager mgr)
    {
        var img = NewImg(name, parent, UiSkin.ButtonSprite());
        img.type = Image.Type.Sliced;
        var t = new GameObject("Label", typeof(RectTransform)).AddComponent<TextMeshProUGUI>();
        t.transform.SetParent(img.transform, false);
        var rt = t.rectTransform; rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = rt.offsetMax = Vector2.zero;
        if (mgr != null && mgr.uiFont != null) t.font = mgr.uiFont;
        t.text = label; t.fontSize = 13; t.color = new Color(0.16f, 0.10f, 0.02f);
        t.alignment = TextAlignmentOptions.Center; t.fontStyle = FontStyles.Bold; t.characterSpacing = 3f;
        t.enableWordWrapping = false; t.raycastTarget = false;
        return img.gameObject;
    }

    public void OnPointerEnter(PointerEventData e) { StopAndStartScale(originalScale * hoverScale); }
    public void OnPointerExit(PointerEventData e) { StopAndStartScale(originalScale); }

    void StopAndStartScale(Vector3 target)
    {
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(ScaleTo(target));
    }

    IEnumerator ScaleTo(Vector3 target)
    {
        while (Vector3.Distance(transform.localScale, target) > 0.001f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, target, Time.unscaledDeltaTime * scaleSpeed);
            yield return null;
        }
        transform.localScale = target;
    }

    public void AnimateIn(float delay)
    {
        transform.localScale = Vector3.zero;
        StartCoroutine(PopInRoutine(delay));
    }

    IEnumerator PopInRoutine(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        float duration = 0.3f, elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = elapsed / duration;
            float curve = Mathf.Sin(progress * Mathf.PI * 0.75f) * 1.15f;
            transform.localScale = Vector3.one * curve;
            yield return null;
        }
        transform.localScale = Vector3.one;
    }
}
