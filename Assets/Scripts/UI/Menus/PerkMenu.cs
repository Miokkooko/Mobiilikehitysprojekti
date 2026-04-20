using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;


public class PerkMenu : MonoBehaviour
{
    [Header("Perk Info")]
    public TMP_Text lastPressedPerkName;
    public TMP_Text lastPressedPerkDescription;
    public Image lastPressedPerkIcon;

    [Header("Perk LayOutGroup")]
    public Transform perkPrefab;
    public Transform perkListParent;
    public StatusEffect placeholderData;

    StatusEffect lastPressedPerk;

    public List<PerkItemUI> selectedTabPerks = new List<PerkItemUI>();
    List<PerkItemUI> selectedPerks = new List<PerkItemUI>();
    List<PerkItemUI> perkButtons = new List<PerkItemUI>();

    private void Start()
    {
        Bastor.Helpers.KillChildren(perkListParent);

        SpawnPerkButtons();
    }
    void OnDestroy()
    {
        foreach (var ui in perkButtons)
        {
            if (ui != null)
                ui.OnPerkClicked -= OnPerkButtonClicked;
        }
    }

    void SpawnPerkButtons()
    {
        Transform t;
        for (int i = 0; i < 12; i++)
        {
            t = Instantiate(perkPrefab, perkListParent);

            if(t.TryGetComponent(out PerkItemUI ui))
            {
                ui.Initialize(placeholderData);
                perkButtons.Add(ui);

                ui.OnPerkClicked += OnPerkButtonClicked;
            }
        }
    }


    void OnPerkButtonClicked(PerkItemUI clickedButton)
    {
        lastPressedPerk = clickedButton.GetData;
        HandleLastPressedPerkUI();

        if (clickedButton.Selected)
        {
            DeselectPerk(clickedButton);
            return;
        }

        if (selectedPerks.Count >= 3)
        {
            Debug.Log("Perk slots are full!");
            return;
        }

        selectedPerks.Add(clickedButton);
        clickedButton.SetSelected(true);

        HandleSelectedNumbering();
    }

    void DeselectPerk(PerkItemUI perk)
    {
        perk.SetSelected(false);
        selectedPerks.Remove(perk);

        foreach (var ui in selectedTabPerks)
        {
            ui.ClearData();
        }

        HandleSelectedNumbering();
    }

    void HandleSelectedNumbering()
    {
        for (int i = 0; i < selectedPerks.Count; i++)
        {
            selectedPerks[i].SetSelectionNumberText((i+1).ToString());

            selectedTabPerks[i].Initialize(selectedPerks[i].GetData, true);
            selectedTabPerks[i].SetSelectionNumberText((i + 1).ToString());
        }
    }

    void HandleLastPressedPerkUI()
    {
        lastPressedPerkDescription.SetText(lastPressedPerk.Description);
        lastPressedPerkName.SetText(lastPressedPerk.Name);
        lastPressedPerkIcon.sprite = lastPressedPerk.Icon;
        lastPressedPerkIcon.gameObject.SetActive(true);
    }
}
