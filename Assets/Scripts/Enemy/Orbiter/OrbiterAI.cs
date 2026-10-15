using UnityEngine;

public class OrbiterAI : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public float currentRadius = 8f;
    public float closingSpeed = 0.5f;   // რამდენად სწრაფად ვიწროვდება წრე
    public float minRadius = 2.5f;       // never orbit closer than this (was 0.5 -> sat on the player)

    private float angle;
    private float startRadius;
    private Transform player;

    void Awake()
    {
        startRadius = currentRadius; // remember the designed start radius for pooled respawns
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    // Pooling-safe: every respawn starts out at the full radius again, from its
    // actual spawn position — so a reused orbiter never pops in on top of the player.
    void OnEnable()
    {
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        currentRadius = startRadius;
        if (player != null)
        {
            Vector2 dir = transform.position - player.position;
            angle = Mathf.Atan2(dir.y, dir.x);
        }
    }

    void Update()
    {
        if (player == null) return;

        angle += rotationSpeed * Time.deltaTime * Mathf.Deg2Rad;
        currentRadius -= closingSpeed * Time.deltaTime;
        if (currentRadius < minRadius) currentRadius = minRadius; // stay dodgeable/killable

        float x = player.position.x + Mathf.Cos(angle) * currentRadius;
        float y = player.position.y + Mathf.Sin(angle) * currentRadius;
        transform.position = new Vector3(x, y, 0);
    }
}
