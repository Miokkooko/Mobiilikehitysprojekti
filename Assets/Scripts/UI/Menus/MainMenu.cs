using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public CharacterSelectMenu charMenu;
    public PerkMenu perkMenu;

    public List<PerkItemUI> selectedPerks = new List<PerkItemUI>();
    private void Awake()
    {
        QualitySettings.vSyncCount = 1;
    }

    private void OnEnable()
    {
        if (DataManager.Instance != null)
            charMenu.SelectCharacter(DataManager.Instance.CharacterData);
    }
    private void Start()
    {
        SetSelectedPerks();
    }

    public void StartGame()
    {
        DataManager.Instance.SelectPlayerData(charMenu.selectedData);

        SceneManager.LoadScene(2);
    }

    public void SetSelectedPerks()
    {
        for (int i = 0; i < selectedPerks.Count; i++)
        {
            selectedPerks[i].Initialize(perkMenu.selectedTabPerks[i].GetData);
        }
    }
}
