using UnityEngine;
using System;

[Serializable]
public class PrefabsSet
{
    [SerializeField] public WrappedObject[] floorTiles;
    [SerializeField] public WrappedObject[] wallTiles;
    [SerializeField] public WrappedObject[] cornerWallTiles;
    [SerializeField] public WrappedObject[] parallelWallTiles;
    [SerializeField] public WrappedObject[] tripleWallTiles;
}

[Serializable]
public class CustomRoomPrefabsSet : PrefabsSet
{
    [SerializeField] public bool isStartRoom;// { get; private set; }
    [SerializeField] public bool isEndRoom;// { get; private set; }
    [SerializeField] public string roomName;
    [SerializeField] public float generationChance;
    [SerializeField] public int maxAmount; // generate this room <= then {amount}
}

