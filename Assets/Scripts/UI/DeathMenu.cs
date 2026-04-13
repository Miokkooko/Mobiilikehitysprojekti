using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class DeathMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    void Start()
    {
        deathPanel.SetActive(false);

        restartButton.onClick.AddListener(RestartGame);
        mainMenuButton.onClick.AddListener(GoToMainMenu);

        GameManager.Instance.player.OnDeath += OnPlayerDeath;
    }

    private void OnPlayerDeath(object sender, KillContext e)
    {
        ShowDeathMenu();
    }

    public void ShowDeathMenu()
    {
        Time.timeScale = 0f;

        // Get stats from GameManager
        float time = GameManager.Instance.GameTime;
        int kills = GameManager.Instance.Kills;
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        statsText.text = $"GAME OVER\n\n" +
                        $"Time: {minutes:00}:{seconds:00}\n" +
                        $"Kills: {kills}";

        deathPanel.SetActive(true);
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
}