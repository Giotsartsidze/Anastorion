using UnityEngine;

public class OrbiterAI : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public float currentRadius = 8f;
    public float closingSpeed = 0.5f; // რამდენად სწრაფად ვიწროვდება წრე
    private float angle;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // საწყისი კუთხის განსაზღვრა პოზიციის მიხედვით
        Vector2 dir = transform.position - player.position;
        angle = Mathf.Atan2(dir.y, dir.x);
    }

    void Update()
    {
        if (player == null) return;

        angle += rotationSpeed * Time.deltaTime * Mathf.Deg2Rad;
        currentRadius -= closingSpeed * Time.deltaTime;

        // მათემატიკური ფორმულა წრეზე მოძრაობისთვის
        float x = player.position.x + Mathf.Cos(angle) * currentRadius;
        float y = player.position.y + Mathf.Sin(angle) * currentRadius;

        transform.position = new Vector3(x, y, 0);

        if (currentRadius <= 0.5f) currentRadius = 0.5f; // რომ მოთამაშეს ბოლომდე არ "შეეჭამოს"
    }
}