using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public Player Player;

    public BarUIElement ExperienceBar;
    public TMP_Text LevelText;

    public BarUIElement HealthBar;
    public TMP_Text HealthText;

    private void Start()
    {
        if(Player == null)
        {
            Debug.LogError("Player not assigned! HUD will not be initialized.");
            return;
        }

        SetPlayerLevelUI();

        Player.OnPlayerHealthChanged += OnPlayerHealthChanged;
        Player.OnPlayerExpChanged += OnPlayerExpChanged;
        Player.OnPlayerLevelUp += OnPlayerLevelUp;
        
    }

    private void OnPlayerLevelUp(int obj)
    {
        SetPlayerLevelUI();   
    }

    private void OnPlayerExpChanged(float obj)
    {
        if(ExperienceBar != null)
            ExperienceBar.SetCurrentValue(obj);
    }

    private void OnDisable()
    {
        Player.OnPlayerHealthChanged -= OnPlayerHealthChanged;
        
    }

    void SetPlayerLevelUI()
    {
        if (ExperienceBar == null)
            return;

        ExperienceBar.SetMinValue(Player.PreviousRequiredExp);
        ExperienceBar.SetMaxValue(Player.RequiredExp);
        ExperienceBar.SetCurrentValue(Player.CurrentExp);

        if (LevelText != null)
            LevelText.text = "Level - " + Player.CurrentLevel;
    }

    private void OnPlayerHealthChanged(float obj)
    {
        if (HealthBar == null)
            return;
        
        HealthBar.SetCurrentValue(obj);
    }
}
