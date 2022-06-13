using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class LevelGeneratorRemoved : MonoBehaviour
{
    [SerializeField] public GenerationSettings generationSettings;

    [SerializeField] public PrefabsSet[] defaultRoomPrefabsSets;
    [SerializeField] public PrefabsSet corridorsPrefabsSet;

    [SerializeField] public CustomRoomPrefabsSet[] customRoomPrefabsSets;

    //[SerializeField] public Vector2Int size;
    [SerializeField] public Vector3 center;
    [SerializeField] public float cellSize;
    [SerializeField] public int seed;

    [SerializeField] public int minRoomsAmount;
    [SerializeField] public int maxRoomsAmount;

    [SerializeField] public Vector2Int minimumRandomRoomSize;
    [SerializeField] public Vector2Int maximumRandomRoomSize;

    //MainPath
    [SerializeField] public int minSideRoomsAmount;
    [SerializeField] public int maxSideRoomsAmount;
    [SerializeField] public CustomRoomPrefabsSet[] customSideRoomPrefabsSets;

    //pohidni parametry
    private int roomsNumber;



    //debug
    [SerializeField] public Vector2Int highLight = new Vector2Int(0, 0);
    [SerializeField] public int highLightPartition = -1;
    //--

    //else
    public LevelGrid grid;

    GameObject generatedLevel;
    GeneratedLevel generatedLevelComponent;

    private void InstantiateTile(GridTile tile, Transform parent, string name = "")
    {
        if (tile.prefab == null)
            return;
        if (tile.instantiated)
            return;

        GameObject InstantiateObj;

        /*PlaceableObject objectPlacingProperties = tile.prefab.gameObject.GetComponent<PlaceableObject>();
        float offset = objectPlacingProperties.tileAlignment;*/
        float offset = tile.prefab.tileAlignment;
        Quaternion rotation = tile.tag.RotationTagToQuaternion() * Quaternion.Euler(tile.prefab.defaultEulerAngles);
        //Debug.Log(tile.prefab.gameObject.name + " " + tile.prefab.defaultEulerAngles + " " + rotation);
        //rotation = Quaternion.Euler(tile.placedPrefab.transform.rotation.eulerAngles.x + rotation.eulerAngles.x,
        //rotation.eulerAngles.y + rotation.eulerAngles.y, tile.placedPrefab.transform.rotation.eulerAngles.z + rotation.eulerAngles.z);

        switch (tile.tag.alignment)
        {
            case TileAlignment.left:
                InstantiateObj = Instantiate(tile.prefab.gameObject, tile.worldCoordinates - (new Vector3(offset, 0, 0)),
                    rotation, parent);
                break;
            case TileAlignment.right:
                InstantiateObj = Instantiate(tile.prefab.gameObject, tile.worldCoordinates + (new Vector3(offset, 0, 0)),
                    rotation, parent);
                break;
            case TileAlignment.up:
                InstantiateObj = Instantiate(tile.prefab.gameObject, tile.worldCoordinates + (new Vector3(0, 0, offset)),
                    rotation, parent);
                break;
            case TileAlignment.down:
                InstantiateObj = Instantiate(tile.prefab.gameObject, tile.worldCoordinates - (new Vector3(0, 0, offset)),
                    rotation, parent);
                break;
            case TileAlignment.center:
                InstantiateObj = Instantiate(tile.prefab.gameObject, tile.worldCoordinates,
                    rotation, parent);
                break;
            default:
                InstantiateObj = Instantiate(tile.prefab.gameObject, tile.worldCoordinates,
                    rotation, parent);
                break;
        }

        if (!String.IsNullOrEmpty(name))
            InstantiateObj.name = name;


        tile.instantiated = true;
    }

    protected void InstantiateRoomAt(Vector2Int gridCoordinates, Room room, Transform parent, int id = -1)
    {
        GameObject roomGameObj = new GameObject("room" + (id == -1 ? "" : id.ToString()));
        roomGameObj.transform.parent = parent;
        GameObject roomWalls = new GameObject("walls");
        GameObject roomFloor = new GameObject("floor");
        GameObject putItSomeWhere = new GameObject("XZ"); //TODO: zdelaty shos' z cym
        roomWalls.transform.parent = roomGameObj.transform;
        roomFloor.transform.parent = roomGameObj.transform;
        putItSomeWhere.transform.parent = roomGameObj.transform;

        roomFloor.AddComponent<MeshFilter>();
        roomFloor.AddComponent<MeshRenderer>();
        roomWalls.AddComponent<MeshFilter>();
        roomWalls.AddComponent<MeshRenderer>();
        for (int i = 0; i < room.entrances.Count; i++)
        {
            LocalTile localTile = room.entrances[i];

            GridTile gridTile = grid.roomsMap[gridCoordinates.x + localTile.localCoordinates.x + ((gridCoordinates.y + localTile.localCoordinates.y) * grid.gridSize.y)];
            InstantiateTile(gridTile, putItSomeWhere.transform);
        }

        for (int i = 0; i < room.roomTiles.Count; i++)
        {
            LocalTile localTile = room.roomTiles[i];

            GridTile gridTile = grid.roomsMap[gridCoordinates.x + localTile.localCoordinates.x + ((gridCoordinates.y + localTile.localCoordinates.y) * grid.gridSize.y)];

            //gridTile.SetPrefab(localTile.prefab);

            if (gridTile.tag.type == TileType.floor)
                InstantiateTile(gridTile, roomFloor.transform);
            else if (gridTile.tag.type == TileType.wall)
                InstantiateTile(gridTile, roomWalls.transform);
            else
                InstantiateTile(gridTile, putItSomeWhere.transform);
        }
      
        
        //TODO: Make Correnct Combiner for Meshes
        //MeshCombiner.CombineMeshes(roomFloor);
        //MeshCombiner.CombineMeshes(roomWalls);
    }

    protected void InstantiateCorridor(Corridor corridor, Transform parent, int id = 0)
    {
        GameObject corridorGameObj = new GameObject("corridor " + (id == 0 ? "" : id.ToString()));
        corridorGameObj.transform.parent = parent;
        GameObject roomWalls = new GameObject("walls");
        GameObject roomFloor = new GameObject("floor");
        roomWalls.transform.parent = corridorGameObj.transform;
        roomFloor.transform.parent = corridorGameObj.transform;

        roomFloor.AddComponent<MeshFilter>();
        roomFloor.AddComponent<MeshRenderer>();
        roomWalls.AddComponent<MeshFilter>();
        roomWalls.AddComponent<MeshRenderer>();
        for (int i = 0; i < corridor.floorTiles.Count; i++)
        {
            InstantiateTile(corridor.floorTiles[i], roomFloor.transform, "floor-" + i);
            InstantiateTile(corridor.wallTiles[i * 2], roomWalls.transform, "wall-" + i + "-0-" + corridor.wallTiles[i * 2].tag.ToString());
            InstantiateTile(corridor.wallTiles[i * 2 + 1], roomWalls.transform, "wall-" + i + "-1-" + corridor.wallTiles[i * 2 + 1].tag.ToString());
        }
    }

    protected virtual void GetSettings()
    {
        //general
        seed = generationSettings.seed;
        center = generationSettings.center;
        cellSize = generationSettings.cellSize;
        minRoomsAmount = generationSettings.minRoomsAmount;
        maxRoomsAmount = generationSettings.maxRoomsAmount;
        minimumRandomRoomSize = generationSettings.minimumRandomRoomSize;
        maximumRandomRoomSize = generationSettings.maximumRandomRoomSize;
        //prefabs
        defaultRoomPrefabsSets = generationSettings.defaultRoomPrefabsSets;
        corridorsPrefabsSet = generationSettings.corridorsPrefabsSet;
        customRoomPrefabsSets = generationSettings.customRoomPrefabsSets;

        //MainPath
        minSideRoomsAmount = generationSettings.minSideRoomsAmount;
        maxSideRoomsAmount = generationSettings.maxSideRoomsAmount;
        customSideRoomPrefabsSets = generationSettings.customSideRoomPrefabsSets;
    }

    protected bool CheckRoomCollision(Vector2Int point, Room room, List<Room> placedRooms, int minDist)
    {
        int xmin = point.x - minDist;
        int ymin = point.y - minDist;
        int xmax = point.x + room.width + minDist;
        int ymax = point.y + room.height + minDist;
        foreach (Room r in placedRooms)
        {
            if (xmax >= r.gridCoordinates.x &&
                xmin <= r.gridCoordinates.x + r.width &&
                ymax >= r.gridCoordinates.y &&
                ymin <= r.gridCoordinates.y + r.height)
            {
                return true;
            }
        }
        return false;
    }

    protected virtual Vector2Int VirualGridRoomsPlacement(Room[] rooms, int minDistanceBetweenRooms, bool optimize=true)
    {
        rooms = rooms.OrderBy(r => UnityEngine.Random.Range(0, 100)).ToArray();
        List<Room> placedRooms = new List<Room>();
        Vector2Int gridEdgeOffset = new Vector2Int(0, 0);
        Vector2Int virtualGridSize = new Vector2Int(rooms[0].width, rooms[0].height) + gridEdgeOffset;
        int roomsXMin = int.MaxValue;
        int roomsYMin = int.MaxValue;
        int idx = 0;
        const int ATTEMPTS = 6;
        int tryCounter = ATTEMPTS;
        while (idx < rooms.Length)
        {
            if (rooms[idx].IsPlaced())
            {
                placedRooms.Add(rooms[idx]);
                idx++;
                continue;
            }

            Vector2Int roomSize = new Vector2Int(rooms[idx].width, rooms[idx].height);
            Vector2Int placeingPoint = Utils.RandomVector2Int(gridEdgeOffset, virtualGridSize - gridEdgeOffset);
            if (CheckRoomCollision(placeingPoint, rooms[idx], placedRooms, minDistanceBetweenRooms))
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
            if (optimize)
                placedRooms[i].gridCoordinates -= (new Vector2Int(roomsXMin, roomsYMin) - gridEdgeOffset);
            if (placedRooms[i].gridCoordinates.x + placedRooms[i].width > xmax)
                xmax = placedRooms[i].gridCoordinates.x + placedRooms[i].width;
            if (placedRooms[i].gridCoordinates.y + placedRooms[i].height > ymax)
                ymax = placedRooms[i].gridCoordinates.y + placedRooms[i].height;
        }
        return new Vector2Int(xmax, ymax) + gridEdgeOffset;
    }

    private Vector2Int VirualGridRoomsPlacementWithHub(Room[] rooms, Room hub, int minDistanceBetweenRooms)
    {
        /*Vector2Int gridSize = VirualGridRoomsPlacement(rooms, minDistanceBetweenRooms, optimize:false);
        Vector2Int hubPlaceingPoint = gridSize / 2 - hub.GetRoomSize() / 2;
        hub.gridCoordinates = hubPlaceingPoint;
        List<Room> roomsForReplacing = new List<Room>();
        for (int i = 0; i < rooms.Length; i++)
        {
            if (CheckRoomCollision(rooms[i].gridCoordinates, rooms[i], new List<Room>() { hub }, minDistanceBetweenRooms))
            {
                rooms[i].RestartPlacingPos();
                //rooms[i].gridCoordinates += new Vector2Int(2, 2);
                roomsForReplacing.Add(rooms[i]);
            }
        }
        Room[] roomsWithHub = new Room[rooms.Length+1];
        for (int i = 0; i < rooms.Length; i++) { roomsWithHub[i] = rooms[i]; }
        roomsWithHub[rooms.Length] = hub;

        gridSize = VirualGridRoomsPlacement(roomsWithHub, minDistanceBetweenRooms, optimize:false);
        return gridSize;*/

        rooms = rooms.OrderBy(r => UnityEngine.Random.Range(0, 100)).ToArray();
        List<Room> placedRooms = new List<Room>();
        Vector2Int gridEdgeOffset = new Vector2Int(0, 0);
        Vector2Int virtualGridSize = new Vector2Int(rooms[0].width, rooms[0].height) + gridEdgeOffset;
        int roomsXMin = int.MaxValue;
        int roomsYMin = int.MaxValue;
        int idx = 0;
        const int ATTEMPTS = 6;
        int tryCounter = ATTEMPTS;
        while (idx < rooms.Length)
        {
            if (rooms[idx].IsPlaced())
            {
                placedRooms.Add(rooms[idx]);
                idx++;
                continue;
            }

            Vector2Int roomSize = new Vector2Int(rooms[idx].width, rooms[idx].height);
            Vector2Int placeingPoint = Utils.RandomVector2Int(gridEdgeOffset, virtualGridSize - gridEdgeOffset);
            if (CheckRoomCollision(placeingPoint, rooms[idx], placedRooms, minDistanceBetweenRooms))
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
        Vector2Int hubPlaceingPoint = gridSize / 2 - hub.GetRoomSize() / 2;
        hub.gridCoordinates = hubPlaceingPoint;
        List<Room> roomsForReplacing = new List<Room>();
        for (int i = 0; i < rooms.Length; i++)
        {
            if (CheckRoomCollision(rooms[i].gridCoordinates, rooms[i], new List<Room>() { hub }, minDistanceBetweenRooms))
            {
                //rooms[i].RestartPlacingPos();
                //rooms[i].gridCoordinates += new Vector2Int(2, 2);
                roomsForReplacing.Add(rooms[i]);
            }
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

                //placedRooms.Add(roomsForReplacing[idx]);
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

    public void Generate()
    {
        GetSettings();

        if (generatedLevelComponent != null)
            generatedLevelComponent.DeleteLevel();

        int generatorSeed;
        if (seed != 0)
            generatorSeed = seed;
        else
            generatorSeed = (int)DateTime.Now.Ticks;
        UnityEngine.Random.InitState(generatorSeed);

        generatedLevel = new GameObject("Generated Level (seed:" + generatorSeed + ")");
        generatedLevelComponent = generatedLevel.AddComponent<GeneratedLevel>();

        roomsNumber = UnityEngine.Random.Range(minRoomsAmount, maxRoomsAmount + 1);

        List<Room> possibleStartRooms = new List<Room>();
        List<Room> possibleEndRooms = new List<Room>();
        Room[] roomsPool = RoomsGenerator.GenerateRoomsPool(customRoomPrefabsSets, minimumRandomRoomSize, maximumRandomRoomSize, roomsNumber, out possibleStartRooms, out possibleEndRooms);

        GameObject rooms = new GameObject("Rooms");
        rooms.transform.parent = generatedLevel.transform;
        GameObject corridors = new GameObject("Corridors");
        corridors.transform.parent = generatedLevel.transform;


        Vector2Int gridSize = VirualGridRoomsPlacement(roomsPool, 2);
        Vector3 upperLeftCorner = new Vector3(gridSize.x * -2, 0, gridSize.y * 2);
        Vector3 offset = center;
        gridSize = new Vector2Int(Mathf.Max(gridSize.x, gridSize.y), Mathf.Max(gridSize.x, gridSize.y));
        grid = new LevelGrid(upperLeftCorner + offset, gridSize, cellSize);
        Debug.Log("Grid Size: " + grid.gridSize);
        RoomsGenerator.PrepareRoomsData(roomsPool, defaultRoomPrefabsSets, customRoomPrefabsSets);
        RoomsGenerator.SetUpPrefabsGlobal(grid, roomsPool);

        //CorridorsGenerator.GenerateSpaningTreeConnection(grid, roomsPool, corridorsPrefabsSet, straightPath);

        RoomsGenerator.PrepareRoomsData(roomsPool, defaultRoomPrefabsSets, customRoomPrefabsSets); // need to update prefabs for entries after adding corridors.
        RoomsGenerator.SetUpPrefabsGlobal(grid, roomsPool);



        for (int i = 0; i < roomsPool.Length; i++)
            InstantiateRoomAt(roomsPool[i].gridCoordinates, roomsPool[i], rooms.transform, i);

        foreach (Corridor corridor in grid.corridors)
            InstantiateCorridor(corridor, corridors.transform);

        /*
        generatedLevelComponent.CleanUp();
        */
    }

    /*public void GenerateHub()
    {
        GetSettings();

        if (generatedLevelComponent != null)
            generatedLevelComponent.DeleteLevel();

        int generatorSeed;
        if (seed != 0)
            generatorSeed = seed;
        else
            generatorSeed = (int)DateTime.Now.Ticks;
        UnityEngine.Random.InitState(generatorSeed);

        generatedLevel = new GameObject("Generated Level (seed:" + generatorSeed + ")");
        generatedLevelComponent = generatedLevel.AddComponent<GeneratedLevel>();

        roomsNumber = UnityEngine.Random.Range(minRoomsAmount, maxRoomsAmount + 1);
        
        roomsNumber++; // add hub room at the end of array
        Room[] roomsPool = RoomsGenerator.GenerateRoomsPool(customRoomPrefabsSets, minimumRandomRoomSize, maximumRandomRoomSize, roomsNumber);
        roomsPool[roomsNumber-1] = new Room(20, 20); // place holder for proper hub room.

        GameObject rooms = new GameObject("Rooms");
        rooms.transform.parent = generatedLevel.transform;
        GameObject corridors = new GameObject("Corridors");
        corridors.transform.parent = generatedLevel.transform;


        Vector2Int gridSize = VirualGridRoomsPlacementWithHub(roomsPool.Take(roomsNumber-1).ToArray(), roomsPool[roomsNumber-1], 2);
        Vector3 upperLeftCorner = new Vector3(gridSize.x * -2, 0, gridSize.y * 2);
        Vector3 offset = center;
        gridSize = new Vector2Int(Mathf.Max(gridSize.x, gridSize.y), Mathf.Max(gridSize.x, gridSize.y));
        grid = new LevelGrid(upperLeftCorner + offset, gridSize, cellSize);
        RoomsGenerator.PrepareRoomsData(roomsPool, defaultRoomPrefabsSets, customRoomPrefabsSets);
        RoomsGenerator.SetUpPrefabsGlobal(grid, roomsPool);



        for (int i = 0; i < roomsPool.Length; i++)
            InstantiateRoomAt(roomsPool[i].gridCoordinates, roomsPool[i], rooms.transform, i);

        //foreach (Corridor corridor in grid.corridors)
            //InstantiateCorridor(corridor, corridors.transform);

    }*/

    public void GenerateMainPath()
    {
        GetSettings();

        if (generatedLevelComponent != null)
            generatedLevelComponent.DeleteLevel();

        int generatorSeed;
        if (seed != 0)
            generatorSeed = seed;
        else
            generatorSeed = (int)DateTime.Now.Ticks;
        UnityEngine.Random.InitState(generatorSeed);

        generatedLevel = new GameObject("Generated Level (seed:" + generatorSeed + ")");
        generatedLevelComponent = generatedLevel.AddComponent<GeneratedLevel>();

        roomsNumber = UnityEngine.Random.Range(minRoomsAmount, maxRoomsAmount + 1);
        int sideRoomsNumber = UnityEngine.Random.Range(minSideRoomsAmount, maxSideRoomsAmount + 1);

        /*List<int> possibleStartRooms = new List<int>();
        List<Room> possibleEndRooms = new List<int>();
        Room[] mainRoomsPool = RoomsGenerator.GenerateRoomsPool(customRoomPrefabsSets, minimumRandomRoomSize, maximumRandomRoomSize, roomsNumber, out possibleStartRooms, out possibleEndRooms);
        int startRoom = Utils.RandomChoise(possibleStartRooms);
        int endRoom = Utils.RandomChoise(possibleEndRooms);

        //Room[] sideRoomsPool = RoomsGenerator.GenerateRoomsPool(customSideRoomPrefabsSets, minimumRandomRoomSize, maximumRandomRoomSize, sideRoomsNumber, out possibleStartRooms, out possibleEndRooms);


        GameObject rooms = new GameObject("Rooms");
        rooms.transform.parent = generatedLevel.transform;
        GameObject corridors = new GameObject("Corridors");
        corridors.transform.parent = generatedLevel.transform;

        Room[] allRooms = mainRoomsPool;
        Vector2Int gridSize = VirualGridRoomsPlacement(allRooms, 2);
        Vector3 upperLeftCorner = new Vector3(gridSize.x * -2, 0, gridSize.y * 2);
        Vector3 offset = center;
        gridSize = new Vector2Int(Mathf.Max(gridSize.x, gridSize.y), Mathf.Max(gridSize.x, gridSize.y));
        grid = new LevelGrid(upperLeftCorner + offset, gridSize, cellSize);
        Debug.Log("Grid Size: " + grid.gridSize);
        //RoomsGenerator.PrepareRoomsData(allRooms, defaultRoomPrefabsSets, customRoomPrefabsSets);
        RoomsGenerator.PrepareRoomsData(mainRoomsPool, defaultRoomPrefabsSets, customRoomPrefabsSets);
        //RoomsGenerator.PrepareRoomsData(sideRoomsPool, defaultRoomPrefabsSets, customSideRoomPrefabsSets);
        RoomsGenerator.SetUpPrefabsGlobal(grid, allRooms);

        //CorridorsGenerator.GenerateMainPathConnection(grid, mainRoomsPool, allRooms, corridorsPrefabsSet);

        //RoomsGenerator.PrepareRoomsData(allRooms, defaultRoomPrefabsSets, customRoomPrefabsSets);
        RoomsGenerator.PrepareRoomsData(mainRoomsPool, defaultRoomPrefabsSets, customRoomPrefabsSets);
        //RoomsGenerator.PrepareRoomsData(sideRoomsPool, defaultRoomPrefabsSets, customSideRoomPrefabsSets);
        RoomsGenerator.SetUpPrefabsGlobal(grid, allRooms);

        

        for (int i = 0; i < allRooms.Length; i++)
            InstantiateRoomAt(allRooms[i].gridCoordinates, allRooms[i], rooms.transform, i);

        foreach (Corridor corridor in grid.corridors)
            InstantiateCorridor(corridor, corridors.transform);
        */
        /*
        generatedLevelComponent.CleanUp();
        */
    }



    public void ShowHighLightedTileInfo()
    {
        if (grid.ValidCoordinates(highLight)) 
        {
            GridTile selectedTile = grid.GetRoomMapTile(highLight);
            VisualDebug.DrawSquareWithMark(selectedTile.worldCoordinates + new Vector3(0, 2, 0), grid.cellSize, Color.green);
            Debug.Log(selectedTile.tag.ToString());
        }
    }

    private void Update()
    {

        if (generatedLevel == null)
            return;
        
        

        if (grid.ValidCoordinates(highLight))
        {
            GridTile selectedTile = grid.GetRoomMapTile(highLight);
            VisualDebug.DrawSquareWithMark(selectedTile.worldCoordinates + new Vector3(0, 2, 0), grid.cellSize, Color.green);
        }

        grid.DebugDrawRoomMap(Color.black);
        grid.DebugDrawCorridorMap(Color.white);

    }
}
