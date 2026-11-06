using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Settings panel. Lives on the SettingsPanel GameObject that PauseMenu / MainMenu
/// toggle active. On Awake it builds a fully code-driven options panel on its OWN
/// root canvas (so it scales + renders identically on the menu and in gameplay —
/// no prefab-instance drift). All sprites (glossy gold slider, handle, button,
/// checkbox, dropdown, chevron) are generated procedurally in code, so there is no
/// asset-import step to go wrong. Controls are wired to SettingsManager.
/// </summary>
public class SettingsUI : MonoBehaviour
{
    public TMP_FontAsset uiFont; // Cinzel

    // ---- palette ----
    static readonly Color TitleGold  = new Color(0.93f, 0.78f, 0.42f);
    static readonly Color Gold        = new Color(0.86f, 0.68f, 0.33f);
    static readonly Color ValueGold   = new Color(0.95f, 0.78f, 0.36f);
    static readonly Color HeaderGold  = new Color(0.62f, 0.45f, 0.20f);
    static readonly Color Slate       = new Color(0.52f, 0.57f, 0.68f);
    static readonly Color RuleGold    = new Color(0.70f, 0.55f, 0.25f, 0.55f);

    static readonly Vector2 C = new Vector2(0.5f, 0.5f);
    static readonly Vector2 L = new Vector2(0f, 0.5f);
    static readonly Vector2 R = new Vector2(1f, 0.5f);

    // ---- sprites (built once) ----
    Sprite panelSprite, trackSprite, fillSprite, handleSprite, buttonSprite, boxSprite, checkSprite, dropSprite, chevronSprite;

    GameObject canvasGO;
    Transform P; // panel transform
    bool built;

    Slider masterSlider, musicSlider, sfxSlider;
    Toggle fullscreenToggle;
    TMP_Dropdown resolutionDropdown;
    TMP_Text masterValue, musicValue, sfxValue;

    void Awake()
    {
        if (built) return;
        BuildSprites();
        BuildUI();
        built = true;
        if (canvasGO) canvasGO.SetActive(gameObject.activeInHierarchy);
    }

    void OnEnable()  { if (canvasGO) { canvasGO.SetActive(true); Refresh(); } }
    void OnDisable() { if (canvasGO) canvasGO.SetActive(false); }
    void OnDestroy() { if (canvasGO) Destroy(canvasGO); }

    static void SetPct(TMP_Text t, float v) { if (t) t.text = Mathf.RoundToInt(v * 100f) + "%"; }

    public void Close()
    {
        var pause = FindAnyObjectByType<PauseMenu>(FindObjectsInactive.Include);
        if (pause != null) pause.CloseSettings();
        else gameObject.SetActive(false);
    }

