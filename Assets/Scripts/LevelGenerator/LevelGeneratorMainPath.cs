using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelGeneratorMainPath : LevelGeneratorBase
{
    public LevelGeneratorMainPath(GenerationSettings settings) : base(settings) { }

    protected override void Generate()
    {


        int roomsNumber = UnityEngine.Random.Range(minRoomsAmount, maxRoomsAmount + 1);
        int sideRoomsNumber = UnityEngine.Random.Range(minSideRoomsAmount, maxSideRoomsAmount + 1);

        List<Room> possibleStartRooms;
        List<Room> possibleEndRooms;
        Room[] mainRoomsPool = RoomsGenerator.GenerateRoomsPool(customRoomPrefabsSets, minimumRandomRoomSize, maximumRandomRoomSize, 
            roomsNumber, out possibleStartRooms, out possibleEndRooms);
        Room startRoom = Utils.RandomChoise(possibleStartRooms);
        Room endRoom = Utils.RandomChoise(possibleEndRooms);
        
        Debug.Log(startRoom + " -> " + endRoom);

        /*foreach (var item in mainRoomsPool)
        {
            Debug.Log(item.GetRoomSize() + ":" + item.gridCoordinates);
            if (item == startRoom)
            {
                Debug.Log(item.GetRoomSize() + ":" + item.gridCoordinates + " - " + startRoom.GetRoomSize() + ":" + startRoom.gridCoordinates);
            }
        }*/
        Room[] sideRoomsPool = RoomsGenerator.GenerateRoomsPool(customSideRoomPrefabsSets, minimumRandomRoomSize, maximumRandomRoomSize, sideRoomsNumber);

        /*GameObject rooms = new GameObject("Rooms");
        rooms.transform.parent = generatedLevel.transform;
        GameObject corridors = new GameObject("Corridors");
        corridors.transform.parent = generatedLevel.transform;*/

        Room[] allRooms = mainRoomsPool.Concat(sideRoomsPool).ToArray();
        Vector2Int gridSize = VirualGridRoomsPlacementDefault(allRooms, 2);
        //Vector2Int gridSize = VirualGridRoomsPlacement(allRooms, 2, startRoom, endRoom, 30);
        Vector3 upperLeftCorner = new Vector3(gridSize.x * -2, 0, gridSize.y * 2);
        Vector3 offset = center;
        gridSize = new Vector2Int(Mathf.Max(gridSize.x, gridSize.y), Mathf.Max(gridSize.x, gridSize.y));
        grid = new LevelGrid(upperLeftCorner + offset, gridSize, cellSize);
        Debug.Log("Grid Size: " + grid.gridSize);
        RoomsGenerator.PrepareRoomsData(mainRoomsPool, defaultRoomPrefabsSets, customRoomPrefabsSets);
        RoomsGenerator.PrepareRoomsData(sideRoomsPool, defaultRoomPrefabsSets, customSideRoomPrefabsSets);
        RoomsGenerator.SetUpPrefabsGlobal(grid, allRooms);

        CorridorsGenerator.GenerateMainPathConnection(grid, mainRoomsPool, startRoom, endRoom, allRooms, corridorsPrefabsSet, straightPath);

        RoomsGenerator.PrepareRoomsData(mainRoomsPool, defaultRoomPrefabsSets, customRoomPrefabsSets);
        RoomsGenerator.PrepareRoomsData(sideRoomsPool, defaultRoomPrefabsSets, customSideRoomPrefabsSets);
        RoomsGenerator.SetUpPrefabsGlobal(grid, allRooms);



        for (int i = 0; i < allRooms.Length; i++)
            InstantiateRoomAt(allRooms[i].gridCoordinates, allRooms[i], rooms.transform, i);

        foreach (Corridor corridor in grid.corridors)
            InstantiateCorridor(corridor, corridors.transform);

    }
}
