using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public CharacterSelectMenu charMenu;

    private void OnEnable()
    {
        if (PlayerDataManager.Instance != null)
            charMenu.SelectCharacter(PlayerDataManager.Instance.CharacterData);
    }

    public void StartGame()
    {
        PlayerDataManager.Instance.SelectPlayerData(charMenu.selectedData);

        SceneManager.LoadScene(1);
    }
}
