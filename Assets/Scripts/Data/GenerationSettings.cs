using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

[Serializable]
[CreateAssetMenu(menuName = "Generation Settings")]
public class GenerationSettings : ScriptableObject
{
    //All types data
    [SerializeField] public int selectedType;
    [SerializeField] public PrefabsSet[] defaultRoomPrefabsSets;
    [SerializeField] public PrefabsSet corridorsPrefabsSet;

    [SerializeField] public CustomRoomPrefabsSet[] customRoomPrefabsSets;

    [SerializeField] public Vector3 center;
    [SerializeField] public bool straightPath;
    [SerializeField] public float cellSize;
    [SerializeField] public int seed;

    [SerializeField] public int minRoomsAmount;
    [SerializeField] public int maxRoomsAmount;

    [SerializeField] public Vector2Int minimumRandomRoomSize;
    [SerializeField] public Vector2Int maximumRandomRoomSize;


    //Main path type data
    [SerializeField] public int minSideRoomsAmount;
    [SerializeField] public int maxSideRoomsAmount;

    [SerializeField] public CustomRoomPrefabsSet[] customSideRoomPrefabsSets;

    //Hub type data
    [SerializeField] public int hubConnections;

}



