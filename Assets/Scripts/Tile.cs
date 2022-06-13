using UnityEngine;

public abstract class Tile
{
    public TileTag tag { get; protected set; }
    public WrappedObject prefab { get; protected set; }

    public Tile(TileTag tag, WrappedObject placedPrefab = null)
    {
        this.tag = tag;
        this.prefab = placedPrefab;
    }

    public void SetPrefab(WrappedObject placedPrefab)
    {
        this.prefab = placedPrefab;
    }

}

public class LocalTile : Tile
{
    public Vector2Int localCoordinates { get; protected set; }
    public LocalTile(Vector2Int localGridCoordinates, TileTag tag, WrappedObject placedPrefab = null) : base(tag, placedPrefab)
    {
        localCoordinates = localGridCoordinates;
        this.tag = tag;
        this.prefab = placedPrefab;
    }

    public void SetCoordinates(Vector2Int vector2) { localCoordinates = vector2; }
}

public class EntranceTile : LocalTile
{
    public bool connected { get; private set; }
    public Room connectedWith { get; private set; }
    public EntranceTile(Vector2Int localGridCoordinates, TileTag tag, WrappedObject placedPrefab = null) : base(localGridCoordinates, tag, placedPrefab)
    {
        localCoordinates = localGridCoordinates;
        this.tag = tag;
        tag.type = TileType.enter;
        this.prefab = placedPrefab;
        connected = false;
        connectedWith = null;
    }

    public EntranceTile(LocalTile localTile) : base(localTile.localCoordinates, localTile.tag, localTile.prefab)
    {
        connected = false;
        connectedWith = null;
    }

    public void Connect(Room room)
    {
        connected = true;
        connectedWith = room;
    }
}

public class GridTile : Tile
{
    public Vector3 worldCoordinates { get; private set; }
    public Vector2Int gridCoordinates { get; private set; }
    public int g { get; set; }
    public int initG = 0;
    public int f { get; set; }


    public bool instantiated { get; set; }

    public GridTile(Vector3 wordCoordinates, Vector2Int gridCoordinates, TileTag tag, WrappedObject placedPrefab = null) : base(tag, placedPrefab)
    {
        this.worldCoordinates = wordCoordinates;
        this.gridCoordinates = gridCoordinates;
        this.prefab = placedPrefab;
        instantiated = false;
    }

    public void SetTag(TileTag tag)
    {
        this.tag = tag;
    }


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

        return this.gridCoordinates == ((GridTile)o).gridCoordinates;
    }

    public static bool operator ==(GridTile a, GridTile b)
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

    public static bool operator !=(GridTile a, GridTile b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        return gridCoordinates.GetHashCode();
    }

}
