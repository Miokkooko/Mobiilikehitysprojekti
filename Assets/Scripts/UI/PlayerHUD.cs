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

    public TMP_Text KillsText;
    public TMP_Text TimerText;
    public TMP_Text CoinText;

    float timer = 0f;

    public InventoryUIHandler inventoryHandler;

    void Start()
    {
        if(Player == null)
        {
            Debug.LogError("Player not assigned! HUD will not be initialized.");
            return;
        }

        KillsText.SetText("0");
        CoinText.SetText("0");
        SetPlayerLevelUI();

        Player.OnPlayerHealthChanged += OnPlayerHealthChanged;
        Player.OnPlayerExpChanged += OnPlayerExpChanged;
        Player.OnPlayerLevelUp += OnPlayerLevelUp;
        Player.OnKill += OnEnemyKilled;
        GameManager.Instance.OnCoinChanged += OnCoinChanged;

        OnPlayerHealthChanged(Player.Health);

        if(inventoryHandler != null)
        {
            inventoryHandler.Initialize(Player.playerData.maxWeapons, Player.playerData.maxPassives);
            Player.OnPlayerGetPassive += OnPlayerGetPassive;
            Player.OnPlayerGetWeapon += OnPlayerGetWeapon;
        }
    }

    private void OnDisable()
    {
        Player.OnPlayerHealthChanged -= OnPlayerHealthChanged;
        Player.OnPlayerGetPassive -= OnPlayerGetPassive;
        Player.OnPlayerGetWeapon -= OnPlayerGetWeapon;
        Player.OnPlayerExpChanged -= OnPlayerExpChanged;
        Player.OnPlayerLevelUp -= OnPlayerLevelUp;
        Player.OnKill -= OnEnemyKilled;
        GameManager.Instance.OnCoinChanged -= OnCoinChanged;
    }

    private void OnPlayerGetWeapon(WeaponData[] obj)
    {
        inventoryHandler.UpdateWeaponList(obj);
    }

    private void OnPlayerGetPassive(PassiveData[] obj)
    {
        inventoryHandler.UpdatePassiveList(obj);
    }

    void Update()
    {
        timer = GameManager.Instance.GameTime;

        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);

        string timeText = string.Format("{0:00}:{1:00}", minutes, seconds);

        TimerText.text = timeText;
    }

    private void OnEnemyKilled(object sender, KillContext e)
    {
        KillsText.text = GameManager.Instance.Kills.ToString();
    }

    private void OnCoinChanged(int obj)
    {
        CoinText.text = GameManager.Instance.Coins.ToString();
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

        obj = (float)System.Math.Round(obj, 1);
        HealthBar.SetCurrentValue(obj);
        HealthBar.SetMaxValue(Player.MaxHealth);
        HealthText.text = obj.ToString() + " / " + Player.MaxHealth;
    }
}
