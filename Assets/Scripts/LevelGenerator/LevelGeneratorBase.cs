using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public abstract class LevelGeneratorBase : MonoBehaviour
{

    protected GenerationSettings generationSettings;
    
    protected PrefabsSet[] defaultRoomPrefabsSets;
    protected PrefabsSet corridorsPrefabsSet;
    
    protected CustomRoomPrefabsSet[] customRoomPrefabsSets;
    
    protected  Vector2Int size;
    protected Vector3 center;
    protected bool straightPath;
    protected float cellSize;
    protected int seed;
    
    protected int minRoomsAmount;
    protected int maxRoomsAmount;
    
    protected Vector2Int minimumRandomRoomSize;
    protected Vector2Int maximumRandomRoomSize;
    
    protected int minSideRoomsAmount;
    protected int maxSideRoomsAmount;
    protected CustomRoomPrefabsSet[] customSideRoomPrefabsSets;

    //debug
    [SerializeField] public Vector2Int highLight = new Vector2Int(0, 0);
    [SerializeField] public int highLightPartition = -1;
    //--

    //else
    public static LevelGrid grid;

    protected static GameObject generatedLevel;
    protected static GeneratedLevel generatedLevelComponent;
    protected GameObject rooms;
    protected GameObject corridors;

    public LevelGeneratorBase(GenerationSettings settings)
    {
        generationSettings = settings;
    }

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

    protected bool CheckRoomCollision(Vector2Int point, Room room, List<Room> placedRooms, int minDist)
    {
        Tuple<Vector2Int, Vector2Int> AABB = room.GetAABBForPoint(point);

        int xmin = AABB.Item1.x - minDist;
        int ymin = AABB.Item1.y - minDist;
        int xmax = AABB.Item2.x + minDist;
        int ymax = AABB.Item2.y + minDist;
        foreach (Room r in placedRooms)
        {
            Tuple<Vector2Int, Vector2Int> otherAABB = r.GetAABBForPoint(r.gridCoordinates);
            if (xmax >= otherAABB.Item1.x &&
                xmin <= otherAABB.Item2.x &&
                ymax >= otherAABB.Item1.y &&
                ymin <= otherAABB.Item2.y)
            {
                return true;
            }
        }
        return false;
    }

    protected virtual Vector2Int VirualGridRoomsPlacementDefault(Room[] rooms, int minDistanceBetweenRooms, bool optimize = true)
    {
        rooms = rooms.OrderBy(r => UnityEngine.Random.Range(0, 100)).ToArray();
        List<Room> placedRooms = new List<Room>();
        Vector2Int gridEdgeOffset = new Vector2Int(1, 1);
        Vector2Int virtualGridSize = new Vector2Int(rooms[0].width, rooms[0].height) + gridEdgeOffset;
        int roomsXMin = int.MaxValue;
        int roomsYMin = int.MaxValue;
        int idx = 0;
        const int ATTEMPTS = 6;
        int tryCounter = ATTEMPTS;
        while (idx < rooms.Length)
        {
            /*if (rooms[idx].IsPlaced())
            {
                placedRooms.Add(rooms[idx]);
                idx++;
                continue;
            }*/

            Vector2Int roomSize = rooms[idx].GetRoomSize();
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

    protected Vector2Int VirualGridRoomsPlacementDefault(Room[] rooms, int minDistanceBetweenRooms, Room start, Room end, float minDistanceBetweenStartAndEnd, bool optimize = true)
    {
        rooms = rooms.OrderBy(r => UnityEngine.Random.Range(0, 100)).ToArray();
        List<Room> placedRooms = new List<Room>();
        Vector2Int gridEdgeOffset = new Vector2Int(1, 1);
        Vector2Int virtualGridSize = new Vector2Int(rooms[0].width, rooms[0].height);
        int roomsXMin = int.MaxValue;
        int roomsYMin = int.MaxValue;
        int idx = 0;
        const int ATTEMPTS = 80;
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
            if (optimize)
                placedRooms[i].gridCoordinates -= (new Vector2Int(roomsXMin, roomsYMin) - gridEdgeOffset);
            if (placedRooms[i].gridCoordinates.x + placedRooms[i].width > xmax)
                xmax = placedRooms[i].gridCoordinates.x + placedRooms[i].width;
            if (placedRooms[i].gridCoordinates.y + placedRooms[i].height > ymax)
                ymax = placedRooms[i].gridCoordinates.y + placedRooms[i].height;
        }

        if (Vector2.Distance(start.GetRoomCenterInGridCoordinates(), end.GetRoomCenterInGridCoordinates()) < minDistanceBetweenStartAndEnd)
        {
            start.RestartPlacingPos();
            end.RestartPlacingPos();
            return VirualGridRoomsPlacementDefault(rooms, minDistanceBetweenRooms, start, end, minDistanceBetweenStartAndEnd, false);
        }
        return new Vector2Int(xmax, ymax) + gridEdgeOffset;
    }

    protected virtual void GetSettings()
    {
        //general
        seed = generationSettings.seed;
        center = generationSettings.center;
        straightPath = generationSettings.straightPath;
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

    protected virtual void PreGeneration()
    {
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

        rooms = new GameObject("Rooms");
        rooms.transform.parent = generatedLevel.transform;
        corridors = new GameObject("Corridors");
        corridors.transform.parent = generatedLevel.transform;
    }

    protected abstract void Generate();



    public void RunGenerator()
    {
        GetSettings();
        PreGeneration();
        Generate();
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
