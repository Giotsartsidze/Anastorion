using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Scene-level singleton that spawns floating damage numbers.
/// Put ONE of these in the gameplay scene — no prefab required. It builds each
/// number in code as a true world-space TextMeshPro so it always tracks the enemy.
/// (You can still assign a prefab to override the look.)
/// Anything that deals damage can call: DamagePopupSpawner.Instance?.Spawn(pos, amount)
/// </summary>
public class DamagePopupSpawner : MonoBehaviour
{
    public static DamagePopupSpawner Instance { get; private set; }

    [Tooltip("OPTIONAL. Leave empty to auto-generate numbers in code with the style below.")]
    public DamagePopup popupPrefab;

    [Header("Auto-generated style (used when no prefab)")]
    public float fontSize = 6f;
    public Color color = Color.white;

    void Awake()
    {
        Instance = this; // newest wins — survives scene reload without vanishing
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // Pooled so a hit-heavy horde doesn't build a fresh TextMeshPro every single hit.
    private readonly Queue<DamagePopup> pool = new Queue<DamagePopup>();

    public void Spawn(Vector3 worldPos, int amount)
    {
        DamagePopup popup = GetOrCreate();
        popup.transform.position = worldPos;
        popup.gameObject.SetActive(true);
        popup.Setup(amount);
    }

    DamagePopup GetOrCreate()
    {
        while (pool.Count > 0) { var p = pool.Dequeue(); if (p != null) return p; } // reuse

        if (popupPrefab != null)
            return Instantiate(popupPrefab);

        // Build a world-space number from scratch (TextMeshPro renders via MeshRenderer).
        GameObject go = new GameObject("DamagePopup");
        TextMeshPro tmp = go.AddComponent<TextMeshPro>();
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.rectTransform.sizeDelta = new Vector2(4f, 2f);
        return go.AddComponent<DamagePopup>();
    }

    public void Return(DamagePopup p)
    {
        if (p == null) return;
        p.gameObject.SetActive(false);
        pool.Enqueue(p);
    }
}
