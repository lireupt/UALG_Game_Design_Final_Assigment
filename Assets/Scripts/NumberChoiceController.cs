using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NumberChoiceController : MonoBehaviour
{
    //public static NumberChoiceController Instance; // Singleton instance
    public NumberChoiceDatabase numberDB;
    public Text nameText;
    public SpriteRenderer artWorkSprite;
    private int selectedOption = 0;
    
    private void Awake()
    {
        //GetSelectedOption(); // Get and store the parsed value
    }

    void Start()
    {
        if (!PlayerPrefs.HasKey("selectedOption"))
        {
            selectedOption = 0;
        }
        else
        {
            Load();
        }

        // Get and store the selected option
        UpdateNumber(selectedOption); 
    }



    // Update is called once per frame
    public void NextOption()
    {
        selectedOption++;

        if (selectedOption >= numberDB.NumberCount())
        {
            selectedOption = 0;
        }

        UpdateNumber(selectedOption);
        Save();
    }

    public void BackOption()
    {
        //Debug.Log("Moving backward");
        selectedOption--;
        if (selectedOption < 0)
        {
            selectedOption = numberDB.NumberCount() - 1;
        }
        UpdateNumber(selectedOption);
    }

    public void UpdateNumber(int selectedOption)
    {
        NumberChoice number = numberDB.GetNumberChoice(selectedOption);
        artWorkSprite.sprite = number.numberSprite;
        //Debug.Log(number);

        // Suponha que 'number' seja uma instância de NumberChoice
        
    }

    private void Load()
    {
        selectedOption = PlayerPrefs.GetInt("selectedOption");
    }

    private void Save()
    {
        PlayerPrefs.SetInt("selectedOption", selectedOption);
    }
}
