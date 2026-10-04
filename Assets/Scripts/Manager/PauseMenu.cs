using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Esc toggles pause in the gameplay scene. Shows a pause panel (and can open the
/// shared settings panel). Won't hijack the level-up / game-over pauses — it only
/// pauses when the game is actually running.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public string menuSceneName = "MainMenu";

    public bool IsPaused { get; private set; }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused) Resume();
            else if (Time.timeScale != 0f) Pause(); // don't steal another menu's pause
        }
    }

    public void Pause()
    {
        IsPaused = true;
        if (pausePanel) pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        IsPaused = false;
        if (settingsPanel) settingsPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OpenSettings()
    {
        if (settingsPanel) settingsPanel.SetActive(true);
        if (pausePanel) pausePanel.SetActive(false); // hide pause behind settings
    }

    public void CloseSettings()
    {
        if (settingsPanel) settingsPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(true); // return to pause menu
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}
