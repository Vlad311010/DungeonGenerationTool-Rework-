using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class RoomsGenerator
{
    private static void SelectPrefabsSet(PrefabsSet[] sets, int setIdx, out WrappedObject[] floorPrefabs, out WrappedObject[] wallPrefabs,
       out WrappedObject[] cornerWallPrefabs, out WrappedObject[] parallelWallPrefabs, out WrappedObject[] tripleWallPrefabs)
    {
        PrefabsSet set = sets[setIdx];
        floorPrefabs = set.floorTiles;
        wallPrefabs = set.wallTiles;
        cornerWallPrefabs = set.cornerWallTiles;
        parallelWallPrefabs = set.parallelWallTiles;
        tripleWallPrefabs = set.tripleWallTiles;
    }

    private static CustomRoomData[] LoadCustomRoomDataFromPrefabsSet(CustomRoomPrefabsSet[] customRoomPrefabs)
    {
        CustomRoomData[] availableCustomRooms = new CustomRoomData[customRoomPrefabs.Length];
        for (int i = 0; i < customRoomPrefabs.Length; i++)
        {
            availableCustomRooms[i] = CustomRoomData.Load(customRoomPrefabs[i].roomName);
        }

        return availableCustomRooms;
    }

    private static void SetUpPrefabsLocal(Room room, WrappedObject[] floorPrefabs,
        WrappedObject[] wallPrefabs, WrappedObject[] cornerWallPrefabs, WrappedObject[] parallelWallPrefabs, WrappedObject[] tripleWallPrefabs)
    {
        foreach (LocalTile localTile in room.roomTiles)
        {
            if (localTile.tag.type == TileType.floor)
            {
                int selectedPrefabIdx = UnityEngine.Random.Range(0, floorPrefabs.Length);
                localTile.SetPrefab(floorPrefabs[selectedPrefabIdx]);
            }
            else if (localTile.tag.type == TileType.wall)
            {
                int selectedPrefabIdx = UnityEngine.Random.Range(0, wallPrefabs.Length);
                localTile.SetPrefab(wallPrefabs[selectedPrefabIdx]);
            }
            else if (localTile.tag.type == TileType.cornerWall)
            {
                int selectedPrefabIdx = UnityEngine.Random.Range(0, cornerWallPrefabs.Length);
                localTile.SetPrefab(cornerWallPrefabs[selectedPrefabIdx]);
            }
            else if (localTile.tag.type == TileType.parallelWall)
            {
                int selectedPrefabIdx = UnityEngine.Random.Range(0, parallelWallPrefabs.Length);
                localTile.SetPrefab(parallelWallPrefabs[selectedPrefabIdx]);
            }
            else if (localTile.tag.type == TileType.tripleWall)
            {
                int selectedPrefabIdx = UnityEngine.Random.Range(0, tripleWallPrefabs.Length);
                localTile.SetPrefab(tripleWallPrefabs[selectedPrefabIdx]);
            }
            /*else if (localTile.tag.type == TileType.enter)
            {
                //int selectedPrefabIdx = UnityEngine.Random.Range(0, wallPrefabs.Length);
                //localTile.SetPrefab(wallPrefabs[selectedPrefabIdx]);
            }*/
        }
        foreach (EntranceTile entrance in room.entrances)
        {
            if (entrance.connected)
                entrance.SetPrefab(null);
            else
            { 
                int selectedPrefabIdx = UnityEngine.Random.Range(0, wallPrefabs.Length);
                entrance.SetPrefab(wallPrefabs[selectedPrefabIdx]);
            }
        }
    }

    public static Room[] GenerateRoomsPool(CustomRoomPrefabsSet[] customRoomPrefabs, Vector2Int minimumRandomRoomSize,
        Vector2Int maximumRandomRoomSize, int roomsAmount)
    {
        Room[] roomsPool = new Room[roomsAmount];
        List<Room> tempPossibleStartRooms = new List<Room>();
        List<Room> tempPossibleEndRooms = new List<Room>();
        customRoomPrefabs.OrderBy(x => x.generationChance);
        CustomRoomData[] availableCustomRooms = LoadCustomRoomDataFromPrefabsSet(customRoomPrefabs); // order of {customRoomPrefabs} respond to order of {avaiableCustomRooms}
        int roomsPoolIdx = 0;
        for (int i = 0; i < availableCustomRooms.Length; i++)
        {
            if (roomsPoolIdx >= roomsAmount)
                break;

            for (int j = 0; j < customRoomPrefabs[i].maxAmount; j++)
            {
                float randVal = UnityEngine.Random.value;
                if (customRoomPrefabs[i].generationChance > randVal)
                {
                    roomsPool[roomsPoolIdx] = new Room(availableCustomRooms[i]);
                    roomsPoolIdx++;
                }
            }
        }

        for (int i = roomsPoolIdx; i < roomsAmount; i++)
        {
            Vector2Int randomRoomSize = Utils.RandomVector2Int(minimumRandomRoomSize, maximumRandomRoomSize);
            roomsPool[i] = new Room(randomRoomSize.x, randomRoomSize.y);
        }


        return roomsPool;
    }

    public static Room[] GenerateRoomsPool(CustomRoomPrefabsSet[] customRoomPrefabs, Vector2Int minimumRandomRoomSize,
        Vector2Int maximumRandomRoomSize, int roomsAmount, out List<Room> possibleStartRooms, out List<Room> possibleEndRooms)
    {
        Room[] roomsPool = new Room[roomsAmount];
        List<Room> tempPossibleStartRooms = new List<Room>();
        List<Room> tempPossibleEndRooms = new List<Room>();
        customRoomPrefabs.OrderBy(x => x.generationChance);
        CustomRoomData[] availableCustomRooms = LoadCustomRoomDataFromPrefabsSet(customRoomPrefabs); // order of {customRoomPrefabs} respond to order of {avaiableCustomRooms}
        int roomsPoolIdx = 0;
        for (int i = 0; i < availableCustomRooms.Length; i++)
        {
            if (roomsPoolIdx >= roomsAmount)
                break;

            for (int j = 0; j < customRoomPrefabs[i].maxAmount; j++)
            {
                float randVal = UnityEngine.Random.value;
                if (customRoomPrefabs[i].generationChance > randVal)
                {
                    roomsPool[roomsPoolIdx] = new Room(availableCustomRooms[i]);
                    if (customRoomPrefabs[i].isStartRoom)
                        tempPossibleStartRooms.Add(roomsPool[roomsPoolIdx]);
                    else if (customRoomPrefabs[i].isEndRoom)
                        tempPossibleEndRooms.Add(roomsPool[roomsPoolIdx]);
                    roomsPoolIdx++;
                }
            }
        }

        for (int i = roomsPoolIdx; i < roomsAmount; i++)
        {
            Vector2Int randomRoomSize = Utils.RandomVector2Int(minimumRandomRoomSize, maximumRandomRoomSize);
            roomsPool[i] = new Room(randomRoomSize.x, randomRoomSize.y);
        }

        if (tempPossibleStartRooms.Count == 0)
        {
            Room randomRoom = Utils.RandomChoise(roomsPool.Where(x => !tempPossibleEndRooms.Contains(x)).ToArray());
            tempPossibleStartRooms.Add(randomRoom);
        }
        if (tempPossibleEndRooms.Count == 0)
        {
            Room randomRoom = Utils.RandomChoise(roomsPool.Where(x => !tempPossibleStartRooms.Contains(x)).ToArray());
            tempPossibleEndRooms.Add(randomRoom);
        }

        possibleStartRooms = tempPossibleStartRooms;
        possibleEndRooms = tempPossibleEndRooms;
        return roomsPool;
    }


    public static void PrepareRoomsData(Room[] rooms, PrefabsSet[] randomRoomPrefabsSets, CustomRoomPrefabsSet[] customRoomPrefabsSets)
    {
        for (int i = 0; i < rooms.Length; i++)
        {
            WrappedObject[] floorPrefabs;
            WrappedObject[] wallTiles;
            WrappedObject[] cornerWallTiles;
            WrappedObject[] parallelWallTiles;
            WrappedObject[] tripleWallTiles;
            if (rooms[i].customRoom)
            {
                int setIdx = Random.Range(0, customRoomPrefabsSets.Length);
                SelectPrefabsSet(customRoomPrefabsSets, setIdx, out floorPrefabs, out wallTiles, out cornerWallTiles,
                        out parallelWallTiles, out tripleWallTiles);
            }
            else
            {
                int setIdx = Random.Range(0, randomRoomPrefabsSets.Length);
                SelectPrefabsSet(randomRoomPrefabsSets, setIdx, out floorPrefabs, out wallTiles, out cornerWallTiles,
                        out parallelWallTiles, out tripleWallTiles);
            }
            SetUpPrefabsLocal(rooms[i], floorPrefabs, wallTiles, cornerWallTiles, parallelWallTiles, tripleWallTiles);
        }
    }

    public static void SetUpPrefabsGlobal(LevelGrid grid, Room[] rooms) // set up prefabs on 'grid'
    {
        for (int i = 0; i < rooms.Length; i++)
        {
            Vector2Int start = rooms[i].gridCoordinates;
            foreach (LocalTile localTile in rooms[i].roomTiles)
            {
                GridTile gridTile = grid.GetRoomMapTile(start.x + localTile.localCoordinates.x, start.y + localTile.localCoordinates.y);
                gridTile.SetPrefab(localTile.prefab);
                gridTile.SetTag(localTile.tag);
            }
            foreach (EntranceTile entrance in rooms[i].entrances)
            {
                GridTile gridTile = grid.GetRoomMapTile(start.x + entrance.localCoordinates.x, start.y + entrance.localCoordinates.y);
                gridTile.SetPrefab(entrance.prefab);
                gridTile.SetTag(entrance.tag);
            }
        }
    }



}
