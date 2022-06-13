using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Room
{
    private static int roomsCounter = 0;
    private int id;
    public bool customRoom { get; private set; }

    public int height { get; private set; }
    public int width { get; private set; }

    public Vector2Int gridCoordinates { get; set; }

    public List<LocalTile> roomTiles;
    public List<EntranceTile> entrances { get; private set; }

    public int rotation { get; private set; }

    public Room(int width, int height)
    {
        id = roomsCounter++;
        customRoom = false;
        roomTiles = new List<LocalTile>();
        entrances = new List<EntranceTile>();
        RestartPlacingPos();

        this.width = width;
        this.height = height;
        GenerateRectangleRoom();
}

    public Room(CustomRoomData roomData)
    {
        id = roomsCounter++;
        customRoom = true;
        this.roomTiles = new List<LocalTile>();
        this.entrances = new List<EntranceTile>();
        RestartPlacingPos();

        this.width = roomData.size;
        this.height = roomData.size;
        GenerateRoomFromRoomData(roomData);
    }

    public void RestartPlacingPos() { gridCoordinates = new Vector2Int(-1, -1); }
    public bool IsPlaced() { return gridCoordinates != new Vector2Int(-1, -1); }

    public void GenerateRectangleRoom()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2Int offset = new Vector2Int(x, y);
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    LocalTile borderTile;
                    if ((x == 0 && y == 0) || (x == width - 1 && y == height - 1)
                        || (x == 0 && y == height - 1) || (x == width - 1 && y == 0))
                    {
                        borderTile = (new LocalTile(offset,
                            new TileTag(
                                TileMap.room,
                                TileType.empty,
                                TileAlignment.center,
                                TileRotation.identity
                                )
                            ));
                        continue;
                    }
                    else if (x == 0)
                        borderTile = (new LocalTile(offset,
                            new TileTag(
                                TileMap.room,
                                TileType.wall,
                                TileAlignment.right,
                                TileRotation.right
                                )
                            ));
                    else if (x == width - 1)
                        borderTile = (new LocalTile(offset,
                            new TileTag(
                                TileMap.room,
                                TileType.wall,
                                TileAlignment.left,
                                TileRotation.left
                                )
                            ));
                    else if (y == 0)
                        borderTile = (new LocalTile(offset,
                            new TileTag(
                                TileMap.room,
                                TileType.wall,
                                TileAlignment.down,
                                TileRotation.down
                                )
                            ));
                    else //if (y == height - 1)
                        borderTile = (new LocalTile(offset,
                            new TileTag(
                                TileMap.room,
                                TileType.wall,
                                TileAlignment.up,
                                TileRotation.up
                                )
                            ));
                    entrances.Add(new EntranceTile(borderTile));
                }
                else
                {
                    roomTiles.Add(new LocalTile(offset, new TileTag(TileMap.room, TileType.floor, TileAlignment.center)));
                }
            }
        }
    }

   
    private void GenerateRoomFromRoomData(CustomRoomData roomData)
    {
        for (int y = 0; y < this.height; y++)
        {
            for (int x = 0; x < this.width; x++)
            {
                if (roomData.tagsData[x, y].type == TileType.empty)
                    continue;

                LocalTile tile = new LocalTile(new Vector2Int(x, y), roomData.tagsData[x, y]);
                

                if (roomData.tagsData[x, y].type == TileType.enter)
                    entrances.Add(new EntranceTile(tile));
                else
                    roomTiles.Add(tile);
            }
        }
    }


    public Tuple<Vector2Int, Vector2Int> GetAABBForPoint(Vector2Int point) // (min x, min y), (max x, max y)
    {
        int xmin = point.x;
        int ymin = point.y;
        int xmax = point.x + width;
        int ymax = point.y + height;

        Vector2Int min = new Vector2Int(xmin, ymin);
        Vector2Int max = new Vector2Int(xmax, ymax);

        return new Tuple<Vector2Int, Vector2Int>(min, max);
    }

    public Vector2 GetRoomCenterInGridCoordinates()
    {
        if (gridCoordinates == null)
            throw new UnityException("Room does't have grid coordinates");

        return new Vector2(gridCoordinates.x + width / 2, gridCoordinates.y + height / 2);
    }

    public Vector2Int GetRoomSize() { return new Vector2Int(width, height); }
    public LocalTile GetRandomEntrancePoint()
    {
        return Utils.RandomChoise(entrances);
    }

    public EntranceTile GetClosestEntrancePoint(LevelGrid grid, Vector2 target)
    {
        return entrances.OrderBy(x => Vector2.Distance(FromLocalToGrid(grid, x).gridCoordinates, target)).First();
        //return placedRoom.entrances.Where(x => !x.connected).OrderBy(x => Vector2.Distance(FromLocalToGrid(grid, x).gridCoordinates, target)).First();
    }

    /*public void AddEntrancePoint(GridTile roomMapTile)
    {
        LocalTile localTile = new LocalTile(
            new Vector2Int(gridCoordinates.x - roomMapTile.gridCoordinates.x, gridCoordinates.y - roomMapTile.gridCoordinates.y),
            new TileTag(TileMap.room, TileType.enter, TileAlignment.center));
        entrances.Add(localTile);
    }*/

    /*public Vector2 Center()
    {
        return new Vector2(gridCoordinates.x + width / 2, gridCoordinates.y + height / 2);
    }*/

    /*public float DistanceBetweenRooms(Room anotherRoom)
    {
        return Vector2.Distance(this.Center(), anotherRoom.Center());
    }
    */
    public GridTile FromLocalToGrid(LevelGrid grid, LocalTile localTile)
    {
        return grid.GetRoomMapTile(gridCoordinates.x + localTile.localCoordinates.x, gridCoordinates.y + localTile.localCoordinates.y);
        //return grid.roomsMap[this.gridCoordinates.x + localTile.localCoordinates.x + ((this.gridCoordinates.y + localTile.localCoordinates.y) * grid.gridSize.y)];
    }

    public override string ToString()
    {
        return "Room(" + id + ")";
    }

    public static int GetLastRoomId() { return roomsCounter - 1; }

    public override bool Equals(object o)
    {
        if (ReferenceEquals(o, null))
        {
            return false;
        }
        if (ReferenceEquals(this, o))
        {
            return true;
        }

        return this.height == ((Room)o).height && this.width == ((Room)o).width;
    }

    public static bool operator ==(Room a, Room b)
    {
        if (ReferenceEquals(a, b))
        {
            return true;
        }
        if (ReferenceEquals(a, null))
        {
            return false;
        }
        if (ReferenceEquals(b, null))
        {
            return false;
        }

        return a.Equals(b);
    }

    public static bool operator !=(Room a, Room b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        int hash = 17;
        hash = hash * 23 + id.GetHashCode();
        hash = hash * 23 + height.GetHashCode();
        hash = hash * 23 + width.GetHashCode();
        return hash;
    }


}
