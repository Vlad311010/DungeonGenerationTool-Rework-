using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelGeneratorMainPath : LevelGeneratorBase
{
    public LevelGeneratorMainPath(GenerationSettings settings) : base(settings) { }

    //Tuple<List<GridTile>, List<GridTile>>
    public List<Vector2Int> RandomWalkRoomPlacing(Vector2Int startPoint, Room[] rooms, out Vector2Int gridSize)
    {
        Vector2Int edgeOffest = new Vector2Int(3, 3);
        Vector2Int[] dir = new Vector2Int[4] { new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0) };

        List<Vector2Int> path = new List<Vector2Int>();
        List<Room> placedRooms = new List<Room>();
        int move = 0;
        Vector2Int currentPoint = startPoint;
        const int placingInterval = 5;
        int counter = placingInterval;

        while (placedRooms.Count < rooms.Length)
        {
            path.Add(currentPoint);
            currentPoint += dir[move];
            if (currentPoint.x <= edgeOffest.x || currentPoint.y <= edgeOffest.y)
            {
                //move = -move;
                move = (move + 2) % 4;
                Debug.Log("1 " + move);
                currentPoint += dir[move]*2;
            }

            if (--counter < 0)
            {
                counter = placingInterval;

                if (!CheckRoomCollision(currentPoint, rooms[placedRooms.Count], placedRooms, 3))
                {
                    rooms[placedRooms.Count].gridCoordinates = currentPoint;
                    placedRooms.Add(rooms[placedRooms.Count]);
                }
            }

            if (UnityEngine.Random.value < 0.4) // change direction
            {
                //move = UnityEngine.Random.Range(0, 4);
                if (UnityEngine.Random.value > 0.5f)
                    move = (move + 1) % 4;
                else
                    move = move == 0 ? 3 : move - 1;
                //move = Utils.RandomChoise(dir);
            }
        }

        int xmax = 0;
        int ymax = 0;
        for (int i = 0; i < placedRooms.Count; i++)
        {
            if (placedRooms[i].gridCoordinates.x + placedRooms[i].width > xmax)
                xmax = placedRooms[i].gridCoordinates.x + placedRooms[i].width;
            if (placedRooms[i].gridCoordinates.y + placedRooms[i].height > ymax)
                ymax = placedRooms[i].gridCoordinates.y + placedRooms[i].height;
        }
        gridSize = new Vector2Int(xmax, ymax) + edgeOffest;

        return path;
    }

    /*protected override Vector2Int VirualGridRoomsPlacement(Room[] rooms, int minDistanceBetweenRooms, bool optimize = true)
    {
        return base.VirualGridRoomsPlacement(rooms, minDistanceBetweenRooms, optimize);
    }*/

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
        mainRoomsPool = new List<Room>() { startRoom }.Concat(mainRoomsPool.Where(x => x != startRoom && x != endRoom)).Concat(new List<Room>() { endRoom }).ToArray();

        
        Room[] sideRoomsPool = RoomsGenerator.GenerateRoomsPool(customSideRoomPrefabsSets, minimumRandomRoomSize, maximumRandomRoomSize, sideRoomsNumber);

        Room[] allRooms = mainRoomsPool.Concat(sideRoomsPool).ToArray();


        Vector2Int gridSize;
        RandomWalkRoomPlacing(Utils.RandomVector2Int(new Vector2Int(5, 5), new Vector2Int(30, 30)), mainRoomsPool, out gridSize);
        gridSize = VirualGridRoomsPlacement(sideRoomsPool, mainRoomsPool, gridSize, 3, false);

        Vector3 upperLeftCorner = new Vector3(gridSize.x * -2, 0, gridSize.y * 2);
        Vector3 offset = center;
        gridSize = new Vector2Int(Mathf.Max(gridSize.x, gridSize.y), Mathf.Max(gridSize.x, gridSize.y));
        grid = new LevelGrid(upperLeftCorner + offset, gridSize, cellSize);


        RoomsGenerator.PrepareRoomsData(mainRoomsPool, defaultRoomPrefabsSets, customRoomPrefabsSets);
        RoomsGenerator.PrepareRoomsData(sideRoomsPool, defaultRoomPrefabsSets, customSideRoomPrefabsSets);
        RoomsGenerator.SetUpPrefabsGlobal(grid, allRooms);
        //RoomsGenerator.SetUpPrefabsGlobal(grid, mainRoomsPool);

        CorridorsGenerator.GenerateMainPathConnection(grid, mainRoomsPool, startRoom, endRoom, allRooms, corridorsPrefabsSet, straightPath);
        //CorridorsGenerator.GenerateMainPathConnection(grid, mainRoomsPool, startRoom, endRoom, mainRoomsPool, corridorsPrefabsSet, straightPath);

        RoomsGenerator.PrepareRoomsData(mainRoomsPool, defaultRoomPrefabsSets, customRoomPrefabsSets);
        RoomsGenerator.PrepareRoomsData(sideRoomsPool, defaultRoomPrefabsSets, customSideRoomPrefabsSets);
        RoomsGenerator.SetUpPrefabsGlobal(grid, allRooms);


        for (int i = 0; i < allRooms.Length; i++)
            InstantiateRoomAt(allRooms[i].gridCoordinates, allRooms[i], rooms.transform, i);

        foreach (Corridor corridor in grid.corridors)
            InstantiateCorridor(corridor, corridors.transform);

        /*Vector2Int gridSize = VirualGridRoomsPlacementDefault(allRooms, 2);
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
            InstantiateCorridor(corridor, corridors.transform);*/

    }
}
