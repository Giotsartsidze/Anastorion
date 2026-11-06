using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shared visual skin so every pop-up panel matches the Settings/Shop look.
/// Generates the SAME procedural navy + gold rounded frame the Settings panel uses
/// (one source of truth) and drops it in as a card BEHIND a panel's existing content,
/// so nothing moves or gets hidden — it only adds the consistent gold frame.
/// Call UiSkin.Frame(panel, size) from a panel's manager (works on inactive panels).
/// </summary>
public static class UiSkin
{
    static Sprite _panel;

    public static Sprite PanelSprite()
    {
        if (_panel == null)
            _panel = RoundRect(176, 176, 30,
                new Color(0.075f, 0.095f, 0.185f), new Color(0.040f, 0.052f, 0.110f),
                new Color(0.83f, 0.66f, 0.32f), 3.2f, new Vector4(30, 30, 30, 30));
        return _panel;
    }

    /// <summary>Add the shared gold-framed card behind this panel's content.
    /// Pass cardSize.x/y <= 0 to auto-fit the card to the content bounds.</summary>
    public static void Frame(GameObject root, Vector2 cardSize, Vector2 offset = default)
    {
        if (root == null) return;
        var t = root.transform;
        if (t.Find("__GoldCard") != null) return; // already skinned

        var rootRT = root.GetComponent<RectTransform>();
        bool rootFullScreen = IsFullScreen(rootRT);
        RootSize(rootRT, out float W, out float H);

        // place the card just above any backdrop/scrim, but below the content
        int idx = 0;
        for (int i = 0; i < t.childCount; i++)
        {
            var c = t.GetChild(i) as RectTransform;
            if (c == null) continue;
            if (IsBackdrop(c, W, H)) { idx = i + 1; continue; }
            break;
        }

        // auto-fit the card to the content when no explicit size is given
        if (cardSize.x <= 0f || cardSize.y <= 0f)
        {
            if (ContentBounds(rootRT, W, H, out Rect b))
            {
                const float pad = 34f;
                cardSize = new Vector2(b.width + pad * 2f, b.height + pad * 2f);
                offset = b.center;
            }
            else cardSize = new Vector2(480, 360);
        }

        var card = NewImg("__GoldCard", t, PanelSprite());
        card.type = Image.Type.Sliced; card.color = Color.white; card.raycastTarget = true;
        var rt = card.rectTransform;
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f); rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = cardSize; rt.anchoredPosition = offset;
        card.transform.SetSiblingIndex(idx);

        // If the root is a bounded box, its own flat background would clash with the
        // gold card, so hide it. If the root is a full-screen dim, keep it (it's the
        // modal backdrop) and just let the card sit on top of it.
        var ri = root.GetComponent<Image>();
        if (ri != null && !rootFullScreen) ri.enabled = false;
    }

    static bool IsFullScreen(RectTransform rt)
    {
        if (rt == null) return false;
        return rt.anchorMin.x < 0.05f && rt.anchorMin.y < 0.05f &&
               rt.anchorMax.x > 0.95f && rt.anchorMax.y > 0.95f;
    }

    // Reliable root size: point-anchored panels use sizeDelta directly (valid even
    // while inactive); stretched panels fall back to the computed rect.
    static void RootSize(RectTransform rt, out float W, out float H)
    {
        if (rt != null && rt.anchorMin == rt.anchorMax) { W = rt.sizeDelta.x; H = rt.sizeDelta.y; }
        else { W = rt != null ? rt.rect.width : 0f; H = rt != null ? rt.rect.height : 0f; }
    }

    // A scrim / dim / backdrop: named so, stretched full-screen, or larger than the
    // panel box itself. These are never counted as content.
    static bool IsBackdrop(RectTransform c, float W, float H)
    {
        if (c == null || c.name == "__GoldCard") return false;
        string n = c.name.ToLowerInvariant();
        if (n.Contains("scrim") || n.Contains("dim") || n.Contains("overlay")) return true;
        if (IsFullScreen(c)) return true;
        float w = (c.anchorMax.x - c.anchorMin.x) * W + c.sizeDelta.x;
        float h = (c.anchorMax.y - c.anchorMin.y) * H + c.sizeDelta.y;
        return W > 1f && H > 1f && w >= W * 1.05f && h >= H * 1.05f;
    }

    /// <summary>Union AABB (root-local) of the panel's content children, skipping any
    /// full-screen scrim and the card itself. Computed from serialized rect data so it
    /// works even while the panel is inactive.</summary>
    static bool ContentBounds(RectTransform root, float W, float H, out Rect bounds)
    {
        bounds = new Rect();
        if (root == null || W <= 1f || H <= 1f) return false;
        bool any = false;
        float xmin = 1e9f, ymin = 1e9f, xmax = -1e9f, ymax = -1e9f;
        for (int i = 0; i < root.childCount; i++)
        {
            var c = root.GetChild(i) as RectTransform;
            if (c == null || c.name == "__GoldCard" || IsBackdrop(c, W, H)) continue;
            float w = (c.anchorMax.x - c.anchorMin.x) * W + c.sizeDelta.x;
            float h = (c.anchorMax.y - c.anchorMin.y) * H + c.sizeDelta.y;
            if (w <= 0f || h <= 0f) continue;
            float axc = ((c.anchorMin.x + c.anchorMax.x) * 0.5f - 0.5f) * W;
            float ayc = ((c.anchorMin.y + c.anchorMax.y) * 0.5f - 0.5f) * H;
            float cx = axc + c.anchoredPosition.x + (0.5f - c.pivot.x) * w;
            float cy = ayc + c.anchoredPosition.y + (0.5f - c.pivot.y) * h;
            xmin = Mathf.Min(xmin, cx - w * 0.5f); xmax = Mathf.Max(xmax, cx + w * 0.5f);
            ymin = Mathf.Min(ymin, cy - h * 0.5f); ymax = Mathf.Max(ymax, cy + h * 0.5f);
            any = true;
        }
        if (!any) return false;
        bounds = new Rect(xmin, ymin, xmax - xmin, ymax - ymin);
        return true;
    }

    static Image NewImg(string name, Transform parent, Sprite sp)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var im = go.AddComponent<Image>(); im.sprite = sp; im.color = Color.white;
        return im;
    }

    // ---- procedural rounded frame (identical look to the Settings panel) ----
    static float SdRound(float px, float py, float w, float h, float r)
    {
        float qx = Mathf.Abs(px - w * 0.5f) - (w * 0.5f - r);
        float qy = Mathf.Abs(py - h * 0.5f) - (h * 0.5f - r);
        float ox = Mathf.Max(qx, 0), oy = Mathf.Max(qy, 0);
        return Mathf.Sqrt(ox * ox + oy * oy) + Mathf.Min(Mathf.Max(qx, qy), 0) - r;
    }

    static Sprite RoundRect(int w, int h, int r, Color top, Color bot, Color border, float bw, Vector4 slice)
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
                Color c = fill; float inside = -d;
                if (bw > 0 && inside >= 0 && inside < bw)
                    c = Color.Lerp(c, border, Mathf.Clamp01((1f - inside / bw) * 1.25f));
                px[y * w + x] = new Color(c.r, c.g, c.b, cov);
            }
        }
        tex.SetPixels(px); tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, slice);
    }
}
