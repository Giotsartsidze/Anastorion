using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject shopPanel;

    void Start()
    {
        Time.timeScale = 1f; // ← მნიშვნელოვანია!
        if (mainPanel != null) mainPanel.SetActive(true);
        if (shopPanel != null) shopPanel.SetActive(false);
    }

    public void PlayGame()
    {
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

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit!"); // Editor-ში ეს ჩანს
    }
}