using UnityEngine;

public class OrbitingWisps : MonoBehaviour
{
    public GameObject wispPrefab;
    public int count = 3;
    public float orbitSpeed = 150f;
    public float radius = 3f;

    // ესენი აკლდა შენს კოდს, რაც ერორს იწვევდა:
    private GameObject[] activeWisps; 
    private float angle;

    public void ActivateSkill()
    {
        this.enabled = true;

        // ძველი ორბების გასუფთავება
        if (activeWisps != null)
        {
            foreach (var wisp in activeWisps) if (wisp != null) Destroy(wisp);
        }

        activeWisps = new GameObject[count];
        
        for (int i = 0; i < count; i++)
        {
            activeWisps[i] = Instantiate(wispPrefab, transform.position, Quaternion.identity);
            // ვამცირებთ ზომას, რომ მოთამაშე არ დაფარონ
            activeWisps[i].transform.localScale = Vector3.one * 0.5f;
        }
    }

    void Update()
    {
        if (activeWisps == null || activeWisps.Length == 0) return;

        angle += orbitSpeed * Time.deltaTime;
        
        // წრეზე თანაბარი გადანაწილების ფორმულა
        float angleStep = 360f / activeWisps.Length;

        for (int i = 0; i < activeWisps.Length; i++)
        {
            if (activeWisps[i] == null) continue;

            float currentAngle = angle + (i * angleStep);
            
            // ტრიგონომეტრიული გამოთვლა:
            float x = transform.position.x + Mathf.Cos(currentAngle * Mathf.Deg2Rad) * radius;
            float y = transform.position.y + Mathf.Sin(currentAngle * Mathf.Deg2Rad) * radius;
            
            activeWisps[i].transform.position = new Vector3(x, y, 0);
        }
    }
}