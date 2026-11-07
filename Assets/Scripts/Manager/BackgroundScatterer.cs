using UnityEngine;

/// <summary>
/// Scatters non-colliding decorative props (rocks, bones, dead trees, ruins...) across
/// the ground so the map stops looking empty. Pure visual — no colliders, sorted far
/// behind gameplay. Put ONE of these in the scene, drag the prop sprites into the list.
/// Deterministic per seed, so the layout is stable between runs (change the seed to reroll).
/// </summary>
public class BackgroundScatterer : MonoBehaviour
{
    [Header("Props")]
    public Sprite[] propSprites;

    [Header("Field")]
    [Tooltip("How many props to scatter.")]
    public int count = 250;
    [Tooltip("Size of the rectangular field, centered on this object.")]
    public Vector2 area = new Vector2(90f, 90f);
    [Tooltip("Keep this radius around the center clear (player spawn).")]
    public float clearRadius = 5f;
    public int seed = 12345;

    [Header("Look")]
    public Vector2 scaleRange = new Vector2(0.6f, 1.3f);
    [Tooltip("Random darkening so props vary (1 = full bright, lower = dimmer).")]
    public Vector2 brightnessRange = new Vector2(0.6f, 1f);
    public string sortingLayerName = "Default";
    [Tooltip("Very negative so props render behind the ground gameplay.")]
    public int sortingOrder = -100;

    [Header("Arena Walls")]
    [Tooltip("Ring the playable area with visible walls + a physical barrier.")]
    public bool buildWalls = true;
    [Tooltip("Playable arena size — match BG_Floor (80 x 50). Walls border this rectangle.")]
    public Vector2 arenaSize = new Vector2(80f, 50f);
    [Tooltip("How far the dark shadow band extends outward to hide the void past the arena.")]
    public float wallOverhang = 90f;
    public Color wallColor = new Color(0.035f, 0.045f, 0.085f, 1f); // shadow
    public Color trimColor = new Color(0.93f, 0.78f, 0.42f, 1f);    // gold
    [Tooltip("Thickness of the invisible barrier that stops the player.")]
    public float colliderThickness = 2f;
    [Tooltip("Below the player/enemies (sorting 0) so they walk IN FRONT of the wall, " +
             "but above the scattered props (-100) so the void beyond stays hidden.")]
    public int wallSortingOrder = -50;
    static Sprite _solid;

    // The playable rectangle in world space, published once the walls are built, so
    // other systems (e.g. auto-aim) can ignore anything that wandered outside it.
    public static Rect ArenaBounds;
    public static bool HasArenaBounds;
    public static bool InsideArena(Vector2 p) => !HasArenaBounds || ArenaBounds.Contains(p);

    void Start()
    {
        if (buildWalls) BuildWalls();
        if (propSprites == null || propSprites.Length == 0) return;

        // Seed locally, then restore global RNG so we don't disturb gameplay randomness.
        Random.State prev = Random.state;
        Random.InitState(seed);

        Transform parent = new GameObject("BackgroundProps").transform;
        parent.SetParent(transform, false);

        Vector3 center = transform.position;
        float clearSqr = clearRadius * clearRadius;

        for (int i = 0; i < count; i++)
        {
            Vector2 offset = new Vector2(
                Random.Range(-area.x * 0.5f, area.x * 0.5f),
                Random.Range(-area.y * 0.5f, area.y * 0.5f));

            if (offset.sqrMagnitude < clearSqr) continue; // keep spawn area clean

            GameObject p = new GameObject("prop");
            p.transform.SetParent(parent, false);
            p.transform.position = center + (Vector3)offset;

            float s = Random.Range(scaleRange.x, scaleRange.y);
            p.transform.localScale = new Vector3(s, s, 1f);

            SpriteRenderer sr = p.AddComponent<SpriteRenderer>();
            sr.sprite = propSprites[Random.Range(0, propSprites.Length)];
            sr.flipX = Random.value < 0.5f;
            if (!string.IsNullOrEmpty(sortingLayerName)) sr.sortingLayerName = sortingLayerName;
            // spread props slightly in order so overlaps layer naturally
            sr.sortingOrder = sortingOrder + (i % 20);

            float b = Random.Range(brightnessRange.x, brightnessRange.y);
            sr.color = new Color(b, b, b, 1f);

            ShadowFx.Attach(p.transform, 0.7f, 0.28f); // subtle shadow grounds the prop
        }

        Random.state = prev;
    }

