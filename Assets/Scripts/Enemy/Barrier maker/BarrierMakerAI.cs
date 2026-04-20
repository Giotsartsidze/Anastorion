using UnityEngine;

public class BarrierMaker : MonoBehaviour
{
    [Header("Partner Logic")]
    public GameObject partnerPrefab;
    public LineRenderer line;
    public GameObject partner;
    public bool isMaster = false;

    [Header("Movement")]
    public float speed = 2f;
    private Transform player;

    [Header("Laser")]
    public float laserDamageInterval = 0.5f;
    private float laserDamageTimer = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (partner == null)
        {
            isMaster = true;
            partner = Instantiate(partnerPrefab, transform.position + Vector3.right * 4f, Quaternion.identity);
            
            if (partner.TryGetComponent<BarrierMaker>(out BarrierMaker partnerScript))
            {
                partnerScript.partner = gameObject;
                partnerScript.isMaster = false;
            }
        }

        // ხაზის საწყისი პარამეტრები კოდიდან, რომ ინსპექტორზე არ ვიყოთ დამოკიდებული
        if (line != null)
        {
            line.positionCount = 2;
            line.startWidth = 0.15f;
            line.endWidth = 0.15f;
            line.sortingOrder = 5; // რომ ყოველთვის ზემოდან ჩანდეს
        }
    }

    void Update()
    {
        if (player != null)
        {
            // 1. მოძრაობა ფლეიერისკენ
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            
            // 2. Sprite Flip (რომ ფლეიერს უყუროს)
            transform.localScale = new Vector3(player.position.x < transform.position.x ? -1 : 1, 1, 1);
        }

        // 3. ლაზერის ლოგიკა
        HandleLaser();
    }

    void HandleLaser()
    {
        if (partner == null || line == null) 
        { 
            if(line != null) line.enabled = false; 
            return; 
        }

        line.enabled = true;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, partner.transform.position);

        // ზიანის მიყენება (Linecast) — cooldown prevents per-frame damage
        laserDamageTimer -= Time.deltaTime;
        RaycastHit2D hit = Physics2D.Linecast(transform.position, partner.transform.position, LayerMask.GetMask("Player"));
        if (hit.collider != null && laserDamageTimer <= 0f)
        {
            hit.collider.GetComponent<PlayerHealth>()?.TakeDamage(1);
            laserDamageTimer = laserDamageInterval;
        }
    }
}