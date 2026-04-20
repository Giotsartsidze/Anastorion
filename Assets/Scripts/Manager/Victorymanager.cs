using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    public static VictoryManager Instance;

    [Header("UI")]
    public GameObject panel;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI killsText;
    public TextMeshProUGUI levelText;

    [Header("Win Condition")]
    public float surviveMinutes = 10f;

    private bool victoryShown = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        if (panel != null) panel.SetActive(false);
    }

    void Update()
    {
        if (victoryShown || DifficultyManager.Instance == null) return;
        if (DifficultyManager.Instance.gameTime >= surviveMinutes * 60f)
            ShowVictory();
    }

    public void ShowVictory()
    {
        if (victoryShown) return;
        victoryShown = true;

        Time.timeScale = 0f;
        if (panel != null) panel.SetActive(true);

        if (RunStats.Instance != null)
        {
            if (timeText != null)  timeText.text  = "Time Survived: " + RunStats.Instance.GetFormattedTime();
            if (killsText != null) killsText.text = "Enemies Defeated: " + RunStats.Instance.killCount;
            if (levelText != null) levelText.text = "Level Reached: " + RunStats.Instance.GetLevel();
        }
    }

    // Wire this to your Restart button in the Inspector
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
