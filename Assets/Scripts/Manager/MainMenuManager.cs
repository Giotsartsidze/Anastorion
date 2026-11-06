using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject shopPanel;
    public GameObject modifierPanel;

    [Header("Intro cutscene (optional)")]
    public StorybookCutscene introCutscene;

    void Start()
    {
        Time.timeScale = 1f; // ← მნიშვნელოვანია!
        if (mainPanel != null) mainPanel.SetActive(true);
        if (shopPanel != null) shopPanel.SetActive(false);
        if (modifierPanel != null) modifierPanel.SetActive(false);

        UiSkin.Frame(modifierPanel, Vector2.zero); // shared gold frame (auto-fit to content)
    }

    public void PlayGame()
    {
        if (introCutscene != null)
            introCutscene.Play(() => SceneManager.LoadScene("SampleScene"));
        else
            SceneManager.LoadScene("SampleScene");
    }

    public void OpenShop()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(true);
        FindObjectOfType<ShopManager>()?.RefreshAllItems();
    }

    public void CloseShop()
    {
        if (shopPanel != null) shopPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true);
    }

    public void OpenModifiers()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (modifierPanel != null) modifierPanel.SetActive(true);
    }

    public void CloseModifiers()
    {
        if (modifierPanel != null) modifierPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}