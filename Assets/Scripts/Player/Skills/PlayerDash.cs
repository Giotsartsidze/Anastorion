using UnityEngine;
using System.Collections;

public class PlayerDash : MonoBehaviour
{
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.5f;

    private bool canDash = true;
    private bool isDashing;
    public bool IsDashing => isDashing; // so movement doesn't brake mid-dash
    private Rigidbody2D rb;
    public TrailRenderer dashTrail;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Kill the persistent trail line before the first dash — it only streaks mid-dash now.
        if (dashTrail != null) dashTrail.emitting = false;
    }

    void Update()
    {
        if (isDashing) return;
        if (Input.GetKeyDown(KeyCode.Space) && canDash)
            StartCoroutine(Dash());
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        // ✅ null check TrailRenderer-ზე
        if (dashTrail != null) dashTrail.emitting = true;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 dashDir = new Vector2(x, y).normalized;
        if (dashDir == Vector2.zero) dashDir = Vector2.right;

        rb.linearVelocity = dashDir * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        // ✅ object destroy-და? coroutine-ი ჩერდება
        if (this == null || !gameObject.activeInHierarchy) yield break;

        isDashing = false;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(dashCooldown);

        // ✅ კვლავ ვამოწმებთ
        if (this == null || !gameObject.activeInHierarchy) yield break;

        canDash = true;
        if (dashTrail != null) dashTrail.emitting = false;

        FindObjectOfType<UpgradeManager>()?.TryTriggerLightning();
    }
}