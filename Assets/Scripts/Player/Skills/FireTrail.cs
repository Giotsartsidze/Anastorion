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
    public bool active = true;           // flip on when unlocked

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
            Instantiate(firePatchPrefab, transform.position, Quaternion.identity);
            lastDropPos = transform.position;
            timer = 0f;
        }
    }
}
