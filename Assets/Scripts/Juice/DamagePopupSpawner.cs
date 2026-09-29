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
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Spawn(Vector3 worldPos, int amount)
    {
        DamagePopup popup;

        if (popupPrefab != null)
        {
            popup = Instantiate(popupPrefab, worldPos, Quaternion.identity);
        }
        else
        {
            // Build a world-space number from scratch. TextMeshPro (the 3D component)
            // renders via MeshRenderer, so transform.position is real world space.
            GameObject go = new GameObject("DamagePopup");
            go.transform.position = worldPos;

            TextMeshPro tmp = go.AddComponent<TextMeshPro>();
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.rectTransform.sizeDelta = new Vector2(4f, 2f);

            popup = go.AddComponent<DamagePopup>();
        }

        popup.Setup(amount);
    }
}
