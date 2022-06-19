using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelGeneratorHub : LevelGeneratorBase
{
   protected int hubConnections;

    public LevelGeneratorHub(GenerationSettings settings) : base(settings) { }

    private Vector2Int VirualGridRoomsPlacement(Room[] rooms, int minDistanceBetweenRooms, Room hub, bool optimize = true)
    {
        rooms = rooms.OrderBy(r => UnityEngine.Random.Range(0, 100)).ToArray();
        List<Room> placedRooms = new List<Room>();
        Vector2Int gridEdgeOffset = new Vector2Int(0, 0);
        Vector2Int virtualGridSize = new Vector2Int(rooms[0].width, rooms[0].height);
        int roomsXMin = int.MaxValue;
        int roomsYMin = int.MaxValue;
        int idx = 0;
        const int ATTEMPTS = 60;
        int tryCounter = ATTEMPTS;
        while (idx < rooms.Length)
        {
            Vector2Int roomSize = new Vector2Int(rooms[idx].width, rooms[idx].height);
            Vector2Int placeingPoint = Utils.RandomVector2Int(gridEdgeOffset, virtualGridSize - gridEdgeOffset);
            if (CheckRoomCollision(placeingPoint, rooms[idx], placedRooms, minDistanceBetweenRooms))
            {
                if (--tryCounter < 0)
                {
                    tryCounter = ATTEMPTS;
                    virtualGridSize += roomSize / 3;
                }
                continue;
            }
            else
            {
                rooms[idx].gridCoordinates = placeingPoint;
                if (placeingPoint.x < roomsXMin)
                    roomsXMin = placeingPoint.x;
                if (placeingPoint.y < roomsYMin)
                    roomsYMin = placeingPoint.y;

                placedRooms.Add(rooms[idx]);
                idx++;
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

        Vector2Int gridSize = new Vector2Int(xmax, ymax) + gridEdgeOffset;
        Vector2Int hubPlaceingPoint = gridSize / 2;
        hub.gridCoordinates = hubPlaceingPoint;
        placedRooms.Add(hub);
        List<Room> roomsForReplacing = new List<Room>();
        for (int i = 0; i < rooms.Length; i++)
        {
            if (CheckRoomCollision(rooms[i].gridCoordinates, rooms[i], new List<Room>() { hub }, minDistanceBetweenRooms))
                roomsForReplacing.Add(rooms[i]);
        }

        
        idx = 0;
        while (idx < roomsForReplacing.Count)
        {
            Vector2Int roomSize = new Vector2Int(roomsForReplacing[idx].width, roomsForReplacing[idx].height);
            Vector2Int placeingPoint = Utils.RandomVector2Int(gridEdgeOffset, virtualGridSize - gridEdgeOffset);
            if (CheckRoomCollision(placeingPoint, roomsForReplacing[idx], placedRooms.Concat(new List<Room>() { hub }).ToList(), minDistanceBetweenRooms))
            {
                if (--tryCounter < 0)
                {
                    tryCounter = ATTEMPTS;
                    virtualGridSize += roomSize / 2;
                }
                continue;
            }
            else
            {
                roomsForReplacing[idx].gridCoordinates = placeingPoint;
                if (placeingPoint.x < roomsXMin)
                    roomsXMin = placeingPoint.x;
                if (placeingPoint.y < roomsYMin)
                    roomsYMin = placeingPoint.y;

                idx++;
            }
        }

        xmax = 0;
        ymax = 0;
        for (int i = 0; i < placedRooms.Count; i++)
        {
            placedRooms[i].gridCoordinates -= (new Vector2Int(roomsXMin, roomsYMin) - gridEdgeOffset);
            if (placedRooms[i].gridCoordinates.x + placedRooms[i].width > xmax)
                xmax = placedRooms[i].gridCoordinates.x + placedRooms[i].width;
            if (placedRooms[i].gridCoordinates.y + placedRooms[i].height > ymax)
                ymax = placedRooms[i].gridCoordinates.y + placedRooms[i].height;
        }

        return new Vector2Int(xmax, ymax) + gridEdgeOffset;
     
    }

    protected override void GetSettings()
    {
        base.GetSettings();
        hubConnections = generationSettings.hubConnections;
    }

    protected override void Generate()
    {
        int roomsNumber = Random.Range(minRoomsAmount, maxRoomsAmount + 1);

        Room[] roomsPool = RoomsGenerator.GenerateRoomsPool(customRoomPrefabsSets, minimumRandomRoomSize, maximumRandomRoomSize,
            roomsNumber, out List<Room> possibleStartRoom, out List<Room> possibleEndRoom);
        Room hub = Utils.RandomChoise(possibleStartRoom);
        roomsPool = roomsPool.Where(x => x != hub).ToArray();

        Vector2Int gridSize = VirualGridRoomsPlacement(roomsPool, 3, hub, false);
        Vector3 upperLeftCorner = new Vector3(gridSize.x * -2, 0, gridSize.y * 2);
        Vector3 offset = center;
        gridSize = new Vector2Int(Mathf.Max(gridSize.x, gridSize.y), Mathf.Max(gridSize.x, gridSize.y));
        grid = new LevelGrid(upperLeftCorner + offset, gridSize, cellSize);
        Room[] allRooms = roomsPool.Concat(new Room[] { hub }).ToArray();
        RoomsGenerator.PrepareRoomsData(allRooms, defaultRoomPrefabsSets, customRoomPrefabsSets);
        RoomsGenerator.SetUpPrefabsGlobal(grid, allRooms);
        
        CorridorsGenerator.GenerateHubConnection(grid, roomsPool, hub, corridorsPrefabsSet, hubConnections, straightPath);

        RoomsGenerator.PrepareRoomsData(allRooms, defaultRoomPrefabsSets, customRoomPrefabsSets);
        RoomsGenerator.SetUpPrefabsGlobal(grid, allRooms);

        for (int i = 0; i < allRooms.Length; i++)
            InstantiateRoomAt(allRooms[i].gridCoordinates, allRooms[i], rooms.transform, i);

        foreach (Corridor corridor in grid.corridors)
            InstantiateCorridor(corridor, corridors.transform);
    }
}
