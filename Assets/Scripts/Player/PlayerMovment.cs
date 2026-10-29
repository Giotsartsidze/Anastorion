using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 15f; // ძალა, რომლითაც ვაწვებით პერსონაჟს
    public float maxSpeed = 12f;  // უმაღლესი სიჩქარე (upgrades won't exceed this)
    [Tooltip("How hard the player brakes when no key is held (higher = snappier stop).")]
    public float stopDeceleration = 100f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private PlayerDash dash;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dash = GetComponent<PlayerDash>();
    }

    void Update()
    {
        // ვკითხულობთ Input-ს (WASD ან ისრები)
        // GetAxisRaw გვაძლევს 0, 1 ან -1-ს (უფრო მკაფიო კონტროლისთვის)
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        // ფიზიკასთან მუშაობა ყოველთვის FixedUpdate-ში ხდება
        MovePlayer();
    }

    void MovePlayer()
    {
        bool dashing = dash != null && dash.IsDashing;

        if (moveInput != Vector2.zero)
        {
            rb.AddForce(moveInput.normalized * moveSpeed, ForceMode2D.Force);
        }
        else if (!dashing)
        {
            // brake quickly when no key is held, so the player stops instead of coasting
            rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, Vector2.zero, stopDeceleration * Time.fixedDeltaTime);
        }

        // clamp so speed upgrades don't make the player infinitely fast — but NOT during a
        // dash, so the dash actually reaches its Dash Speed instead of being capped to maxSpeed.
        if (!dashing)
            rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxSpeed);
    }
}