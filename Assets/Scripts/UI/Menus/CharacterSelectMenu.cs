using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectMenu : MonoBehaviour
{
    CarouselArray<PlayerData> characterData;

    public Image selectedCharSprite;
    public TMP_Text selectedCharName;
    public RankBar selectedCharRank;

    public Image previousChar;
    public Image nextChar;

    public Color disabledColor;
    public Color enabledColor;
    public GameObject selectedCharLock;

    PlayerData selection;
    public PlayerData selectedData => selection;

    List<CharacterEntry> characterEntries;

    private void OnEnable()
    {
        characterData = CarouselArray<PlayerData>.FromArray(Resources.LoadAll<PlayerData>("UnitData/Players"));
        characterData.Next();
        characterEntries = SaveManager.GetCharacterEntries();
        SetCharacters();
    }

    int GetCopiesFor(CharacterType character)
    {
        foreach (CharacterEntry entry in characterEntries)
        {
            if (entry.type == character)
                return entry.value;
        }

        return 0;
    }
    void SetCharacters()
    {
        selectedCharSprite.sprite = characterData.Current.baseSprite;

        int copies = GetCopiesFor(characterData.Current.characterType);

        selectedCharName.SetText(copies > 0 ? characterData.Current.GetName() : "???");
        selectedCharRank.gameObject.SetActive(copies > 0);
        selectedCharSprite.color = copies > 0 ? enabledColor : disabledColor;
        selectedCharRank.Initialize(0, RankManager.GetRankProgress(copies), RankManager.CopiesPerRank, copies);
        selectedCharLock.SetActive(copies <= 0);
        selection = copies > 0 ? characterData.Current : null;
        
        copies = GetCopiesFor(characterData.PeekNext().characterType);

        nextChar.sprite = characterData.PeekNext().baseSprite;
        nextChar.color = copies > 0 ? enabledColor : disabledColor;

        copies = GetCopiesFor(characterData.PeekPrevious().characterType);

        previousChar.sprite = characterData.PeekPrevious().baseSprite;
        previousChar.color = copies > 0 ? enabledColor : disabledColor;
    }

    public void MoveRosterRight()
    {
        characterData.Next();
        SetCharacters();
    }
    public void MoveRosterLeft()
    {
        characterData.Previous();
        SetCharacters();
    }

    public void SelectCharacter(PlayerData data)
    {
        if (data == null)
            return;

        characterData.TrySetCurrent(data);

        SetCharacters();
    }
}