using UnityEngine;

// Soft drop-shadow helper. Call ShadowFx.Attach(transform) to give any sprite a
// flattened dark blob at its feet, rendered just under it — grounds units/props so
// they sit in the world instead of floating. The shadow sprite is generated in code
// (no asset import needed, so it can't silently fail to load).
public static class ShadowFx
{
    static Sprite shadow;

    static Sprite Shadow()
    {
        if (shadow != null) return shadow;
        const int S = 64;
        var tex = new Texture2D(S, S, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
        var cols = new Color32[S * S];
        float cx = S * 0.5f, cy = S * 0.5f;
        for (int y = 0; y < S; y++)
            for (int x = 0; x < S; x++)
            {
                float dx = (x - cx) / (S * 0.5f);
                float dy = (y - cy) / (S * 0.5f);
                float d = Mathf.Sqrt(dx * dx + dy * dy);
                float a = Mathf.Clamp01(1f - d);
                a *= a;                                  // soft falloff
                cols[y * S + x] = new Color32(0, 0, 0, (byte)(a * 160f));
            }
        tex.SetPixels32(cols);
        tex.Apply();
        shadow = Sprite.Create(tex, new Rect(0, 0, S, S), new Vector2(0.5f, 0.5f), 100f);
        return shadow;
    }

    public static void Attach(Transform owner, float widthScale = 0.8f, float alpha = 0.5f)
    {
        var sp = Shadow();
        if (sp == null || owner == null) return;

        var ownerSR = owner.GetComponentInChildren<SpriteRenderer>();
        var go = new GameObject("Shadow");
        go.transform.SetParent(owner, false);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sp;
        sr.color = new Color(1f, 1f, 1f, alpha);

        float width = 1f, feet = -0.4f;
        if (ownerSR != null && ownerSR.sprite != null)
        {
            sr.sortingLayerID = ownerSR.sortingLayerID;
            sr.sortingOrder = ownerSR.sortingOrder - 1;
            var b = ownerSR.sprite.bounds.size;
            width = b.x * widthScale;
            feet = -b.y * 0.42f;
        }

        float shW = sp.bounds.size.x;              // ~0.64 world units
        float s = shW > 0f ? width / shW : 1f;
        go.transform.localScale = new Vector3(s, s * 0.5f, 1f);  // flattened
        go.transform.localPosition = new Vector3(0f, feet, 0f);
    }
}
