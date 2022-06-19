using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid
{
    public Vector3 upperLeftCornerWorld { get; private set; }
    public Vector2Int upperLeftCornerGrid { get; private set; }
    public Vector2Int gridSize { get; private set; }
    public float cellSize { get; private set; }
    public GridTile[] roomsMap { get; private set; }
    public GridTile[] corridorsMap { get; private set; }

    public List<Room> rooms { get; private set; }
    public List<Corridor> corridors { get; private set; }
    //public Room[] rooms { get; private set; }
    //public List<Corridor> corridors { get; private set; }

    const int corridorMapExtensionSize = 6;
    const int halfExtensionSize = corridorMapExtensionSize / 2;
    public Vector2Int gridRealSize { get; private set; }

    public LevelGrid(Vector3 upperLeftCorner, Vector2Int gridSize, float cellSize)
    {
        this.upperLeftCornerWorld = upperLeftCorner;
        this.upperLeftCornerGrid = new Vector2Int(0, 0);
        this.gridSize = gridSize;
        gridRealSize = gridSize + new Vector2Int(corridorMapExtensionSize, corridorMapExtensionSize);
        this.cellSize = cellSize;
        roomsMap = new GridTile[gridSize.x * gridSize.y];
        corridorsMap = new GridTile[(gridSize.x + corridorMapExtensionSize) * (gridSize.y + corridorMapExtensionSize)];

        rooms = new List<Room>();
        corridors = new List<Corridor>();

        GenerateGrid();
    }

    private void GenerateGrid()
    {
        Vector3 cellPos = upperLeftCornerWorld + new Vector3(cellSize / 2f, 0, -cellSize / 2f);
        for (int i = 0; i < gridSize.y; i++)
        {
            for (int j = 0; j < gridSize.x; j++)
            {
                roomsMap[j + gridSize.y * i] = new GridTile(cellPos, new Vector2Int(j, i), TileTag.Empty());
                corridorsMap[j + gridSize.y * i] = new GridTile(cellPos, new Vector2Int(j, i), TileTag.Empty());
                cellPos += new Vector3(cellSize, 0);
            }
            cellPos = new Vector3(upperLeftCornerWorld.x + cellSize / 2, upperLeftCornerWorld.y, cellPos.z - cellSize);
        }

        cellPos = upperLeftCornerWorld + new Vector3(cellSize / 2f, 0, -cellSize / 2f) + new Vector3(-cellSize * halfExtensionSize, 0, +cellSize * halfExtensionSize);
        for (int i = 0; i < gridRealSize.y; i++)
        {
            for (int j = 0; j < gridRealSize.x; j++)
            {
                corridorsMap[j + gridRealSize.y * i] = new GridTile(cellPos, new Vector2Int(j, i), TileTag.Empty());
                cellPos += new Vector3(cellSize, 0);
            }
            cellPos = new Vector3(upperLeftCornerWorld.x + cellSize / 2 - cellSize * 3, upperLeftCornerWorld.y, cellPos.z - cellSize);
        }

    }

    public bool IsValidRoomMapCoordinates(int x, int y)
    {
        return !(x < 0 || y < 0 || x > gridSize.x - 1 || y > gridSize.y - 1);
    }
    public bool IsValidRoomMapCoordinates(Vector2Int coordinates) { return IsValidRoomMapCoordinates(coordinates.x, coordinates.y); }
    public bool IsValidCorridorMapCoordinates(int x, int y)
    {
        return !(x < 0 || y < 0 || x > gridRealSize.x - 1 || y > gridRealSize.y - 1);
    }
    public bool IsValidCorridorMapCoordinates(Vector2Int coordinates) { return IsValidCorridorMapCoordinates(coordinates.x, coordinates.y); }
    public bool ValidCoordinates(Vector2Int coordinates) { return IsValidRoomMapCoordinates(coordinates.x, coordinates.y); }

    public GridTile GetRoomMapTile(int x, int y)
    {
        if (!IsValidRoomMapCoordinates(x, y))
            throw new System.ArgumentException(string.Format("Coordinates {0}, {1} are out of room map bounds", x, y));

        return roomsMap[x + gridSize.y * y];
    }
    public GridTile GetRoomMapTile(Vector2Int coordinates) { return GetRoomMapTile(coordinates.x, coordinates.y); }
    public GridTile GetCorridorMapTile(int x, int y)
    {
        if (!IsValidCorridorMapCoordinates(x, y))
            throw new System.ArgumentException(string.Format("Coordinates {0}, {1} are out of corridor map bounds", x, y));

        return corridorsMap[x + gridRealSize.y * y];
    }
    public GridTile GetCorridorMapTile(Vector2Int coordinates) { return GetCorridorMapTile(coordinates.x, coordinates.y); }
    public GridTile FromRoomToCorridorMap(GridTile roomMapTile)
    {
        return GetCorridorMapTile(roomMapTile.gridCoordinates + new Vector2Int(halfExtensionSize, halfExtensionSize));
    }

    public bool CanConvertCorridorToRoomMap(GridTile corridorMapTile)
    {
        Vector2Int coordinates = corridorMapTile.gridCoordinates;
        return coordinates.x >= halfExtensionSize && coordinates.y >= halfExtensionSize
            && coordinates.x - halfExtensionSize < gridSize.x && coordinates.y - halfExtensionSize < gridSize.y;
    }

    public GridTile FromCorridorToRoomMap(GridTile corridorMapTile)
    {
        Vector2Int coordinates = corridorMapTile.gridCoordinates;
        if (!CanConvertCorridorToRoomMap(corridorMapTile))
            throw new System.ArgumentException();

        return GetRoomMapTile(coordinates - new Vector2Int(halfExtensionSize, halfExtensionSize));
    }

    public void AddCorridor(Corridor corridor)
    {
        corridors.Add(corridor);
    }

    public List<GridTile> GetAdjacentTiles(GridTile tile)
    {
        List<GridTile> adjacentTiles = new List<GridTile>();
        int x = tile.gridCoordinates.x;
        int y = tile.gridCoordinates.y;

        if (IsValidCorridorMapCoordinates(x, y - 1))
            adjacentTiles.Add(GetCorridorMapTile(x, y - 1));
        if (IsValidCorridorMapCoordinates(x + 1, y))
            adjacentTiles.Add(GetCorridorMapTile(x + 1, y));
        if (IsValidCorridorMapCoordinates(x, y + 1))
            adjacentTiles.Add(GetCorridorMapTile(x, y + 1));
        if (IsValidCorridorMapCoordinates(x - 1, y))
            adjacentTiles.Add(GetCorridorMapTile(x - 1, y));

        return adjacentTiles;
    }


    public void DebugDrawRoomMap(Color color)
    {
        foreach (GridTile tile in roomsMap)
        {
            VisualDebug.DrawSquare(tile.worldCoordinates, cellSize, color);
        }
    }

    public void DebugDrawCorridorMap(Color color)
    {
        foreach (GridTile tile in corridorsMap)
        {
            VisualDebug.DrawSquare(tile.worldCoordinates, cellSize, color);
        }
    }

    public Vector3 GridCoordinates2World(Vector2Int gridCoordinates)
    {
        if (gridCoordinates.x < 0 || gridCoordinates.y < 0)
            Debug.LogError("raise error coordinates are counting from upperLeftConrer (0,0);");
        // raise error coordinates are counting from upperLeftConrer (0,0);

        if (gridCoordinates.x > gridSize.x || gridCoordinates.y > gridSize.y)
            Debug.LogError("raise error out of bound");
        //raise error out of bound

        return roomsMap[gridCoordinates.y * gridSize.y + gridCoordinates.x].worldCoordinates;
    }

    public Vector3 WorldCenter()
    {
        return new Vector3(upperLeftCornerWorld.x + (gridSize.x * cellSize) / 2, upperLeftCornerWorld.y, upperLeftCornerWorld.z - (gridSize.y * cellSize) / 2);
    }

    public Vector2Int WorldCoordinates2Grid(Vector3 worldCoordinates)
    {
        //TODO

        //(center.x - worldCoordinates.x) distance
        //int tileOffset = (center.x - worldCoordinates.x) / cellSize;

        return Vector2Int.zero;
    }
}

