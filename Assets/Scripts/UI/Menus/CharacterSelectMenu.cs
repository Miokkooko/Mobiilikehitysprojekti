using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectMenu : MonoBehaviour
{
    CarouselArray<PlayerData> characterData;

    public Image selectedCharSprite;
    public TMP_Text selectedCharName;
    public TMP_Text selectedCharRank;

    public Image previousChar;
    public Image nextChar;

    public PlayerData selectedData => characterData.Current;

    int selectedIndex = 0;

    private void OnEnable()
    {
        characterData = CarouselArray<PlayerData>.FromArray(Resources.LoadAll<PlayerData>("UnitData/Players"));
        SetCharacters();
    }

    void SetCharacters()
    {
        selectedCharSprite.sprite = characterData.Current.baseSprite;

        string[] name = characterData[selectedIndex].unitName.Split(',');
        string unifiedName = name[0] + "\n" + name[1];
        selectedCharName.SetText(unifiedName);
        selectedCharRank.SetText("Rank 1"); // TODO
        
        nextChar.sprite = characterData.PeekNext().baseSprite;
        previousChar.sprite = characterData.PeekPrevious().baseSprite;
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