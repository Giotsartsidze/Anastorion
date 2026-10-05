using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour
{
    public GameObject projectilePrefab;
    public int projectileCount = 8; // რამდენი ისარი გავარდეს ერთდროულად
    public float attackInterval = 3f; // ყოველ 3 წამში
    public float projectileSpeed = 5f;

    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Bite (melee)")]
    public Sprite[] biteFrames;     // assign the gveleshapi_boss_attack_bite sheet
    public float meleeRange = 3f;
    public int biteDamage = 25;
    public float biteCooldown = 2.5f;

    private float biteTimer;
    private float patternOffset;
    private Transform player;
    private CharacterAnimator anim;

    void Start()
    {
        anim = GetComponent<CharacterAnimator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        StartCoroutine(AttackRoutine());
    }

    void Update()
    {
        if (player == null) return;
        float dist = Vector2.Distance(transform.position, player.position);

        // Chase the player — a moving threat, not a turret.
        if (dist > meleeRange)
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        // Bite when close.
        biteTimer -= Time.deltaTime;
        if (dist <= meleeRange && biteTimer <= 0f)
        {
            Bite();
            biteTimer = biteCooldown;
        }
    }

    void Bite()
    {
        if (anim != null && biteFrames != null && biteFrames.Length > 0)
            anim.PlayAttack(biteFrames);
        if (player != null && Vector2.Distance(transform.position, player.position) <= meleeRange + 0.5f)
            player.GetComponent<PlayerHealth>()?.TakeDamage(biteDamage);
    }

    IEnumerator AttackRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval);
            ShootStarPattern();
        }
    }

    void ShootStarPattern()
    {
        if (anim != null) anim.PlayAttack();
        float angleStep = 360f / projectileCount;
        float angle = patternOffset;   // rotate each volley so the gaps shift
        patternOffset += 13f;

        for (int i = 0; i < projectileCount; i++)
        {
            // ვიანგარიშებთ მიმართულებას წრეზე
            float dirX = transform.position.x + Mathf.Sin((angle * Mathf.Deg2Rad));
            float dirY = transform.position.y + Mathf.Cos((angle * Mathf.Deg2Rad));

            Vector3 projectileMoveVector = new Vector3(dirX, dirY, 0);
            Vector2 projectileDir = (projectileMoveVector - transform.position).normalized;

            // ვქმნით ტყვიას
            GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            
            // ვაძლევთ ტყვიას მიმართულებას და კუთხეს
            float rotationAngle = Mathf.Atan2(projectileDir.y, projectileDir.x) * Mathf.Rad2Deg;
            proj.transform.rotation = Quaternion.Euler(0, 0, rotationAngle);

            angle += angleStep;
        }
    }
}