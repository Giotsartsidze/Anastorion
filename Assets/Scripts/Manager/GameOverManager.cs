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

    [Header("Dream outro (optional)")]
    public StorybookCutscene outro;

    void Awake()
    {
        Instance = this; // newest wins — survives scene reload without vanishing
        if (panel != null) panel.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(false); // ← ახალი
        UiSkin.Frame(panel, new Vector2(560, 400)); // shared gold frame
    }

    public void ShowGameOver()
    {
        Time.timeScale = 0f;

        // Dream framing: death = waking up. Play the outro (shards folded in), then
        // return to the bedroom menu. Falls back to the results panel if no outro is set.
        if (outro != null)
        {
            outro.Play(() => { Time.timeScale = 1f; SceneManager.LoadScene("MainMenu"); });
            return;
        }

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