using UnityEngine;
using System.Collections.Generic;

public class EliteModifier : MonoBehaviour
{
    [Header("Elite Settings")]
    public float eliteChance = 10f;
    public float healthMultiplier = 3f;
    public float scaleMultiplier = 1.3f;
    public GameObject auraVisual; // ჩააგდე Circle Sprite აქ

    [Header("Aura Buff Settings")]
    public float auraRadius = 5f;
    public float speedMultiplier = 1.25f; // 25%-ით აჩქარება
    public LayerMask enemyLayer;

	[Header("Visual Identifier")]
	public GameObject eliteIcon;

    private bool isElite = false;
    private List<GameObject> buffedEnemies = new List<GameObject>();

    void Start()
    {
        if (Random.Range(0, 100) <= eliteChance)
        {
            MakeElite();
        }
    }

    void MakeElite()
    {
        isElite = true;
        transform.localScale *= scaleMultiplier;
        GetComponent<SpriteRenderer>().color = new Color(1, 0.5f, 0, 1); // ნარინჯისფერი

        if (auraVisual != null) auraVisual.SetActive(true);

        EnemyHealth healthScript = GetComponent<EnemyHealth>();
        if (healthScript != null)
        {
            // თუ ეს მტერი ტანკია (ანუ Health Drop-ს აგდებს)
            if (healthScript.dropType == EnemyHealth.DropType.Health)
            {
                healthScript.dropCount = 3; // დააგდოს 3 გული
                healthScript.health *= 2;   // კიდევ უფრო მეტი HP ელიტარულ ტანკს
            }
            else
            {
                healthScript.dropCount = 5; // ჩვეულებრივ ელიტარს - 5 XP
            }
        }

		if (eliteIcon != null) 
    	{
        eliteIcon.SetActive(true);
        // შეგვიძლია ფერიც დავუმთხვიოთ აურას
        eliteIcon.GetComponent<SpriteRenderer>().color = Color.yellow;
   		}

        // ვიწყებთ პერიოდულ შემოწმებას (0.5 წამში ერთხელ)
       	if (auraVisual != null) auraVisual.SetActive(true);
    	InvokeRepeating(nameof(ApplyAuraBuff), 0.5f, 0.5f);
    }

    void ApplyAuraBuff()
    {
        if (!isElite) return;

        // ვპოულობთ ყველა მტერს რადიუსში
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, auraRadius, enemyLayer);
        
        foreach (var enemy in hitEnemies)
        {
            // საკუთარ თავს არ ვუფერადებთ
            if (enemy.gameObject == this.gameObject) continue;

            // ვამოწმებთ, მტერი უკვე აჩქარებულია თუ არა (რომ სტეკინგი არ მოხდეს)
            EnemyAI ai = enemy.GetComponent<EnemyAI>() ?? enemy.GetComponentInParent<EnemyAI>();
            if (ai != null && !ai.isBuffed)
            {
                ai.ApplySpeedBuff(speedMultiplier);
            }
        }
    }
    
    // რადიუსის დახატვა ედიტორში (ვიზუალური კონტროლისთვის)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, auraRadius);
    }

	void Update()
    {
       // EliteModifier-ის Update-ში
if (isElite && auraVisual != null)
{
    float pulse = 1f + Mathf.Sin(Time.time * 3f) * 0.1f;
    auraVisual.transform.localScale = new Vector3(pulse, pulse, 1);
}
    }
}