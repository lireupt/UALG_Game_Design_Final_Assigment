using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyIA : MonoBehaviour
{
    public CreateLevel createLevel;  // Atribua isso através do Inspector
    private IEnumerable<Vector2Int> cleanCoordinates;

    private void Start()
    {
        // Encontrar todos os objetos CreateLevel na cena
        CreateLevel[] createLevels = FindObjectsOfType<CreateLevel>();

        foreach (var level in createLevels)
        {
            Debug.Log(level);
        }
    }
}