    // ====================================================================== UI
    void BuildUI()
    {
        // hide the old prefab visuals underneath
        var selfImg = GetComponent<Image>(); if (selfImg) selfImg.enabled = false;
        for (int i = transform.childCount - 1; i >= 0; i--) transform.GetChild(i).gameObject.SetActive(false);

        canvasGO = new GameObject("SettingsRootCanvas");
        var cv = canvasGO.AddComponent<Canvas>();
        cv.renderMode = RenderMode.ScreenSpaceOverlay;
        cv.sortingOrder = 200;
        var sc = canvasGO.AddComponent<CanvasScaler>();
        sc.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        sc.referenceResolution = new Vector2(1920, 1080);
        sc.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        var scrim = NewImg("Scrim", canvasGO.transform, null);
        Stretch(scrim.rectTransform);
        scrim.color = new Color(0.02f, 0.02f, 0.05f, 0.80f);
        scrim.raycastTarget = true;

        var panel = NewImg("Panel", canvasGO.transform, panelSprite);
        panel.type = Image.Type.Sliced;
        var prt = panel.rectTransform;
        prt.anchorMin = prt.anchorMax = C; prt.pivot = C;
        prt.sizeDelta = new Vector2(880, 820); prt.anchoredPosition = Vector2.zero;
        P = panel.transform;

        MakeText("Title", P, "SETTINGS", new Vector2(0, 322), new Vector2(820, 72), 58, TitleGold, TextAlignmentOptions.Center, C, true, 9);
        MakeText("Sub", P, "· the dreamer's rest ·", new Vector2(0, 270), new Vector2(820, 30), 22, Slate, TextAlignmentOptions.Center, C, false, 6);

        Header("AUDIO", 206);
        Row("Master", 118, out masterSlider, out masterValue);
        Row("Music",   42, out musicSlider,  out musicValue);
        Row("SFX",    -34, out sfxSlider,     out sfxValue);

        Header("DISPLAY", -118);
        // display row
        MakeText("FsLabel", P, "Fullscreen", new Vector2(-360, -200), new Vector2(260, 44), 30, Gold, TextAlignmentOptions.Left, L);
        fullscreenToggle = MakeToggle(P, new Vector2(-150, -200), new Vector2(46, 46));
        resolutionDropdown = MakeDropdown(P, new Vector2(162, -200), new Vector2(356, 58));

        MakeButton(P, new Vector2(0, -322), new Vector2(360, 80), "BACK");

        // listeners
        masterSlider.onValueChanged.AddListener(v => { if (SettingsManager.Instance) SettingsManager.Instance.SetMaster(v); SetPct(masterValue, v); });
        musicSlider.onValueChanged.AddListener(v => { if (SettingsManager.Instance) SettingsManager.Instance.SetMusic(v); SetPct(musicValue, v); });
        sfxSlider.onValueChanged.AddListener(v => { if (SettingsManager.Instance) SettingsManager.Instance.SetSFX(v); SetPct(sfxValue, v); });
        fullscreenToggle.onValueChanged.AddListener(f => { if (SettingsManager.Instance) SettingsManager.Instance.SetFullscreen(f); });
        resolutionDropdown.onValueChanged.AddListener(i => { if (SettingsManager.Instance) SettingsManager.Instance.SetResolution(i); });
    }

    void Refresh()
    {
        var s = SettingsManager.Instance;
        if (s == null) return;
        masterSlider.SetValueWithoutNotify(s.Master);
        musicSlider.SetValueWithoutNotify(s.Music);
        sfxSlider.SetValueWithoutNotify(s.SFX);
        SetPct(masterValue, s.Master); SetPct(musicValue, s.Music); SetPct(sfxValue, s.SFX);
        fullscreenToggle.SetIsOnWithoutNotify(s.Fullscreen);

        resolutionDropdown.ClearOptions();
        var opts = new List<string>();
        foreach (var r in s.Resolutions) opts.Add(r.width + " × " + r.height);
        resolutionDropdown.AddOptions(opts);
        resolutionDropdown.SetValueWithoutNotify(s.ResolutionIndex);
        resolutionDropdown.RefreshShownValue();
    }

    void Header(string text, float y)
    {
        MakeText("H_" + text, P, text, new Vector2(-360, y), new Vector2(420, 28), 20, HeaderGold, TextAlignmentOptions.Left, L, false, 10);
        var rule = NewImg("Rule_" + text, P, null);
        rule.color = RuleGold; rule.raycastTarget = false;
        var rt = rule.rectTransform; rt.anchorMin = rt.anchorMax = C; rt.pivot = C;
        rt.sizeDelta = new Vector2(720, 2); rt.anchoredPosition = new Vector2(0, y - 22);
    }

    void Row(string label, float y, out Slider slider, out TMP_Text val)
    {
        MakeText(label, P, label, new Vector2(-360, y), new Vector2(250, 46), 34, Gold, TextAlignmentOptions.Left, L);
        slider = MakeSlider(P, new Vector2(46, y), new Vector2(300, 18));
        val = MakeText(label + "V", P, "0%", new Vector2(372, y), new Vector2(120, 46), 32, ValueGold, TextAlignmentOptions.Right, R, true);
    }

