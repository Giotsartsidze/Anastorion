using UnityEngine;

public class EliteModifier : MonoBehaviour
{
    [Header("Elite Settings")]
    [Range(0, 100)]
    public float eliteChance = 10f; // 10% შანსი
    public float healthMultiplier = 3f;
    public float scaleMultiplier = 1.5f;
    public Color eliteColor = new Color(1f, 0.8f, 0f); // ოქროსფერი

    private bool isElite = false;

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

        // 1. ვზრდით ზომას
        transform.localScale *= scaleMultiplier;

        // 2. ვუცვლით ფერს (ვიზუალური ინდიკატორი)
        GetComponent<SpriteRenderer>().color = eliteColor;

        // 3. ვუზრდით სიცოცხლეს (მივმართავთ EnemyHealth-ს)
        EnemyHealth healthScript = GetComponent<EnemyHealth>();
        if (healthScript != null)
        {
        healthScript.xpCount = 5;
            healthScript.health = Mathf.RoundToInt(healthScript.health * healthMultiplier);
        }
        
        // 4. ბონუსი: შეგვიძლია დავამატოთ აურა ან სხვა ეფექტი
        Debug.Log(gameObject.name + " has evolved into an ELITE!");
    }
}