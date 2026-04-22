using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public CharacterSelectMenu charMenu;
    public PerkMenu perkMenu;

    public List<PerkItemUI> selectedPerks = new List<PerkItemUI>();

    bool skipFirstReload = true;

    private void Awake()
    {
        Application.targetFrameRate = 120;
        QualitySettings.vSyncCount = 1;
        SaveManager.OnSaveLoaded += ReloadMainMenu;
    }
    private void OnDestroy()
    {
        SaveManager.OnSaveLoaded -= ReloadMainMenu;
    }

    private void OnEnable()
    {
        if (DataManager.Instance != null)
            charMenu.SelectCharacter(DataManager.Instance.CharacterData);

        ReloadMainMenu();
    }

    void ReloadMainMenu()
    {
        if(skipFirstReload)
        {
            skipFirstReload = false;
            return;
        }
        charMenu.Reload();
        //perkMenu.Reload();
    }

    private void Start()
    {
        SetSelectedPerks();
    }

    public void StartGame()
    {
        if(charMenu.selectedData == null)
        {
            Debug.Log("Invalid Character Selected!");
            return;
        }

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