    // ---- primitive builders ----
    static Image NewImg(string name, Transform parent, Sprite sp)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var im = go.AddComponent<Image>();
        im.sprite = sp; im.color = Color.white;
        return im;
    }

    static void Stretch(RectTransform rt)
    { rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero; }

    TextMeshProUGUI MakeText(string name, Transform parent, string text, Vector2 pos, Vector2 size,
                             float fs, Color c, TextAlignmentOptions align, Vector2 pivot, bool bold = false, float spacing = 0)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rt = (RectTransform)go.transform;
        rt.anchorMin = rt.anchorMax = C; rt.pivot = pivot;
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        var t = go.AddComponent<TextMeshProUGUI>();
        if (uiFont != null) t.font = uiFont;
        t.text = text; t.fontSize = fs; t.color = c; t.alignment = align;
        t.characterSpacing = spacing;
        if (bold) t.fontStyle = FontStyles.Bold;
        t.enableWordWrapping = false; t.raycastTarget = false; t.overflowMode = TextOverflowModes.Overflow;
        return t;
    }

    Slider MakeSlider(Transform parent, Vector2 pos, Vector2 size)
    {
        float handle = size.y * 2.1f;
        var go = new GameObject("Slider", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rt = (RectTransform)go.transform;
        rt.anchorMin = rt.anchorMax = C; rt.pivot = C; rt.anchoredPosition = pos; rt.sizeDelta = size;

        var bg = NewImg("Background", go.transform, trackSprite); bg.type = Image.Type.Sliced;
        var bgrt = bg.rectTransform; bgrt.anchorMin = new Vector2(0, 0.5f); bgrt.anchorMax = new Vector2(1, 0.5f);
        bgrt.pivot = C; bgrt.sizeDelta = new Vector2(0, size.y); bgrt.anchoredPosition = Vector2.zero;

        var fa = new GameObject("Fill Area", typeof(RectTransform)); fa.transform.SetParent(go.transform, false);
        var fart = (RectTransform)fa.transform; fart.anchorMin = new Vector2(0, 0.5f); fart.anchorMax = new Vector2(1, 0.5f);
        fart.pivot = C; fart.sizeDelta = new Vector2(-handle, size.y); fart.anchoredPosition = Vector2.zero;
        var fill = NewImg("Fill", fa.transform, fillSprite); fill.type = Image.Type.Sliced;
        var flrt = fill.rectTransform; flrt.anchorMin = new Vector2(0, 0); flrt.anchorMax = new Vector2(1, 1);
        flrt.offsetMin = Vector2.zero; flrt.offsetMax = Vector2.zero;

        var ha = new GameObject("Handle Slide Area", typeof(RectTransform)); ha.transform.SetParent(go.transform, false);
        var hart = (RectTransform)ha.transform; hart.anchorMin = Vector2.zero; hart.anchorMax = Vector2.one;
        hart.sizeDelta = new Vector2(-handle, 0); hart.anchoredPosition = Vector2.zero;
        var hd = NewImg("Handle", ha.transform, handleSprite);
        var hdrt = hd.rectTransform; hdrt.anchorMin = new Vector2(0, 0); hdrt.anchorMax = new Vector2(0, 1);
        hdrt.pivot = C; hdrt.sizeDelta = new Vector2(handle, 0); hdrt.anchoredPosition = Vector2.zero;

        var s = go.AddComponent<Slider>();
        s.fillRect = flrt; s.handleRect = hdrt; s.targetGraphic = hd;
        s.direction = Slider.Direction.LeftToRight; s.minValue = 0; s.maxValue = 1;
        return s;
    }

    Toggle MakeToggle(Transform parent, Vector2 pos, Vector2 size)
    {
        var go = new GameObject("FullscreenToggle", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rt = (RectTransform)go.transform;
        rt.anchorMin = rt.anchorMax = C; rt.pivot = C; rt.anchoredPosition = pos; rt.sizeDelta = size;

        var box = NewImg("Box", go.transform, boxSprite); box.type = Image.Type.Sliced; Stretch(box.rectTransform);
        var chk = NewImg("Check", go.transform, checkSprite); chk.color = ValueGold;
        var crt = chk.rectTransform; crt.anchorMin = Vector2.zero; crt.anchorMax = Vector2.one;
        crt.offsetMin = new Vector2(6, 6); crt.offsetMax = new Vector2(-6, -6);

        var t = go.AddComponent<Toggle>();
        t.targetGraphic = box; t.graphic = chk; t.isOn = true;
        return t;
    }

    Button MakeButton(Transform parent, Vector2 pos, Vector2 size, string label)
    {
        var img = NewImg("BackButton", parent, buttonSprite); img.type = Image.Type.Sliced;
        var rt = img.rectTransform; rt.anchorMin = rt.anchorMax = C; rt.pivot = C;
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        var b = img.gameObject.AddComponent<Button>(); b.targetGraphic = img;
        var colors = b.colors; colors.highlightedColor = new Color(1.08f, 1.08f, 1.08f, 1f); colors.pressedColor = new Color(0.85f, 0.85f, 0.85f, 1f); b.colors = colors;
        MakeText("Label", img.transform, label, Vector2.zero, size, 36, new Color(0.16f, 0.10f, 0.02f), TextAlignmentOptions.Center, C, true, 4);
        b.onClick.AddListener(Close);
        return b;
    }

    TMP_Dropdown MakeDropdown(Transform parent, Vector2 pos, Vector2 size)
    {
        var img = NewImg("ResolutionDropdown", parent, dropSprite); img.type = Image.Type.Sliced;
        var rt = img.rectTransform; rt.anchorMin = rt.anchorMax = C; rt.pivot = C;
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        var dd = img.gameObject.AddComponent<TMP_Dropdown>();
        dd.targetGraphic = img;

        var label = MakeText("Label", img.transform, "", Vector2.zero, size, 26, Gold, TextAlignmentOptions.Left, C);
        var lrt = label.rectTransform; lrt.anchorMin = Vector2.zero; lrt.anchorMax = Vector2.one;
        lrt.offsetMin = new Vector2(22, 0); lrt.offsetMax = new Vector2(-48, 0);

        var arrow = NewImg("Arrow", img.transform, chevronSprite); arrow.color = Gold;
        var art = arrow.rectTransform; art.anchorMin = art.anchorMax = new Vector2(1, 0.5f); art.pivot = new Vector2(1, 0.5f);
        art.sizeDelta = new Vector2(26, 26); art.anchoredPosition = new Vector2(-16, 0);

        // template
        var tpl = new GameObject("Template", typeof(RectTransform), typeof(Image), typeof(ScrollRect));
        tpl.transform.SetParent(img.transform, false);
        var tplrt = (RectTransform)tpl.transform;
        tplrt.anchorMin = new Vector2(0, 0); tplrt.anchorMax = new Vector2(1, 0); tplrt.pivot = new Vector2(0.5f, 1);
        tplrt.anchoredPosition = new Vector2(0, 2); tplrt.sizeDelta = new Vector2(0, 230);
        var tplImg = tpl.GetComponent<Image>(); tplImg.sprite = dropSprite; tplImg.type = Image.Type.Sliced;
        tplImg.color = new Color(0.07f, 0.09f, 0.17f, 1f);

        var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
        viewport.transform.SetParent(tpl.transform, false);
        var vprt = (RectTransform)viewport.transform;
        vprt.anchorMin = Vector2.zero; vprt.anchorMax = Vector2.one; vprt.pivot = new Vector2(0.5f, 1f);
        vprt.offsetMin = new Vector2(4, 4); vprt.offsetMax = new Vector2(-4, -4);
        viewport.GetComponent<Mask>().showMaskGraphic = false;
        viewport.GetComponent<Image>().color = Color.white;

        var content = new GameObject("Content", typeof(RectTransform)); content.transform.SetParent(viewport.transform, false);
        var crt = (RectTransform)content.transform; crt.anchorMin = new Vector2(0, 1); crt.anchorMax = new Vector2(1, 1);
        crt.pivot = new Vector2(0.5f, 1); crt.sizeDelta = new Vector2(0, 46); crt.anchoredPosition = Vector2.zero;

        var item = new GameObject("Item", typeof(RectTransform), typeof(Toggle)); item.transform.SetParent(content.transform, false);
        var irt = (RectTransform)item.transform; irt.anchorMin = new Vector2(0, 0.5f); irt.anchorMax = new Vector2(1, 0.5f);
        irt.pivot = C; irt.sizeDelta = new Vector2(0, 46); irt.anchoredPosition = Vector2.zero;
        var itemBg = NewImg("Item Background", item.transform, null); Stretch(itemBg.rectTransform);
        itemBg.color = new Color(0.83f, 0.66f, 0.30f, 0.22f);
        var itemChk = NewImg("Item Checkmark", item.transform, null);
        var icrt = itemChk.rectTransform; icrt.anchorMin = icrt.anchorMax = L; icrt.sizeDelta = Vector2.zero; itemChk.color = new Color(0, 0, 0, 0);
        var itemLbl = MakeText("Item Label", item.transform, "Option", Vector2.zero, size, 24, Gold, TextAlignmentOptions.Left, C);
        var ilrt = itemLbl.rectTransform; ilrt.anchorMin = Vector2.zero; ilrt.anchorMax = Vector2.one; ilrt.offsetMin = new Vector2(22, 0); ilrt.offsetMax = new Vector2(-10, 0);
        var tgl = item.GetComponent<Toggle>(); tgl.targetGraphic = itemBg; tgl.graphic = itemChk; tgl.isOn = true;

        var sr = tpl.GetComponent<ScrollRect>(); sr.content = crt; sr.viewport = vprt;
        sr.horizontal = false; sr.vertical = true; sr.movementType = ScrollRect.MovementType.Clamped; sr.scrollSensitivity = 24;
        tpl.SetActive(false);

        dd.template = tplrt; dd.captionText = label; dd.itemText = itemLbl;
        return dd;
    }

    // ================================================================== SPRITES
    void BuildSprites()
    {
        Color navyTop = new Color(0.075f, 0.095f, 0.185f);
        Color navyBot = new Color(0.040f, 0.052f, 0.110f);
        Color frameGold = new Color(0.83f, 0.66f, 0.32f);
        panelSprite = RoundRect(176, 176, 30, navyTop, navyBot, frameGold, 3.2f, false, V(30));

        Color trackCol = new Color(0.135f, 0.155f, 0.255f);
        trackSprite = RoundRect(72, 36, 18, trackCol, trackCol * 0.9f, trackCol, 0, false, new Vector4(18, 0, 18, 0));

        Color fillTop = new Color(1.0f, 0.85f, 0.45f);
        Color fillBot = new Color(0.82f, 0.60f, 0.16f);
        Color fillEdge = new Color(0.58f, 0.40f, 0.10f);
        fillSprite = RoundRect(72, 36, 18, fillTop, fillBot, fillEdge, 1.5f, true, new Vector4(18, 0, 18, 0));

        handleSprite = Ball(52);

        Color btnTop = new Color(0.99f, 0.83f, 0.42f);
        Color btnBot = new Color(0.82f, 0.58f, 0.16f);
        Color btnEdge = new Color(0.55f, 0.38f, 0.10f);
        buttonSprite = RoundRect(120, 72, 16, btnTop, btnBot, btnEdge, 3f, true, V(16));

        boxSprite = RoundRect(56, 56, 10, new Color(0.09f, 0.11f, 0.20f), new Color(0.06f, 0.08f, 0.15f), frameGold, 3f, false, V(10));
        checkSprite = Check(56);
        dropSprite = RoundRect(96, 72, 14, new Color(0.085f, 0.105f, 0.195f), new Color(0.055f, 0.07f, 0.14f), frameGold, 2.2f, false, V(14));
        chevronSprite = Chevron(40);
    }

    static Vector4 V(float f) { return new Vector4(f, f, f, f); }

    static float SdRound(float px, float py, float w, float h, float r)
    {
        float qx = Mathf.Abs(px - w * 0.5f) - (w * 0.5f - r);
        float qy = Mathf.Abs(py - h * 0.5f) - (h * 0.5f - r);
        float ox = Mathf.Max(qx, 0), oy = Mathf.Max(qy, 0);
        return Mathf.Sqrt(ox * ox + oy * oy) + Mathf.Min(Mathf.Max(qx, qy), 0) - r;
    }

    static Sprite RoundRect(int w, int h, int r, Color top, Color bot, Color border, float bw, bool gloss, Vector4 slice)
    {
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear; tex.wrapMode = TextureWrapMode.Clamp;
        var px = new Color[w * h];
        for (int y = 0; y < h; y++)
        {
            float t = (h <= 1) ? 0f : (float)y / (h - 1);
            Color fill = Color.Lerp(bot, top, t);
            for (int x = 0; x < w; x++)
            {
                float d = SdRound(x + 0.5f, y + 0.5f, w, h, r);
                float cov = Mathf.Clamp01(0.5f - d);
                Color c = fill;
                if (gloss)
                {
                    float g = Mathf.Clamp01((t - 0.52f) / 0.48f);
                    c = Color.Lerp(c, new Color(Mathf.Min(1, c.r + 0.35f), Mathf.Min(1, c.g + 0.32f), Mathf.Min(1, c.b + 0.28f)), g * g * 0.9f);
                    float lg = Mathf.Clamp01((0.40f - t) / 0.40f);
                    c = Color.Lerp(c, c * 0.80f, lg * 0.55f);
                }
                float inside = -d;
                if (bw > 0 && inside >= 0 && inside < bw)
                    c = Color.Lerp(c, border, Mathf.Clamp01((1f - inside / bw) * 1.25f));
                px[y * w + x] = new Color(c.r, c.g, c.b, cov);
            }
        }
        tex.SetPixels(px); tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), C, 100f, 0, SpriteMeshType.FullRect, slice);
    }

    static Sprite Ball(int size)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear; tex.wrapMode = TextureWrapMode.Clamp;
        var px = new Color[size * size];
        float cx = size / 2f, cy = size / 2f, Rr = size / 2f - 1.5f;
        Color top = new Color(1f, 0.93f, 0.62f), bot = new Color(0.74f, 0.52f, 0.14f), rim = new Color(0.46f, 0.31f, 0.07f);
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float dx = x + 0.5f - cx, dy = y + 0.5f - cy;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float cov = Mathf.Clamp01(Rr - dist + 0.5f);
                float t = (float)y / (size - 1);
                Color c = Color.Lerp(bot, top, t);
                float edge = Mathf.Clamp01((dist - (Rr - 3f)) / 3f);
                c = Color.Lerp(c, rim, edge * 0.85f);
                float sx = cx - Rr * 0.34f, sy = cy + Rr * 0.40f;
                float sd = Mathf.Sqrt((x - sx) * (x - sx) + (y - sy) * (y - sy));
                float spec = Mathf.Clamp01(1f - sd / (Rr * 0.55f));
                c = Color.Lerp(c, Color.white, spec * spec * 0.7f);
                px[y * size + x] = new Color(c.r, c.g, c.b, cov);
            }
        tex.SetPixels(px); tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), C, 100f);
    }

    static void Stroke(Color[] px, int w, int h, float x0, float y0, float x1, float y1, float thick, Color col)
    {
        int minx = Mathf.Max(0, (int)(Mathf.Min(x0, x1) - thick - 1));
        int maxx = Mathf.Min(w - 1, (int)(Mathf.Max(x0, x1) + thick + 1));
        int miny = Mathf.Max(0, (int)(Mathf.Min(y0, y1) - thick - 1));
        int maxy = Mathf.Min(h - 1, (int)(Mathf.Max(y0, y1) + thick + 1));
        float dx = x1 - x0, dy = y1 - y0; float len2 = dx * dx + dy * dy;
        for (int y = miny; y <= maxy; y++)
            for (int x = minx; x <= maxx; x++)
            {
                float t = len2 <= 0 ? 0 : Mathf.Clamp01(((x - x0) * dx + (y - y0) * dy) / len2);
                float px2 = x0 + t * dx, py2 = y0 + t * dy;
                float dd = Mathf.Sqrt((x - px2) * (x - px2) + (y - py2) * (y - py2));
                float a = Mathf.Clamp01(thick * 0.5f - dd + 0.5f);
                if (a <= 0) continue;
                int idx = y * w + x;
                Color prev = px[idx];
                float na = Mathf.Max(prev.a, a * col.a);
                px[idx] = new Color(col.r, col.g, col.b, na);
            }
    }

    static Sprite Check(int size)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear; tex.wrapMode = TextureWrapMode.Clamp;
        var px = new Color[size * size];
        for (int i = 0; i < px.Length; i++) px[i] = new Color(1, 1, 1, 0);
        float s = size; float th = size * 0.17f;
        Stroke(px, size, size, s * 0.22f, s * 0.52f, s * 0.43f, s * 0.30f, th, Color.white);
        Stroke(px, size, size, s * 0.43f, s * 0.30f, s * 0.78f, s * 0.72f, th, Color.white);
        tex.SetPixels(px); tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), C, 100f);
    }

    static Sprite Chevron(int size)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear; tex.wrapMode = TextureWrapMode.Clamp;
        var px = new Color[size * size];
        for (int i = 0; i < px.Length; i++) px[i] = new Color(1, 1, 1, 0);
        float s = size; float th = size * 0.14f;
        Stroke(px, size, size, s * 0.26f, s * 0.62f, s * 0.50f, s * 0.38f, th, Color.white);
        Stroke(px, size, size, s * 0.50f, s * 0.38f, s * 0.74f, s * 0.62f, th, Color.white);
        tex.SetPixels(px); tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), C, 100f);
    }
}
