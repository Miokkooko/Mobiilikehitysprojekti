using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsHandler : MonoBehaviour
{
    public string language = "english";
    public string difficulty = "easy";
    public string playerName = "";

    // [SerializeField]
    // Text textBox;

    // string valueText = "";

    // public void Test()
    // {
    //     Debug.Log("DROP DOWN CHANGED");
    //     Dropdown m_Dropdown;
    //     m_Dropdown = GetComponent();
    //     Debug.Log(m_Dropdown.options[m_Dropdown.value].text);


    //     //store in variable
    //     valueText = m_Dropdown.options[m_Dropdown.value].text;
    //     //set textbox value
    //     textBox.text = valueText;
    // }

    public void ChangeLanguage(int index) 
    {
        switch (index)
        {
            case 0: language="english"; break;
            case 1: language="suomi"; break;
        }
        Debug.Log(language);
    }

    public void ChangeDifficulty(int index)
    {
        switch (index)
        {
            case 0: difficulty = "easy"; break;
            case 1: difficulty = "medium"; break;
            case 2: difficulty = "hard"; break;
        } 
        Debug.Log(difficulty);
    }

    public void ChangeName(string newName)
    {
        playerName = newName;
        Debug.Log(playerName);
    }
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