    // Visible boundary around the arena: dark shadow bands (hide the void beyond),
    // a gold inner trim, and thin colliders that physically contain the player.
    void BuildWalls()
    {
        float hx = arenaSize.x * 0.5f, hy = arenaSize.y * 0.5f;
        Vector3 c = transform.position;
        Transform root = new GameObject("ArenaWalls").transform;
        root.SetParent(transform, false);

        // Publish the playable rectangle so auto-aim / skills can skip out-of-bounds enemies.
        ArenaBounds = new Rect(c.x - hx, c.y - hy, arenaSize.x, arenaSize.y);
        HasArenaBounds = true;

        // Dark shadow bands. Top/bottom span full width (incl. corners); left/right fill the sides.
        MakePanel(root, c + new Vector3(0, hy + wallOverhang * 0.5f, 0), new Vector2(arenaSize.x + wallOverhang * 2f, wallOverhang));
        MakePanel(root, c + new Vector3(0, -hy - wallOverhang * 0.5f, 0), new Vector2(arenaSize.x + wallOverhang * 2f, wallOverhang));
        MakePanel(root, c + new Vector3(-hx - wallOverhang * 0.5f, 0, 0), new Vector2(wallOverhang, arenaSize.y));
        MakePanel(root, c + new Vector3(hx + wallOverhang * 0.5f, 0, 0), new Vector2(wallOverhang, arenaSize.y));

        // Gold trim along the inner edges.
        float t = 0.6f;
        MakeTrim(root, c + new Vector3(0, hy, 0), new Vector2(arenaSize.x, t));
        MakeTrim(root, c + new Vector3(0, -hy, 0), new Vector2(arenaSize.x, t));
        MakeTrim(root, c + new Vector3(-hx, 0, 0), new Vector2(t, arenaSize.y));
        MakeTrim(root, c + new Vector3(hx, 0, 0), new Vector2(t, arenaSize.y));

        // Physical barriers just outside each edge (block the player's rigidbody).
        MakeCollider(root, c + new Vector3(0, hy + colliderThickness * 0.5f, 0), new Vector2(arenaSize.x, colliderThickness));
        MakeCollider(root, c + new Vector3(0, -hy - colliderThickness * 0.5f, 0), new Vector2(arenaSize.x, colliderThickness));
        MakeCollider(root, c + new Vector3(-hx - colliderThickness * 0.5f, 0, 0), new Vector2(colliderThickness, arenaSize.y));
        MakeCollider(root, c + new Vector3(hx + colliderThickness * 0.5f, 0, 0), new Vector2(colliderThickness, arenaSize.y));
    }

    void MakePanel(Transform parent, Vector3 pos, Vector2 size)
    {
        var go = new GameObject("WallPanel");
        go.transform.SetParent(parent, false); go.transform.position = pos;
        go.transform.localScale = new Vector3(size.x, size.y, 1f);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = SolidSprite(); sr.color = wallColor;
        if (!string.IsNullOrEmpty(sortingLayerName)) sr.sortingLayerName = sortingLayerName;
        sr.sortingOrder = wallSortingOrder;
    }

    void MakeTrim(Transform parent, Vector3 pos, Vector2 size)
    {
        var go = new GameObject("WallTrim");
        go.transform.SetParent(parent, false); go.transform.position = pos;
        go.transform.localScale = new Vector3(size.x, size.y, 1f);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = SolidSprite(); sr.color = trimColor;
        if (!string.IsNullOrEmpty(sortingLayerName)) sr.sortingLayerName = sortingLayerName;
        sr.sortingOrder = wallSortingOrder + 2;
    }

    void MakeCollider(Transform parent, Vector3 pos, Vector2 size)
    {
        var go = new GameObject("WallCollider");
        go.transform.SetParent(parent, false); go.transform.position = pos;
        var bc = go.AddComponent<BoxCollider2D>();
        bc.size = size;
    }

    static Sprite SolidSprite()
    {
        if (_solid != null) return _solid;
        var tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
        var px = new Color[16]; for (int i = 0; i < 16; i++) px[i] = Color.white;
        tex.SetPixels(px); tex.Apply();
        tex.wrapMode = TextureWrapMode.Clamp;
        _solid = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f); // 1 world unit
        return _solid;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(area.x, area.y, 0f));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, clearRadius);
    }
}
