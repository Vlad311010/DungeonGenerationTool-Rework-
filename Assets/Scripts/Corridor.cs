using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corridor
{
    public List<GridTile> floorTiles { get; set; }
    public List<GridTile> wallTiles { get; set; }
    public Room startRoom { get; private set; }
    public Room endRoom { get; private set; }

    public Corridor(List<GridTile> path, List<GridTile> adjacentTiles, Room startRoom, Room endRoom)
    {
        floorTiles = path;
        wallTiles = adjacentTiles;
        this.startRoom = startRoom;
        this.endRoom = endRoom;
        GenerateCorridor();
    }

    public void GenerateCorridor()
    {
        for (int i = 0; i < floorTiles.Count; i++)
        {
            GridTile cur = floorTiles[i];
            /*if (wallTiles[i].tag.subtype == TileTag.TileSubtype.empty)
                wallTiles[i].SetTag(new TileTag(
                    TileTag.TileType.corridor,
                    TileTag.TileSubtype.wall,
                    TileTag.TileAlignment.center,
                    TileTag.TileStatus.none
                ));*/
            if (floorTiles[i].tag.map == TileMap.corridor && (floorTiles[i].tag.type == TileType.wall || floorTiles[i].tag.type == TileType.empty))
            {
                floorTiles.RemoveAt(i);
                wallTiles.RemoveAt(i * 2);
                wallTiles.RemoveAt(i * 2);
                continue;
            }

            cur.SetTag(new TileTag(
                    TileMap.corridor,
                    TileType.floor,
                    TileAlignment.center,
                    TileRotation.identity
                    //TileStatus.none
                ));
            cur.tag.avoideInPathFinding = true;
        }
    }

    public static Corridor UniteCorridors(Corridor corridor1, Corridor corridor2)
    {
        corridor1.floorTiles.AddRange(corridor2.floorTiles);
        corridor1.wallTiles.AddRange(corridor2.wallTiles);
        corridor1.endRoom = corridor2.endRoom;
        corridor2.startRoom = corridor1.startRoom;
        return corridor1;
    }
}
