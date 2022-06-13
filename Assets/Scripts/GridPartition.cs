using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridPartition
{
    //public Vector3 center { get; private set; }
    
    public Vector3 upperLeftCorner { get; private set; }
    public Vector2Int upperLeftCornerGrid { get; private set; }
    public Vector2Int size { get; private set; } // width, height
    public bool splitable { get; private set; }

    public List<GridPartition> adjacent { get; private set; }

    public Room placedRoom { get; private set; }
    public Vector2Int placedRoomLocalCoordinates { get; private set; }


    //Vector3 upperLeftCorner = center + new Vector3(-cellSize * gridSize.x / 2f, 0, cellSize * size.y / 2f);

    public GridPartition(Vector3 upperLeftCorner, Vector2Int upperLeftCornerGridCoordinates,
        Vector2Int size)
    {
        //this.center = center;
        this.upperLeftCorner = upperLeftCorner;
        this.upperLeftCornerGrid = upperLeftCornerGridCoordinates;
        this.size = size;
        splitable = true;
    }

    public void SetAsUnspliteable() { splitable = false; }

    /*public static bool[] CheckAdjacentBorders(DungeonGrid grid, GridPartition partition)
    {
    }*/

    public void PlaceRoom(Room room, Vector2Int localCoordinates)
    {
        this.placedRoom = room;
        this.placedRoomLocalCoordinates = localCoordinates;
    }

    public bool HasRoom() { return placedRoom != null; }

    public bool CanFitRoom(Room room)
    {
        Vector2Int roomSize = new Vector2Int(room.width, room.height);
        Vector2Int sizeDifference = this.size - roomSize;
        return sizeDifference.x > 2 && sizeDifference.y > 2;
    }
    public bool CanBeSplited(Vector2 minSize)
    {
        return this.size.y / 2 < minSize.y || this.size.x / 2 < minSize.x;
    }

    public bool CanBeSplitedHorizintal(Vector2 minSize) { return this.size.y / 2 > minSize.y; }
    public bool CanBeSplitedVertical(Vector2 minSize) { return this.size.x / 2 > minSize.x; }

    public  static void SplitWithSizeCheking(LevelGrid grid, GridPartition partition, bool horizontal, out GridPartition newPartition1,
        out GridPartition newPartition2, Vector2 minSize)
    {
        Vector2Int newSizePartition1;
        Vector2Int newSizePartition2;

        Vector3 offsetPartition2;
        bool oddSize;

        if (horizontal && partition.size.y / 2 < minSize.y)
            horizontal = false;
        else if (!horizontal && partition.size.x / 2 < minSize.x)
            horizontal = true;

        if (horizontal)
        {
            oddSize = partition.size.y % 2 == 0;

            newSizePartition1 = new Vector2Int(partition.size.x, partition.size.y / 2);
            newSizePartition2 = oddSize ? newSizePartition1 : new Vector2Int(partition.size.x, partition.size.y / 2 + 1);

            offsetPartition2 = new Vector3(0, 0, grid.cellSize * newSizePartition1.y);

            newPartition1 = new GridPartition(partition.upperLeftCorner, partition.upperLeftCornerGrid, newSizePartition1);
            newPartition2 = new GridPartition(partition.upperLeftCorner - offsetPartition2,
                partition.upperLeftCornerGrid + new Vector2Int(0, newSizePartition1.y), newSizePartition2);
        }
        else
        {
            oddSize = partition.size.x % 2 == 0;

            newSizePartition1 = new Vector2Int(partition.size.x / 2, partition.size.y);
            newSizePartition2 = oddSize ? newSizePartition1 : new Vector2Int(partition.size.x / 2 + 1, partition.size.y);

            offsetPartition2 = new Vector3(grid.cellSize * newSizePartition1.x, 0, 0);

            newPartition1 = new GridPartition(partition.upperLeftCorner, partition.upperLeftCornerGrid, newSizePartition1);
            newPartition2 = new GridPartition(partition.upperLeftCorner + offsetPartition2,
                partition.upperLeftCornerGrid + new Vector2Int(newSizePartition1.x, 0), newSizePartition2);
        }

        if (newPartition1.size.x / 2 < minSize.x && newPartition1.size.y / 2 < minSize.y)
            newPartition1.SetAsUnspliteable();
        if (newPartition2.size.x / 2 < minSize.x && newPartition2.size.y / 2 < minSize.y)
            newPartition2.SetAsUnspliteable();
    }

    public static void Split(LevelGrid grid, GridPartition partition, bool horizontal, out GridPartition newPartition1, out GridPartition newPartition2)
    {
        Vector2Int newSizePartition1;
        Vector2Int newSizePartition2;

        Vector3 offsetPartition2;
        bool oddSize;

        if (horizontal)
        {
            oddSize = partition.size.y % 2 == 0;

            newSizePartition1 = new Vector2Int(partition.size.x, partition.size.y / 2);
            newSizePartition2 = oddSize ? newSizePartition1 : new Vector2Int(partition.size.x, partition.size.y / 2 + 1);

            offsetPartition2 = new Vector3(0, 0, grid.cellSize * newSizePartition1.y);

            newPartition1 = new GridPartition(partition.upperLeftCorner, partition.upperLeftCornerGrid, newSizePartition1);
            newPartition2 = new GridPartition(partition.upperLeftCorner - offsetPartition2,
                partition.upperLeftCornerGrid + new Vector2Int(0, newSizePartition1.y), newSizePartition2);
        }
        else
        {
            oddSize = partition.size.x % 2 == 0;

            newSizePartition1 = new Vector2Int(partition.size.x / 2, partition.size.y);
            newSizePartition2 = oddSize ? newSizePartition1 : new Vector2Int(partition.size.x / 2 + 1, partition.size.y);

            offsetPartition2 = new Vector3(grid.cellSize * newSizePartition1.x, 0, 0);

            newPartition1 = new GridPartition(partition.upperLeftCorner, partition.upperLeftCornerGrid, newSizePartition1);
            newPartition2 = new GridPartition(partition.upperLeftCorner + offsetPartition2,
                partition.upperLeftCornerGrid + new Vector2Int(newSizePartition1.x, 0), newSizePartition2);
        }
    }


    public bool CanBeSplitedHorizotalAfterRoomPlacing(Room room, Vector2Int roomCoordinates) // cheking if room is placed on spliting line(midle of partition)
    {
        int splitingPoint = this.size.y / 2;

        int roomHighestPoint = roomCoordinates.y;
        int roomLowestPoint = roomCoordinates.y + room.height;


        return roomHighestPoint < splitingPoint && roomLowestPoint < splitingPoint
               || roomHighestPoint > splitingPoint && roomLowestPoint > splitingPoint;
    }

    public bool RoomBelongToUpperPartition() // make sense only if partition with room can be spleated (room doesnt lie on horizontal(vertical?) min line of partition)
    {
        if (this.placedRoom == null)
        {
            Debug.LogWarning("Room not placed");
            return false;
        }

        int splitingPoint = this.size.y / 2;
        int roomHighestPoint = this.placedRoomLocalCoordinates.y;
        return roomHighestPoint < splitingPoint;
    }

    public bool CanBeSplitedVerticalAfterRoomPlacing(Room room, Vector2Int roomCoordinates) 
    {
        int splitingPoint = this.size.x / 2;

        int roomLeftSidePoint = roomCoordinates.x;
        int roomRightSidePoint = roomCoordinates.x + room.width;

        return roomLeftSidePoint < splitingPoint && roomRightSidePoint < splitingPoint
               || roomLeftSidePoint > splitingPoint && roomRightSidePoint > splitingPoint;
    }

    public bool RoomBelongToLeftPartition() // make sense only if partition with room can be spleated (room doesnt lie on horizontal(vertical?) min line of partition)
    {
        if (this.placedRoom == null)
            Debug.LogError("Room not placed");

        int splitingPoint = this.size.x / 2;
        int roomLeftSidePoint = this.placedRoomLocalCoordinates.x;
        return roomLeftSidePoint < splitingPoint;
    }

    public Vector2 CalculatePlacedRoomCenter() // center of room)Grid coordinates)
    {
        Vector2Int roomCoordinates = upperLeftCornerGrid + placedRoomLocalCoordinates;
        return new Vector2(roomCoordinates.x + placedRoom.width / 2, roomCoordinates.y + placedRoom.height / 2);
    }

    bool IsPointInsidePartition(Vector2Int point)
    {
        int xMin = this.upperLeftCornerGrid.x;
        int yMin = this.upperLeftCornerGrid.y;
        int xMax = xMin + this.size.x;
        int yMax = yMin + this.size.y;
        return point.x >= xMin && point.x <= xMax && point.y >= yMin && point.y <= yMax;
    }

    public void FindAdjacent(LevelGrid grid, List<GridPartition> partitions)
    {
        int xMin = this.upperLeftCornerGrid.x;
        int yMin = this.upperLeftCornerGrid.y;
        int xMax = xMin + this.size.x;
        int yMax = yMin + this.size.y;
        Vector2Int[] pointsToCheck = new Vector2Int[8]
        {
            new Vector2Int(xMin-1, yMin),
            new Vector2Int(xMin,   yMin-1),
            new Vector2Int(xMax+1, yMin),
            new Vector2Int(xMax,   yMin-1),
            new Vector2Int(xMin-1, yMax),
            new Vector2Int(xMin,   yMax+1),
            new Vector2Int(xMax+1, yMax),
            new Vector2Int(xMax,   yMax+1)
        };
        this.adjacent = new List<GridPartition>();
        for (int i = 0; i < 8; i++)
        {
            if (!grid.IsValidRoomMapCoordinates(pointsToCheck[i]))
                continue;

            for (int j = 0; j < partitions.Count; j++)
            {
                if (partitions[j].IsPointInsidePartition(pointsToCheck[i]))
                {
                    if (partitions[j].adjacent == null)
                        partitions[j].adjacent = new List<GridPartition>();


                    if (!this.adjacent.Exists(x => x.upperLeftCornerGrid == partitions[j].upperLeftCornerGrid))
                        this.adjacent.Add(partitions[j]);
                    if (!partitions[j].adjacent.Exists(x => x.upperLeftCornerGrid == this.upperLeftCornerGrid))
                        partitions[j].adjacent.Add(this);
                }

            }
        }
    }

    public GridTile FromLocalToGrid(LevelGrid grid, LocalTile localTile)
    {
        Vector2Int RoomGridCoordinates = this.upperLeftCornerGrid + this.placedRoomLocalCoordinates;
        return grid.GetRoomMapTile(RoomGridCoordinates.x + localTile.localCoordinates.x, RoomGridCoordinates.y + localTile.localCoordinates.y);
    }

    public EntranceTile GetClosestEntrancePoint(LevelGrid grid, Vector2 target)
    {
        return placedRoom.entrances.OrderBy(x => Vector2.Distance(FromLocalToGrid(grid, x).gridCoordinates, target)).First();
        //return placedRoom.entrances.Where(x => !x.connected).OrderBy(x => Vector2.Distance(FromLocalToGrid(grid, x).gridCoordinates, target)).First();
    }



    /*public static List<GridPartition> SplitGrid(LevelGrid grid, Vector2Int minSize, int iter)
    {
        List<GridPartition> partitions = new List<GridPartition>() { new GridPartition(grid.upperLeftCornerWorld, Vector2Int.zero, grid.gridSize) };
        List<GridPartition> unsplitable = new List<GridPartition>();
        for (int i = 0; i < iter; i++)
        {
            bool horizontal = Random.value > 0.5f;

            GridPartition newPartition1;
            GridPartition newPartition2;

            //int idx = Random.Range(0, partitions.Count);
            int idx = 0;
            SplitWithSizeCheking(grid, partitions[idx], horizontal, out newPartition1, out newPartition2, minSize);


            partitions.RemoveAt(idx);
            if (newPartition1.splitable)
                partitions.Add(newPartition1);
            else
                unsplitable.Add(newPartition1);

            if (newPartition2.splitable)
                partitions.Add(newPartition2);
            else
                unsplitable.Add(newPartition2);

            partitions.OrderBy(x => Utils.Size(x.size));

            if (partitions.Count == 0)
            { Debug.LogWarning("spliting break"); break;  }
        }
        partitions = partitions.Concat(unsplitable).ToList();

        return partitions;
    }*/
}

