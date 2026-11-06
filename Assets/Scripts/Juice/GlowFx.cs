using UnityEngine;

// Adds a soft glowing halo behind a sprite (pickups, projectiles) so the bloom
// post-process has something luminous to grab — makes collectibles and shots sparkle.
// Halo sprite is generated in code. Call GlowFx.Attach(transform, color).
public static class GlowFx
{
    static Sprite halo;

    static Sprite Halo()
    {
        if (halo != null) return halo;
        const int S = 64;
        var tex = new Texture2D(S, S, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
        var cols = new Color32[S * S];
        float c = S * 0.5f;
        for (int y = 0; y < S; y++)
            for (int x = 0; x < S; x++)
            {
                float dx = (x - c) / (S * 0.5f);
                float dy = (y - c) / (S * 0.5f);
                float d = Mathf.Sqrt(dx * dx + dy * dy);
                float a = Mathf.Clamp01(1f - d);
                a = a * a * a;                       // bright tight core, soft edge
                cols[y * S + x] = new Color32(255, 255, 255, (byte)(a * 255f));
            }
        tex.SetPixels32(cols);
        tex.Apply();
        halo = Sprite.Create(tex, new Rect(0, 0, S, S), new Vector2(0.5f, 0.5f), 100f);
        return halo;
    }

    public static void Attach(Transform owner, Color color, float size = 1.3f)
    {
        var sp = Halo();
        if (sp == null || owner == null) return;

        var ownerSR = owner.GetComponentInChildren<SpriteRenderer>();
        var go = new GameObject("Glow");
        go.transform.SetParent(owner, false);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sp;
        sr.color = color;
        if (ownerSR != null)
        {
            sr.sortingLayerID = ownerSR.sortingLayerID;
            sr.sortingOrder = ownerSR.sortingOrder - 1; // behind the pickup
        }

        float shW = sp.bounds.size.x;
        float s = shW > 0f ? size / shW : 1f;
        go.transform.localScale = new Vector3(s, s, 1f);
        go.transform.localPosition = Vector3.zero;
    }
}
