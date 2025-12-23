using UnityEngine;

public class OrbitingWisps : MonoBehaviour
{
    public GameObject wispPrefab;
    public int wispCount = 2;       // რამდენი ბურთულა
    public float orbitSpeed = 100f; // ტრიალის სისწრაფე
    public float orbitRadius = 2.5f; // მანძილი მოთამაშისგან

    private GameObject[] wisps;
    private float currentAngle = 0f;

    // ეს ფუნქცია გამოიძახება ლუტის აღებისას
    public void ActivateSkill()
    {
        wisps = new GameObject[wispCount];
        float angleStep = 360f / wispCount;

        for (int i = 0; i < wispCount; i++)
        {
            wisps[i] = Instantiate(wispPrefab, transform.position, Quaternion.identity);
            // სილამაზისთვის ცოტა დავაპატარავოთ
            wisps[i].transform.localScale = Vector3.one * 0.5f;
        }
        
        enabled = true; // ჩავრთოთ Update
        Debug.Log("Orbiting Wisps Activated!");
    }

    void Update()
    {
        if (wisps == null) return;

        currentAngle += orbitSpeed * Time.deltaTime;
        float angleStep = 360f / wispCount;

        for (int i = 0; i < wispCount; i++)
        {
            if (wisps[i] == null) continue;

            float angle = currentAngle + (i * angleStep);
            // მათემატიკა წრეზე ტრიალისთვის
            float x = transform.position.x + Mathf.Cos(angle * Mathf.Deg2Rad) * orbitRadius;
            float y = transform.position.y + Mathf.Sin(angle * Mathf.Deg2Rad) * orbitRadius;
            
            wisps[i].transform.position = new Vector3(x, y, 0);
        }
    }
}