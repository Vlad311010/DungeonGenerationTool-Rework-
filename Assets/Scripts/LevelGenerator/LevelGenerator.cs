using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] public GenerationSettings generationSettings;

    public void Generate()
    {
        if (generationSettings.selectedType == 0)
            GenerateMainPath();
        else if (generationSettings.selectedType == 1)
            GenerateHub();
    }

    public void GenerateHub()
    {
        new LevelGeneratorHub(generationSettings).RunGenerator();
    }

    public void GenerateMainPath()
    {
        new LevelGeneratorMainPath(generationSettings).RunGenerator();
    }
}
