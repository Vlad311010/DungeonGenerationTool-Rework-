using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VisualDebug
{
    public static void DrawCircle(Vector3 center, float radius, Color color)
    {
        Vector3 prevPos = center + new Vector3(radius, 0, 0);
        for (int i = 0; i < 30; i++)
        {
            float angle = (float)(i + 1) / 30.0f * Mathf.PI * 2.0f;
            Vector3 newPos = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Debug.DrawLine(prevPos, newPos, color);
            prevPos = newPos;
        }
    }

    public static void DrawSquare(Vector3 center, float size, Color color)
    {
        Vector3 prevPos = center + new Vector3(size / 2, 0, -size / 2);
        Vector3 dir = new Vector3(size, 0, 0);
        for (int i = 0; i < 4; i++)
        {
            dir = Quaternion.AngleAxis(-90, Vector3.up) * dir;
            Vector3 newPos = prevPos + dir;
            Debug.DrawLine(prevPos, newPos, color);
            prevPos = newPos;
        }
    }

    public static void DrawSquareWithMark(Vector3 center, float size, Color color)
    {
        Vector3 prevPos = center + new Vector3(size / 2, 0, -size / 2);
        Vector3 dir = new Vector3(size, 0, 0);
        for (int i = 0; i < 4; i++)
        {
            dir = Quaternion.AngleAxis(-90, Vector3.up) * dir;
            Vector3 newPos = prevPos + dir;
            Vector3 markPos = prevPos + Quaternion.AngleAxis(-45, Vector3.up) * dir;
            Debug.DrawLine(prevPos, newPos, color);
            Debug.DrawLine(prevPos, markPos, color);

            prevPos = newPos;
        }
    }

    public static void DrawSquareFromCorner(Vector3 upperLeftCorner, float size, Color color)
    {
        Vector3 prevPos = upperLeftCorner;
        Vector3 dir = new Vector3(size, 0, 0);
        for (int i = 0; i < 4; i++)
        {
            dir = Quaternion.AngleAxis(90, Vector3.up) * dir;
            Vector3 newPos = prevPos - dir;
            Debug.DrawLine(prevPos, newPos, color);
            prevPos = newPos;
        }
    }

    // draw grid partition
    public static void DrawGridPartition(GridPartition partition, float cellSize, Color color, float hightOffset=0) 
    {
        Vector3 upperLeftCorner = partition.upperLeftCorner;//space.center + new Vector3(-cellSize * space.size.x / 2f, 0, cellSize * space.size.y / 2f);
        Vector3 pos = upperLeftCorner + new Vector3(cellSize / 2, hightOffset, -cellSize / 2);
        for (int i = 0; i < partition.size.y; i++)
        {
            for (int j = 0; j < partition.size.x; j++)
            {
                DrawSquare(pos, cellSize, color);
                pos += new Vector3(cellSize, 0, 0);
            }
            pos = new Vector3(upperLeftCorner.x + cellSize / 2, pos.y, pos.z - cellSize);
        }
    }
    
    // draw grid partition but border tiles marked with X.
    public static void DrawGridPartition(GridPartition partition, float cellSize, Color color, Color BorderColor, float hightOffset=0)
    {
        Vector3 upperLeftCorner = partition.upperLeftCorner;//space.center + new Vector3(-cellSize * space.size.x / 2f, 0, cellSize * space.size.y / 2f);
        Vector3 pos = upperLeftCorner + new Vector3(cellSize / 2, hightOffset, -cellSize / 2);
        for (int i = 0; i < partition.size.y; i++)
        {
            for (int j = 0; j < partition.size.x; j++)
            {
                if (j == 0 || j == partition.size.x - 1 || i == 0 || i == partition.size.y - 1)
                    DrawSquareWithMark(pos, cellSize, BorderColor);
                else
                    DrawSquare(pos, cellSize, color);
                pos += new Vector3(cellSize, 0, 0);
            }
            pos = new Vector3(upperLeftCorner.x + cellSize / 2, pos.y, pos.z - cellSize);
        }

    }

    public static void ShowRooomTiles(LevelGrid grid, GridPartition partition, Color color)
    {
        Vector2Int start = partition.upperLeftCornerGrid + partition.placedRoomLocalCoordinates;
        if (partition.placedRoom != null)
        {
            foreach (LocalTile localTile in partition.placedRoom.roomTiles)
            {
                GridTile gridTile = grid.GetRoomMapTile(start.x + localTile.localCoordinates.x, start.y + localTile.localCoordinates.y);
                DrawSquareWithMark(gridTile.worldCoordinates, grid.cellSize, color);
            }
        }
    }
}
