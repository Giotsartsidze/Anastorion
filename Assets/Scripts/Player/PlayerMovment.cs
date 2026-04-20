using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 15f; // ძალა, რომლითაც ვაწვებით პერსონაჟს
    public float maxSpeed = 12f;  // უმაღლესი სიჩქარე (upgrades won't exceed this)
    
    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
        if (moveInput != Vector2.zero)
        {
            // ვიყენებთ AddForce-ს, რომ მივიღოთ რბილი, წყალქვეშა ცურვის ეფექტი
            rb.AddForce(moveInput.normalized * moveSpeed, ForceMode2D.Force);
        }
        // clamp so speed upgrades don't make the player infinitely fast
        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxSpeed);
    }
}