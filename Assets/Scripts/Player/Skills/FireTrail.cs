using UnityEngine;

/// <summary>
/// Ember Path — leaves a lingering fire patch where the player moves, so kiting
/// through the horde burns everything you lead past. Put on the Player, assign the
/// fire-patch prefab. Only drops when you've actually moved, so standing still
/// doesn't stack a bonfire.
/// </summary>
public class FireTrail : MonoBehaviour
{
    public GameObject firePatchPrefab;   // prefab with a SpriteRenderer + FirePatch
    public float dropInterval = 0.4f;    // seconds between patches
    public float minMoveDistance = 0.4f; // only drop after moving this far
    public bool active = false;          // turned on when the Ember Path card is picked
    public int patchDamage = 2;          // raised by further Ember Path picks
    public float patchRadius = 1.2f;     // size of each fire patch
    public bool evolved = false;         // Comet evolution

    /// <summary>Comet evolution: a huge, scorching, rapid blazing trail.</summary>
    public void Evolve()
    {
        if (evolved) return;
        evolved = true;
        patchDamage *= 2;
        patchRadius += 1f;
        dropInterval = Mathf.Max(0.15f, dropInterval * 0.6f);
    }

    private float timer;
    private Vector3 lastDropPos;

    void Start()
    {
        lastDropPos = transform.position;
    }

    void Update()
    {
        if (!active || firePatchPrefab == null) return;

        timer += Time.deltaTime;
        if (timer >= dropInterval &&
            Vector3.Distance(transform.position, lastDropPos) >= minMoveDistance)
        {
            GameObject patch = Instantiate(firePatchPrefab, transform.position, Quaternion.identity);
            FirePatch fp = patch.GetComponent<FirePatch>();
            if (fp != null) { fp.damagePerTick = patchDamage; fp.radius = patchRadius; }
            lastDropPos = transform.position;
            timer = 0f;
        }
    }
}
