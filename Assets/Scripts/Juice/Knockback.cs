using UnityEngine;

/// <summary>
/// Universal knockback that works even when an enemy's AI sets transform.position
/// directly every frame. The push is applied as an additive offset in LateUpdate
/// (which runs AFTER the AI's Update), so it nudges the enemy without being
/// overwritten, then decays back to zero.
/// </summary>
[DisallowMultipleComponent]
public class Knockback : MonoBehaviour
{
    [Tooltip("How fast the push slows down (units/sec^2). Higher = shorter, snappier push.")]
    public float deceleration = 50f;

    private Vector2 velocity;

    public void Apply(Vector2 direction, float force)
    {
        velocity = direction.normalized * force;
    }

    void LateUpdate()
    {
        if (velocity.sqrMagnitude < 0.0001f) return;
        transform.position += (Vector3)(velocity * Time.deltaTime);
        velocity = Vector2.MoveTowards(velocity, Vector2.zero, deceleration * Time.deltaTime);
    }

    void OnDisable()
    {
        velocity = Vector2.zero; // pooling safety — don't carry a push into the next spawn
    }
}
