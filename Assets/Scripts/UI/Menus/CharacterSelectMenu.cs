using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterSelectMenu : MonoBehaviour
{
    PlayerData[] characterData;

    public Image selectedCharSprite;
    public TMP_Text selectedCharName;
    public TMP_Text selectedCharRank;

    public Image previousChar;
    public Image nextChar;

    public PlayerData selectedData => characterData[selectedIndex];

    int selectedIndex = 0;

    private void OnEnable()
    {
        characterData = Resources.LoadAll<PlayerData>("UnitData/Players");
        SetCharacters();
    }

    void SetCharacters()
    {
        selectedCharSprite.sprite = characterData[selectedIndex].baseSprite;

        string[] name = characterData[selectedIndex].unitName.Split(',');
        string unifiedName = name[0] + "\n" + name[1];
        selectedCharName.SetText(unifiedName);

        selectedCharRank.SetText("Rank 1"); // TODO

        int index = selectedIndex + 1;

        if (index >= characterData.Length)
            index = 0;
        
        nextChar.sprite = characterData[index].baseSprite;

        index = selectedIndex - 1;

        if (index < 0)
            index = characterData.Length - 1;

        previousChar.sprite = characterData[index].baseSprite;
    }

    public void MoveRosterRight()
    {
        selectedIndex++;

        if (selectedIndex >= characterData.Length)
            selectedIndex = 0;
    
        SetCharacters();
    }
    public void MoveRosterLeft()
    {
        selectedIndex--;

        if (selectedIndex < 0)
            selectedIndex = characterData.Length - 1;
    
        SetCharacters();
    }

    public void SelectCharacter(PlayerData data)
    {
        if (data == null)
            return;

        for (int i = 0; i < characterData.Length; i++)
        {
            if (data == characterData[i])
            {
                selectedIndex = i;
                break;
            }
        }

        SetCharacters();
    }
}
