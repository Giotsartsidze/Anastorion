using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Survivor-style HUD: a full-width XP bar across the top with a LV badge, and a chunky
// HP bar top-left with "cur / max" text. Gold-on-navy (not a green copy). Built in code
// as its own ROOT canvas (the game Canvas uses an 800x600 reference, so nesting would
// mis-scale it). Reads the live values from ExperienceManager / PlayerHealth each frame
// and hides the old plain sliders.
public class GameHud : MonoBehaviour
{
    [Header("Assets")]
    public Sprite barTrack;   // hud_track (9-sliced)
    public Sprite barFill;    // hud_fill (9-sliced, tinted)
    public TMP_FontAsset font;

    [Header("Colors")]
    public Color xpColor = new Color(.878f, .659f, .204f);   // gold
    public Color hpColor = new Color(.816f, .271f, .290f);   // crimson
    public Color hpLow   = new Color(.90f, .30f, .20f);
    public Color bossColor = new Color(.72f, .16f, .28f);    // boss crimson
    static readonly Color INK = new Color(.047f, .047f, .078f);
    static readonly Color GOLD_L = new Color(1f, .886f, .541f);
    static readonly Color TXT = new Color(1f, .925f, .745f);

    public static GameHud Instance;

    ExperienceManager xp;
    PlayerHealth hp;

    GameObject hudRoot;
    Image xpFill, hpFill;
    TextMeshProUGUI lvlText, xpText, hpText;

    // Boss bar (driven by BossHealth)
    GameObject bossBarRoot;
    Image bossFill;
    TextMeshProUGUI bossText, bossNameText;
    int bossCur, bossMax;
    bool bossActive;

    void Awake() { Instance = this; }

    void Start()
    {
        xp = FindObjectOfType<ExperienceManager>();
        hp = FindObjectOfType<PlayerHealth>();
        // retire the old plain sliders — this HUD replaces them
        if (xp != null && xp.xpSlider != null) xp.xpSlider.gameObject.SetActive(false);
        if (hp != null && hp.healthSlider != null) hp.healthSlider.gameObject.SetActive(false);
        BuildUI();
    }

    void Update()
    {
        // hide the HUD while a menu/panel is open (it lives on its own canvas above the game)
        bool show = !MenuGuard.AnyOpen;
        if (hudRoot != null && hudRoot.activeSelf != show) hudRoot.SetActive(show);
        if (!show) return;

        if (xp != null)
        {
            float t = xp.targetXP > 0 ? Mathf.Clamp01((float)xp.currentXP / xp.targetXP) : 0f;
            if (xpFill != null) xpFill.fillAmount = t;
            if (lvlText != null) lvlText.text = "LV " + xp.currentLevel;
            if (xpText != null) xpText.text = xp.currentXP + " / " + xp.targetXP;
        }
        if (hp != null)
        {
            float t = hp.maxHealth > 0 ? Mathf.Clamp01((float)hp.currentHealth / hp.maxHealth) : 0f;
            if (hpFill != null) { hpFill.fillAmount = t; hpFill.color = t < 0.3f ? hpLow : hpColor; }
            if (hpText != null) hpText.text = Mathf.Max(0, hp.currentHealth) + " / " + hp.maxHealth;
        }
        if (bossActive && bossBarRoot != null)
        {
            float t = bossMax > 0 ? Mathf.Clamp01((float)bossCur / bossMax) : 0f;
            if (bossFill != null) bossFill.fillAmount = t;
            if (bossText != null) bossText.text = Mathf.Max(0, bossCur) + " / " + bossMax;
        }
    }

    // ---- Boss bar API (called by BossHealth) ----
    public void ShowBoss(string name, int cur, int max)
    {
        bossCur = cur; bossMax = max; bossActive = true;
        if (bossNameText != null) bossNameText.text = name;
        if (bossFill != null) bossFill.fillAmount = max > 0 ? Mathf.Clamp01((float)cur / max) : 0f;
        if (bossText != null) bossText.text = cur + " / " + max;
        if (bossBarRoot != null) bossBarRoot.SetActive(true);
    }
    public void UpdateBoss(int cur, int max) { bossCur = cur; bossMax = max; }
    public void HideBoss() { bossActive = false; if (bossBarRoot != null) bossBarRoot.SetActive(false); }

