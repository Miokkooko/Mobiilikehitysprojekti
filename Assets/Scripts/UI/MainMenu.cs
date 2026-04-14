using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public CharacterSelectMenu charMenu;

    public void StartGame()
    {
        PlayerDataManager.Instance.SelectPlayerData(charMenu.selectedData);
        SceneManager.LoadScene(1);
    }
}
