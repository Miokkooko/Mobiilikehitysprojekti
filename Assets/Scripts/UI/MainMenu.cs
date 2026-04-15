using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public CharacterSelectMenu charMenu;

    private void OnEnable()
    {
        if (DataManager.Instance != null)
            charMenu.SelectCharacter(DataManager.Instance.CharacterData);
    }

    public void StartGame()
    {
        DataManager.Instance.SelectPlayerData(charMenu.selectedData);

        SceneManager.LoadScene(2);
    }
}