    void BuildUI()
    {
        var go = new GameObject("GameHudCanvas"); // ROOT, see class note
        hudRoot = go;
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1400;
        var sc = go.AddComponent<CanvasScaler>();
        sc.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        sc.referenceResolution = new Vector2(1920, 1080);
        sc.matchWidthOrHeight = 0.5f;

        // ---- XP bar: full width, top ----
        var xpBar = NewRect("XpBar", go.transform);
        xpBar.anchorMin = new Vector2(0, 1); xpBar.anchorMax = new Vector2(1, 1); xpBar.pivot = new Vector2(.5f, 1);
        xpBar.offsetMin = new Vector2(150, -40); xpBar.offsetMax = new Vector2(-40, -10);
        Img("XpTrack", xpBar, barTrack, Color.white, stretch: true);
        xpFill = Img("XpFill", xpBar, barFill, xpColor, stretch: true);
        xpFill.type = Image.Type.Filled; xpFill.fillMethod = Image.FillMethod.Horizontal; xpFill.fillOrigin = 0; xpFill.fillAmount = 0f;
        xpText = Text("XpNum", xpBar, "0 / 100", 16f, new Color(1, 1, 1, .85f)); Stretch(xpText.rectTransform);

        // LV badge, left of the bar
        var badge = Img("LvBadge", go.transform, barTrack, new Color(.878f, .659f, .204f), stretch: false);
        var brt = badge.rectTransform; brt.anchorMin = new Vector2(0, 1); brt.anchorMax = new Vector2(0, 1); brt.pivot = new Vector2(0, 1);
        brt.anchoredPosition = new Vector2(24, -8); brt.sizeDelta = new Vector2(120, 34);
        lvlText = Text("Lv", badge.transform, "LV 1", 20f, Color.white); lvlText.fontStyle = FontStyles.Bold; Stretch(lvlText.rectTransform);

        // ---- HP bar: top-left, below XP ----
        var hpBar = NewRect("HpBar", go.transform);
        hpBar.anchorMin = new Vector2(0, 1); hpBar.anchorMax = new Vector2(0, 1); hpBar.pivot = new Vector2(0, 1);
        hpBar.anchoredPosition = new Vector2(24, -52); hpBar.sizeDelta = new Vector2(340, 40);
        Img("HpTrack", hpBar, barTrack, Color.white, stretch: true);
        hpFill = Img("HpFill", hpBar, barFill, hpColor, stretch: true);
        hpFill.type = Image.Type.Filled; hpFill.fillMethod = Image.FillMethod.Horizontal; hpFill.fillOrigin = 0; hpFill.fillAmount = 1f;
        hpText = Text("HpNum", hpBar, "100 / 100", 20f, Color.white); hpText.fontStyle = FontStyles.Bold; Stretch(hpText.rectTransform);
        var hpLbl = Text("HpLbl", hpBar, "HP", 15f, new Color(1, 1, 1, .7f));
        hpLbl.alignment = TextAlignmentOptions.Left; var hlt = hpLbl.rectTransform;
        Stretch(hlt); hlt.offsetMin = new Vector2(16, 0); hlt.offsetMax = new Vector2(-16, 0);

        // ---- Boss bar: wide, bottom-center, hidden until a boss spawns ----
        var bossBar = NewRect("BossBar", go.transform);
        bossBar.anchorMin = bossBar.anchorMax = new Vector2(.5f, 0f); bossBar.pivot = new Vector2(.5f, 0f);
        bossBar.anchoredPosition = new Vector2(0, 40); bossBar.sizeDelta = new Vector2(1000, 32);
        bossBarRoot = bossBar.gameObject;
        Img("BossTrack", bossBar, barTrack, Color.white, stretch: true);
        bossFill = Img("BossFill", bossBar, barFill, bossColor, stretch: true);
        bossFill.type = Image.Type.Filled; bossFill.fillMethod = Image.FillMethod.Horizontal; bossFill.fillOrigin = 0; bossFill.fillAmount = 1f;
        bossText = Text("BossNum", bossBar, "", 17f, Color.white); bossText.fontStyle = FontStyles.Bold; Stretch(bossText.rectTransform);
        bossNameText = Text("BossName", bossBar, "", 20f, GOLD_L);
        var nrt = bossNameText.rectTransform;
        nrt.anchorMin = nrt.anchorMax = new Vector2(.5f, 1f); nrt.pivot = new Vector2(.5f, 0f);
        nrt.anchoredPosition = new Vector2(0, 5); nrt.sizeDelta = new Vector2(1000, 26);
        bossNameText.fontStyle = FontStyles.Bold; bossNameText.characterSpacing = 6f;
        bossBarRoot.SetActive(false);
    }

    RectTransform NewRect(string n, Transform parent) { var g = new GameObject(n); g.transform.SetParent(parent, false); return g.AddComponent<RectTransform>(); }
    Image Img(string n, Transform parent, Sprite s, Color c, bool stretch)
    {
        var rt = NewRect(n, parent); var i = rt.gameObject.AddComponent<Image>();
        i.sprite = s; i.type = Image.Type.Sliced; i.color = c; i.raycastTarget = false;
        if (stretch) Stretch(rt);
        return i;
    }
    TextMeshProUGUI Text(string n, Transform parent, string s, float size, Color c)
    {
        var rt = NewRect(n, parent); var t = rt.gameObject.AddComponent<TextMeshProUGUI>();
        if (font != null) t.font = font; t.text = s; t.fontSize = size; t.color = c;
        t.alignment = TextAlignmentOptions.Center; t.raycastTarget = false; t.enableWordWrapping = false;
        return t;
    }
    static void Stretch(RectTransform rt) { rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero; }
}
