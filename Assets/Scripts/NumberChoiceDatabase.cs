using System.Collections;
using UnityEngine;

[CreateAssetMenu]
public class NumberChoiceDatabase : ScriptableObject
{

    public NumberChoice[] numberChoices;

    public int NumberCount()
    {
        return numberChoices.Length;
    }

    public NumberChoice GetNumberChoice(int index)
    {
        return numberChoices[index];
    }
}
