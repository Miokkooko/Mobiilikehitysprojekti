using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CharacterSelection
{
    public CharacterType CharacterType;
    public PlayerData[] tiers;

    public PlayerData GetData => tiers[0];
    public PlayerData GetDataByTier(int copies)
    {
        return tiers[RankManager.GetRank(copies) - 1];
    }
}
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
    public CharacterSelection[] characters;

    private void OnEnable()
    {
        PlayerData[] temp = new PlayerData[characters.Length];
        for (int i = 0; i < characters.Length; i++) 
        {
            temp[i] = characters[i].GetData;
        }

        characterData = CarouselArray<PlayerData>.FromArray(temp);
        characterData.Next();
        characterEntries = SaveManager.GetCharacterEntries();
        SetCharacters();
    }

    public void Reload()
    {
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

        if(copies > 0)
        {
            foreach(CharacterSelection entry in characters)
            {
                if(entry.CharacterType == characterData.Current.characterType)
                {
                    selection = entry.GetDataByTier(copies);
                }    
            }
        }
        else
            selection = null;
        
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