using UnityEngine;
using System;
public enum TileMap
{
    none = 0,
    room = 1,
    corridor = 2
}

public enum TileType
{
    empty = 0,
    floor = 1,

    wall = 2,
    cornerWall = 3,
    parallelWall = 4,
    tripleWall = 5,

    enter = 6,
}

public enum TileAlignment
{
    center = 0,

    up = 1,
    right = 2,
    down = 3,
    left = 4,

    /*up_right = 5,
    down_right = 6,
    down_left = 7,
    up_left = 8*/
}

public enum TileRotation
{
    identity = 0,

    up = 1,
    right = 2,
    down = 3,
    left = 4,

    /*up_right = 5,
    down_right = 6,
    down_left = 7,
    up_left = 8*/
}

/*public enum TileStatus
{
    none = 0,
    possibleEntryPoint = 1,
    ignore = 2,
}*/


[Serializable]
public class TileTag
{

    public TileMap map { get; private set; }
    public TileType type { get; set; }
    public TileAlignment alignment { get; set; }
    public TileRotation rotation { get; set; }
    //public TileStatus status { get; set; }
    public bool avoideInPathFinding;
    public TileTag(TileMap map, TileType type, TileAlignment alignment, TileRotation rotation = TileRotation.identity/*, TileStatus status = TileStatus.none*/)
    {
        this.map = map;
        this.type = type;
        this.alignment = alignment;
        this.rotation = rotation;
        //this.status = status;
    }

    public Quaternion RotationTagToQuaternion()
    {
        switch (this.rotation)
        {
            case TileRotation.up:
                return Quaternion.Euler(0, 0, 0);
            case TileRotation.right:
                return Quaternion.Euler(0, 90, 0);
            case TileRotation.down:
                return Quaternion.Euler(0, 180, 0);
            case TileRotation.left:
                return Quaternion.Euler(0, -90, 0);

            /*case TileRotation.up_right:
                return Quaternion.Euler(0, 180, 0);
            case TileRotation.down_right:
                return Quaternion.Euler(0, -90, 0);
            case TileRotation.down_left:
                return Quaternion.Euler(0, 0, 0);
            case TileRotation.up_left:
                return Quaternion.Euler(0, 90, 0);*/

            default:
                return Quaternion.Euler(0, 0, 0);
        }
    }

    public void Rotate90D()
    {
        if (rotation == TileRotation.identity)
            rotation = TileRotation.up;

        rotation = (TileRotation)(((int)rotation + 1) % Enum.GetValues(typeof(TileRotation)).Length);

        if (alignment == TileAlignment.center)
            return;

        alignment = (TileAlignment)(((int)alignment + 1) % Enum.GetValues(typeof(TileAlignment)).Length);
        if (alignment == TileAlignment.center)
            alignment = (TileAlignment)(((int)alignment + 1) % Enum.GetValues(typeof(TileAlignment)).Length);
    }

    public override string ToString()
    {
        return map.ToString() + "-" + type.ToString() + "-" + alignment.ToString() + "-" + rotation + "-"/* + status.ToString()*/;
    }
    public static TileTag Empty()
    {
        return new TileTag(TileMap.none, TileType.empty, TileAlignment.center);
    }

    public bool IsFloor() { return type == TileType.floor; }
    public bool IsSimpleWall() { return type == TileType.wall; }
    public bool IsAnyWall() { return type == TileType.wall || type == TileType.cornerWall || type == TileType.parallelWall || type == TileType.tripleWall; }
    public bool IsEmpty() { return type == TileType.empty; }
    public bool IsSimpleWallOrEmpty() { return IsSimpleWall() || IsEmpty(); }
    public bool IsAnyWallOrEmpty() { return IsAnyWall() || IsEmpty(); }




}
