using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseButton; // Drag PauseButton here
    [SerializeField] private GameObject menuPanel;   // Drag BackgroundPanel here
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    private bool isPaused = false;

    void Start()
    {
        // Hide menu, show button
        menuPanel.SetActive(false);

        resumeButton.onClick.AddListener(ResumeGame);
        restartButton.onClick.AddListener(RestartGame);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        pauseButton.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        menuPanel.SetActive(false);
        pauseButton.SetActive(true);
    }

    void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}