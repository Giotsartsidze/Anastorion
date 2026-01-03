using UnityEngine;
using System.Collections;

public class PlayerDash : MonoBehaviour
{
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.5f;
    
    private bool canDash = true;
    private bool isDashing;
    private Rigidbody2D rb;
    public TrailRenderer dashTrail;
    void Start() => rb = GetComponent<Rigidbody2D>();

    void Update()
    {
        if (isDashing) return;

        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        dashTrail.emitting = true; // კუდის გამოჩენა
        

        // ვიღებთ მოძრაობის მიმართულებას PlayerMovement-იდან
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 dashDir = new Vector2(x, y).normalized;

        // თუ არ ვმოძრაობთ, წინ გადახტეს (მაგალითად მარჯვნივ)
        if (dashDir == Vector2.zero) dashDir = Vector2.right;

        rb.linearVelocity = dashDir * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        rb.linearVelocity = Vector2.zero; // Dash-ის მერე გაჩერდეს

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
        dashTrail.emitting = false; // კუდის გაქრობა
        
		FindObjectOfType<UpgradeManager>().TryTriggerLightning();	
    }
}