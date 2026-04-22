using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Mono.Cecil;


public class PerkMenu : MonoBehaviour
{
    [Header("Perk Info")]
    public TMP_Text lastPressedPerkName;
    public TMP_Text lastPressedPerkDescription;
    public Image lastPressedPerkIcon;

    [Header("Perk LayOutGroup")]
    public Transform perkPrefab;
    public Transform perkListParent;
    public PerkData placeholderData;

    PerkData lastPressedPerk;
    PerkData[] perkDatas;

    public List<PerkItemUI> selectedTabPerks = new List<PerkItemUI>();
    List<PerkItemUI> selectedPerks = new List<PerkItemUI>();
    List<PerkItemUI> perkButtons = new List<PerkItemUI>();

    public PerkData[] GetSelectedPerkDatas()
    {
        PerkData[] array = new PerkData[selectedPerks.Count];

        for (int i = 0; i < array.Length; i++)
        {
            array[i] = selectedPerks[i].GetData;
        }

        return array;
    }

    private void Start()
    {
        Bastor.Helpers.KillChildren(perkListParent);
        perkDatas = Resources.LoadAll<PerkData>("StatusEffects/Perks");
        SpawnPerkButtons();
        Reload();
    }

    public void Reload()
    {
        SetPerkOwnershipStatus();
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
        Bastor.Helpers.KillChildren(perkListParent);
        perkButtons.Clear();

        foreach (var pData in perkDatas)
        {
            Transform t = Instantiate(perkPrefab, perkListParent);
            if (t.TryGetComponent(out PerkItemUI ui))
            {
                ui.Initialize(pData); // Varmista, että PerkItemUI.Initialize hyväksyy PerkDatan!
                perkButtons.Add(ui);
                ui.OnPerkClicked += OnPerkButtonClicked;
            }
        }
    }

    void SetPerkOwnershipStatus()
    {
        PerkEntry perk;
        for (int i = 0; i < perkButtons.Count; i++)
        {
            perk = SaveManager.GetPerkEntry(perkButtons[i].GetData.type);
            if(perk.value > 0)
            {
                perkButtons[i].SetOwned(true);
            }
            else
            {
                perkButtons[i].SetOwned(false);
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
        // Jos PerkDatassa ei ole Description-kenttää, käytä nimeä tai lisää Description PerkDataan
        lastPressedPerkDescription.SetText(lastPressedPerk.GetDescription());
        lastPressedPerkName.SetText(lastPressedPerk.GetName() + " - " + RankManager.GetRank(SaveManager.GetPerkEntry(lastPressedPerk.type).value));
        lastPressedPerkIcon.sprite = lastPressedPerk.GetIcon();
        lastPressedPerkIcon.gameObject.SetActive(true);
    }
}
