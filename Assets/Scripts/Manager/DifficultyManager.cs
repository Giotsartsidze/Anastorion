using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance;

    [Header("Scaling Settings")]
    public float scalingFactor = 0.2f; // 20% ზრდა წუთში
    public float gameTime = 0f;

    void Awake()
    {
        Instance = this; // newest wins — survives scene reload without vanishing
    }

    void Update()
    {
        gameTime += Time.deltaTime;
    }

    // ეს ფუნქცია დაგვიბრუნებს მიმდინარე მულტიპლიკატორს
    public float GetDifficultyMultiplier()
    {
        return 1 + (Mathf.Floor(gameTime / 60f) * scalingFactor);
    }
}