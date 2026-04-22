using System;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerkItemUI : MonoBehaviour
{
    PerkData data;
    public PerkData GetData => data;

    public Image bg;
    public Sprite lockSprite;
    public Button perkButton;

    public Image perkIcon;

    Color originalColor;
    public Color selectedColor;

    public Image selectionBG;
    public TMP_Text selectionNumber;
    bool isSelected;
    public bool Selected => isSelected;

    public Action<PerkItemUI> OnPerkClicked;

    private void Awake()
    {
        if (originalColor == default)
            originalColor = bg.color;
    }

    public void Initialize(PerkData effect, bool initialState = false)
    {
        selectionBG.color = selectedColor;
        if (!initialState) SetSelectionNumberText("");

        data = effect;
        isSelected = initialState;

        HandleUIState();

        if (data == null)
        {
            Debug.Log("No PerkData found!");
            perkIcon.gameObject.SetActive(false);
            return;
        }

        perkIcon.sprite = data.GetIcon();
        perkIcon.gameObject.SetActive(true);
    }

    public void ClearData()
    {
        data = null;
        isSelected = false;
        perkIcon.gameObject.SetActive(false);
        HandleUIState();
    }

    void HandleUIState()
    {
        if (isSelected)
        {
            selectionBG.gameObject.SetActive(true);
            bg.color = selectedColor;
        }
        else
        {
            selectionBG.gameObject.SetActive(false);
            bg.color = originalColor;
        } 
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        HandleUIState();
    }

    public void SetOwned(bool owned)
    {
        perkButton.interactable = owned;
        perkIcon.sprite = owned ? GetData.Icon : lockSprite;
    }

    public void OnSelected()
    {
        OnPerkClicked?.Invoke(this);
    }

    public void SetSelectionNumberText(string text)
    {
        selectionNumber.SetText(text);
    }
}