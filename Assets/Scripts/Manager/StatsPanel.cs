using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Toggleable (Tab) character sheet, shop-style: an opaque gold-on-navy panel with
// STATS / SYNERGIES tabs. Stats tab shows Amirani + his live stats; Synergies tab shows
// each evolution as [icon]+[icon] -> name with a gold pip progress bar. Pauses while open.
// UI is built in code; assign the sprites/font in the Inspector.
public class StatsPanel : MonoBehaviour
{
    [Tooltip("Old text panel, if any — hidden on Awake so only the new sheet shows.")]
    public GameObject panel;

    [Header("Assets")]
    public Sprite panelSprite;   // shop_panel (9-sliced)
    public Sprite pipFill;
    public Sprite pipTrack;
    public Sprite portrait;      // amirani_portrait
    public TMP_FontAsset font;

    [Header("Stat icons")]
    public Sprite icoHealth, icoSpeed, icoDash, icoDart, icoWisp, icoPulse;
    [Header("Skill icons")]
    public Sprite icoEmber, icoStar, icoSmite, icoChain;

    public KeyCode toggleKey = KeyCode.Tab;

    static readonly Color GOLD = new Color(.878f, .659f, .204f);
    static readonly Color GOLD_L = new Color(1f, .886f, .541f);
    static readonly Color NAVY = new Color(.102f, .118f, .212f);
    static readonly Color NAVY_L = new Color(.157f, .188f, .314f);
    static readonly Color TXT = new Color(1f, .925f, .745f);
    static readonly Color DIM = new Color(.604f, .627f, .745f);
    static readonly Color INK = new Color(.047f, .047f, .078f);

    Canvas canvas;
    RectTransform statsBody, synBody;
    Image tabStatsBg, tabSynBg;
    TextMeshProUGUI tabStatsTxt, tabSynTxt;
    bool open;

    void Awake()
    {
        if (panel != null) panel.SetActive(false);
        BuildUI();
        canvas.gameObject.SetActive(false);
    }

