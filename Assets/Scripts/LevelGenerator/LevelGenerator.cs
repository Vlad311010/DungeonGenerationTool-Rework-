using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] public GenerationSettings generationSettings;
    
    private LevelGrid grid;
    private GameObject generatedLevel;
    private GeneratedLevel generatedLevelComponent;

    public void Generate()
    {
        throw new NotImplementedException();
    }

    public void GenerateHub()
    {
        new LevelGeneratorHub(generationSettings).RunGenerator();
    }

    public void GenerateMainPath()
    {
        new LevelGeneratorMainPath(generationSettings).RunGenerator();
    }

    /*public void ShowHighLightedTileInfo()
    {
        if (grid.ValidCoordinates(highLight))
        {
            GridTile selectedTile = grid.GetRoomMapTile(highLight);
            VisualDebug.DrawSquareWithMark(selectedTile.worldCoordinates + new Vector3(0, 2, 0), grid.cellSize, Color.green);
            Debug.Log(selectedTile.tag.ToString());
        }
    }*/
}
