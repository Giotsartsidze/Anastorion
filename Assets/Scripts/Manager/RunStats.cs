using UnityEngine;

public class RunStats : MonoBehaviour
{
    public static RunStats Instance;

    public int killCount = 0;
    private int currentLevel = 1;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterKill() => killCount++;
    public void SetLevel(int level) => currentLevel = level;
    public int GetLevel() => currentLevel;

    public float GetTime() => DifficultyManager.Instance != null ? DifficultyManager.Instance.gameTime : 0f;

    public string GetFormattedTime()
    {
        float time = GetTime();
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