    void OnDestroy() { if (canvas != null) Destroy(canvas.gameObject); }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey)) Toggle();
        else if (open && Input.GetKeyDown(KeyCode.Escape)) Toggle();
    }

    public void Toggle()
    {
        if (!open)
        {
            if (MenuGuard.AnyOpen) return;
            open = true;
            canvas.gameObject.SetActive(true);
            BuildStats();
            BuildSynergies();
            ShowTab(true);
            Time.timeScale = 0f;
            MenuGuard.AnyOpen = true;
        }
        else
        {
            open = false;
            canvas.gameObject.SetActive(false);
            Time.timeScale = 1f;
            MenuGuard.AnyOpen = false;
        }
    }

    void ShowTab(bool stats)
    {
        statsBody.gameObject.SetActive(stats);
        synBody.gameObject.SetActive(!stats);
        tabStatsBg.color = stats ? GOLD : NAVY;
        tabSynBg.color = stats ? NAVY : GOLD;
        tabStatsTxt.color = stats ? INK : DIM;
        tabSynTxt.color = stats ? DIM : INK;
    }

    // ---------------- STATS TAB ----------------
    void BuildStats()
    {
        Clear(statsBody);

        var xp = FindObjectOfType<ExperienceManager>();
        var hp = FindObjectOfType<PlayerHealth>();
        var mv = FindObjectOfType<PlayerMovement>();
        var dash = FindObjectOfType<PlayerDash>();
        var darts = FindObjectOfType<StellarDarts>();
        var wisp = FindObjectOfType<OrbitingWisps>();
        var pulse = FindObjectOfType<LightPulse>();
        var fire = FindObjectOfType<FireTrail>();
        var star = FindObjectOfType<Starfall>();
        var smite = FindObjectOfType<HeavenSmite>();
        var chain = FindObjectOfType<ChainLash>();

        // portrait + name/level + HP bar
        var head = Row(statsBody, 190f);
        var port = Img("Portrait", head, portrait);
        SetRect(port.rectTransform, new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(0, .5f),
                new Vector2(100f, 0f), new Vector2(180f, 180f));
        port.preserveAspect = true;

        var nameT = Text("Name", head, "AMIRANI", 34f, GOLD_L);
        nameT.alignment = TextAlignmentOptions.TopLeft;
        SetRect(nameT.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(210f, -14f), new Vector2(-220f, 44f)); nameT.rectTransform.offsetMin = new Vector2(300, -58); nameT.rectTransform.offsetMax = new Vector2(-10, -14);
        var lvlT = Text("Lvl", head, "Level " + (xp != null ? xp.currentLevel : 1) + " · the dreamer", 20f, DIM);
        lvlT.alignment = TextAlignmentOptions.TopLeft;
        SetRect(lvlT.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(0, 0)); lvlT.rectTransform.offsetMin = new Vector2(300, -92); lvlT.rectTransform.offsetMax = new Vector2(-10, -58);

        if (hp != null)
        {
            var hpTrack = Img("HpTrack", head, null); hpTrack.color = NAVY_L;
            SetRect(hpTrack.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 1),
                    new Vector2(0, 0), new Vector2(0, 0)); hpTrack.rectTransform.offsetMin = new Vector2(300, -128); hpTrack.rectTransform.offsetMax = new Vector2(-14, -104);
            var hpFill = Img("HpFill", hpTrack.transform, null); hpFill.color = new Color(.878f, .337f, .353f);
            hpFill.type = Image.Type.Filled; hpFill.fillMethod = Image.FillMethod.Horizontal; hpFill.fillAmount = 1f;
            Stretch(hpFill.rectTransform);
            var hpLbl = Text("HpLbl", hpTrack.transform, "HP  " + hp.maxHealth, 15f, TXT);
            Stretch(hpLbl.rectTransform);
        }

        // stat rows
        if (mv != null)   StatRow(icoSpeed, "Move Speed", mv.maxSpeed.ToString("0.#"));
        if (dash != null) StatRow(icoDash, "Dash", dash.dashSpeed.ToString("0.#") + "  ·  CD " + dash.dashCooldown.ToString("0.#") + "s");
        if (darts != null)StatRow(icoDart, "Starlight Darts", "x" + darts.dartCount + "  ·  " + darts.fireRate.ToString("0.##") + "s");
        if (wisp != null) StatRow(icoWisp, "Spirit Wisps", wisp.count.ToString());
        if (pulse != null)StatRow(icoPulse, "Light Pulse", pulse.active ? ("r " + pulse.maxRadius.ToString("0.#")) : "locked");

        // active skills as chips
        var skills = new List<(Sprite, string)>();
        if (fire != null && fire.active) skills.Add((icoEmber, "Ember Path"));
        if (star != null && star.active) skills.Add((icoStar, "Starfall"));
        if (smite != null && smite.active) skills.Add((icoSmite, "Heaven's Smite"));
        if (chain != null && chain.active) skills.Add((icoChain, "Chain Lash"));
        if (skills.Count > 0)
        {
            Text("ActiveHdr", statsBody, "ACTIVE SKILLS", 14f, GOLD).alignment = TextAlignmentOptions.Left;
            var chipRow = Row(statsBody, 52f);
            float x = 10f;
            foreach (var (ic, nm) in skills)
            {
                var chip = Img("Chip", chipRow, null); chip.color = NAVY;
                var crt = chip.rectTransform; crt.anchorMin = new Vector2(0, .5f); crt.anchorMax = new Vector2(0, .5f);
                crt.pivot = new Vector2(0, .5f); crt.anchoredPosition = new Vector2(x, 0); crt.sizeDelta = new Vector2(44 + nm.Length * 11f, 40);
                var ci = Img("i", chip.transform, ic); SetRect(ci.rectTransform, new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(22, 0), new Vector2(26, 26));
                var ct = Text("t", chip.transform, nm, 16f, TXT); ct.alignment = TextAlignmentOptions.Left;
                SetRect(ct.rectTransform, new Vector2(0, 0), new Vector2(1, 1), new Vector2(0, .5f), Vector2.zero, Vector2.zero);
                ct.rectTransform.offsetMin = new Vector2(40, 0); ct.rectTransform.offsetMax = new Vector2(-8, 0);
                x += crt.sizeDelta.x + 8f;
            }
        }
    }

    void StatRow(Sprite ico, string label, string val)
    {
        var row = Row(statsBody, 46f);
        var bg = row.gameObject.AddComponent<Image>(); bg.color = NAVY; bg.raycastTarget = false;
        var accent = Img("Accent", row, null); accent.color = GOLD;
        SetRect(accent.rectTransform, new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, .5f), new Vector2(1.5f, 0), new Vector2(3, 0));
        if (ico != null) { var i = Img("Ico", row, ico); SetRect(i.rectTransform, new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(26, 0), new Vector2(28, 28)); }
        var lab = Text("Lbl", row, label, 19f, DIM); lab.alignment = TextAlignmentOptions.Left;
        SetRect(lab.rectTransform, new Vector2(0, 0), new Vector2(1, 1), new Vector2(0, .5f), Vector2.zero, Vector2.zero);
        lab.rectTransform.offsetMin = new Vector2(66, 0); lab.rectTransform.offsetMax = new Vector2(-180, 0);
        var v = Text("Val", row, val, 20f, TXT); v.alignment = TextAlignmentOptions.Right; v.fontStyle = FontStyles.Bold;
        SetRect(v.rectTransform, new Vector2(1, 0), new Vector2(1, 1), new Vector2(1, .5f), Vector2.zero, Vector2.zero);
        v.rectTransform.offsetMin = new Vector2(-220, 0); v.rectTransform.offsetMax = new Vector2(-14, 0);
    }

    // ---------------- SYNERGIES TAB ----------------
    void BuildSynergies()
    {
        Clear(synBody);
        var um = FindObjectOfType<UpgradeManager>();
        if (um == null) { Text("none", synBody, "No run in progress.", 20f, DIM); return; }

        SynRow(icoWisp, icoPulse, "Supernova", um.WispLevel, 3, um.RadiusLevel, 2, um.SupernovaUnlocked);
        SynRow(icoDash, icoSpeed, "Chain Lightning", um.DashLevel, 2, um.SpeedLevel, 2, um.LightningUnlocked);
        SynRow(icoEmber, icoSpeed, "Comet", um.FireTrailLevel, 3, um.SpeedLevel, 3, um.CometUnlocked);
        SynRow(icoStar, icoPulse, "Meteor Storm", um.StarfallLevel, 3, um.RadiusLevel, 3, um.MeteorStormUnlocked);
        SynRow(icoSmite, icoDash, "Thunderstorm", um.SmiteLevel, 3, um.DashLevel, 3, um.ThunderstormUnlocked);
        SynRow(icoChain, icoChain, "Titan's Wrath", um.ChainLashLevel, 5, 0, 0, um.ChainLashLevel >= 5);
    }

    void SynRow(Sprite a, Sprite b, string name, int l1, int r1, int l2, int r2, bool done)
    {
        var row = Row(synBody, 80f);
        var bg = row.gameObject.AddComponent<Image>(); bg.color = NAVY; bg.raycastTarget = false;
        var outline = row.gameObject.AddComponent<Outline>();
        outline.effectColor = done ? GOLD : new Color(.588f, .376f, .102f);
        outline.effectDistance = new Vector2(2, 2);

        if (a != null) { var ia = Img("A", row, a); SetRect(ia.rectTransform, new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(34, 0), new Vector2(34, 34)); }
        var plus = Text("+", row, "+", 22f, DIM); SetRect(plus.rectTransform, new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(.5f, .5f), new Vector2(60, 0), new Vector2(20, 30));
        if (b != null && r2 > 0) { var ib = Img("B", row, b); SetRect(ib.rectTransform, new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(86, 0), new Vector2(34, 34)); }

        var nm = Text("Nm", row, name, 22f, done ? GOLD_L : TXT); nm.alignment = TextAlignmentOptions.Left;
        SetRect(nm.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 1), Vector2.zero, Vector2.zero);
        nm.rectTransform.offsetMin = new Vector2(130, -40); nm.rectTransform.offsetMax = new Vector2(-150, -8);

        // pip bar: total requirement, filled = met
        int total = r1 + r2, got = Mathf.Min(l1, r1) + Mathf.Min(l2, r2);
        var track = Img("PipTrack", row, pipTrack);
        SetRect(track.rectTransform, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(132, 12), new Vector2(Mathf.Max(80, total * 26f), 16));
        var fill = Img("PipFill", track.transform, pipFill);
        fill.type = Image.Type.Filled; fill.fillMethod = Image.FillMethod.Horizontal; fill.fillOrigin = 0;
        fill.fillAmount = total > 0 ? (float)got / total : 0f;
        Stretch(fill.rectTransform);

        var state = Text("St", row, done ? "✦ UNLOCKED" : got + " / " + total, 16f, done ? GOLD_L : DIM);
        state.alignment = TextAlignmentOptions.Right;
        SetRect(state.rectTransform, new Vector2(1, .5f), new Vector2(1, .5f), new Vector2(1, .5f), new Vector2(-14, 0), new Vector2(150, 30));
    }

    // ---------------- UI scaffolding ----------------
    void BuildUI()
    {
        var canvasGO = new GameObject("StatsSheetCanvas");
        // ROOT canvas (NOT parented under the game Canvas). The game Canvas scales from an
        // 800x600 reference; nesting makes our own CanvasScaler inert, so our 1920-based
        // layout renders ~2.4x too big and spills off-screen. As a root canvas our scaler wins.
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1500;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        var dim = Img("Dim", canvasGO.transform, null); Stretch(dim.rectTransform);
        dim.color = new Color(0, 0, 0, .55f); dim.raycastTarget = true;

        var p = Img("Sheet", canvasGO.transform, panelSprite); p.type = Image.Type.Sliced;
        // anchor to a fraction of the screen so it always fits, regardless of resolution/DPI
        var prt = p.rectTransform;
        prt.anchorMin = new Vector2(.17f, .16f); prt.anchorMax = new Vector2(.83f, .84f);
        prt.pivot = new Vector2(.5f, .5f); prt.offsetMin = Vector2.zero; prt.offsetMax = Vector2.zero;

        var title = Text("Title", p.transform, "CHARACTER", 26f, GOLD_L);
        SetRect(title.rectTransform, new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(0, -34), new Vector2(600, 40));

        // tabs
        tabStatsBg = Img("TabStats", p.transform, null);
        SetRect(tabStatsBg.rectTransform, new Vector2(0, 1), new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(0, 0), new Vector2(0, 0));
        tabStatsBg.rectTransform.offsetMin = new Vector2(40, -118); tabStatsBg.rectTransform.offsetMax = new Vector2(-6, -74);
        tabStatsTxt = Text("t", tabStatsBg.transform, "STATS", 20f, INK); Stretch(tabStatsTxt.rectTransform);
        tabStatsBg.raycastTarget = true; // clickable
        var bS = tabStatsBg.gameObject.AddComponent<Button>(); bS.onClick.AddListener(() => ShowTab(true));

        tabSynBg = Img("TabSyn", p.transform, null);
        SetRect(tabSynBg.rectTransform, new Vector2(.5f, 1), new Vector2(1, 1), new Vector2(.5f, 1), new Vector2(0, 0), new Vector2(0, 0));
        tabSynBg.rectTransform.offsetMin = new Vector2(6, -118); tabSynBg.rectTransform.offsetMax = new Vector2(-40, -74);
        tabSynTxt = Text("t", tabSynBg.transform, "SYNERGIES", 20f, DIM); Stretch(tabSynTxt.rectTransform);
        tabSynBg.raycastTarget = true; // clickable
        var bY = tabSynBg.gameObject.AddComponent<Button>(); bY.onClick.AddListener(() => ShowTab(false));

        statsBody = Body(p.transform);
        synBody = Body(p.transform);

        var hint = Text("Hint", p.transform, "Tab / Esc to close", 16f, DIM);
        hint.alignment = TextAlignmentOptions.BottomRight;
        SetRect(hint.rectTransform, new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-24, 20), new Vector2(360, 30));
    }

    RectTransform Body(Transform parent)
    {
        var rt = NewRect("Body", parent);
        rt.anchorMin = new Vector2(0, 0); rt.anchorMax = new Vector2(1, 1);
        rt.offsetMin = new Vector2(40, 60); rt.offsetMax = new Vector2(-40, -130);
        var vlg = rt.gameObject.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 8; vlg.childControlWidth = true; vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true; vlg.childForceExpandHeight = false;
        vlg.padding = new RectOffset(6, 6, 6, 6);
        return rt;
    }

    RectTransform Row(Transform parent, float h)
    {
        var rt = NewRect("Row", parent);
        var le = rt.gameObject.AddComponent<LayoutElement>(); le.minHeight = h; le.preferredHeight = h;
        return rt;
    }

    void Clear(RectTransform body) { for (int i = body.childCount - 1; i >= 0; i--) Destroy(body.GetChild(i).gameObject); }

    RectTransform NewRect(string n, Transform parent) { var go = new GameObject(n); go.transform.SetParent(parent, false); return go.AddComponent<RectTransform>(); }
    Image Img(string n, Transform parent, Sprite s) { var rt = NewRect(n, parent); var i = rt.gameObject.AddComponent<Image>(); i.sprite = s; i.raycastTarget = false; if (s == null) i.color = Color.white; return i; }
    TextMeshProUGUI Text(string n, Transform parent, string s, float size, Color c)
    {
        var rt = NewRect(n, parent); var t = rt.gameObject.AddComponent<TextMeshProUGUI>();
        if (font != null) t.font = font; t.text = s; t.fontSize = size; t.color = c; t.raycastTarget = false;
        t.alignment = TextAlignmentOptions.Center; t.enableWordWrapping = false; return t;
    }
    static void Stretch(RectTransform rt) { rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero; }
    static void SetRect(RectTransform rt, Vector2 aMin, Vector2 aMax, Vector2 piv, Vector2 pos, Vector2 size)
    { rt.anchorMin = aMin; rt.anchorMax = aMax; rt.pivot = piv; rt.anchoredPosition = pos; rt.sizeDelta = size; }
}
