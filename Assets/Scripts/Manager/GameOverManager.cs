using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("UI")]
    public GameObject panel;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI killsText;
    public TextMeshProUGUI levelText;

    [Header("Shop")] // ← ახალი
    public GameObject shopPanel;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        if (panel != null) panel.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(false); // ← ახალი
    }

    public void ShowGameOver()
    {
        Time.timeScale = 0f;
        if (panel != null) panel.SetActive(true);

        if (RunStats.Instance != null)
        {
            if (timeText != null)  timeText.text  = "Time Survived: " + RunStats.Instance.GetFormattedTime();
            if (killsText != null) killsText.text = "Enemies Defeated: " + RunStats.Instance.killCount;
            if (levelText != null) levelText.text = "Level Reached: " + RunStats.Instance.GetLevel();
        }
    }

    // ← ახალი: "Shop" ღილაკს მიაბი
    public void OpenShop()
    {
        if (panel != null) panel.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(true);
        FindObjectOfType<ShopManager>()?.RefreshAllItems();
    }

   public void Restart()
{
    Time.timeScale = 1f;
    if (shopPanel != null) shopPanel.SetActive(false);
    // ← MainMenu-ზე გადავდივართ, არა restart
    SceneManager.LoadScene("MainMenu");
}
    // GameOverManager.cs-ში დაამატე
public void CloseShop()
{
    if (shopPanel != null) shopPanel.SetActive(false);
    if (panel != null) panel.SetActive(true);
}
}