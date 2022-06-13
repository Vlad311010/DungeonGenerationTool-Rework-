using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathFinding 
{
    private static int Heuristic(GridTile end, GridTile tile)
    {
        return (int)Vector2Int.Distance(tile.gridCoordinates, end.gridCoordinates) + UnityEngine.Random.Range(0, 100);
    }
    public static Tuple<List<GridTile>, List<GridTile>> AStar(LevelGrid grid, GridTile start, GridTile end,
        bool avoidIntersection=false, bool straightPath=true)
    {
        //int generatorSeed = (int)DateTime.Now.Ticks;
        //UnityEngine.Random.InitState(generatorSeed);
        for (int y = 0; y < grid.gridRealSize.y; y++)
        {
            for (int x = 0; x < grid.gridRealSize.x; x++)
            {
                GridTile tile = grid.GetCorridorMapTile(x, y);
                tile.g = tile.initG;
                tile.f = Int32.MaxValue;
            }
        }

        start.f = Heuristic(start, end);

        List<GridTile> closedList = new List<GridTile>();
        List<GridTile> openList = new List<GridTile>() { start };
        Dictionary<Vector2Int, Vector2Int> parents = new Dictionary<Vector2Int, Vector2Int>();
        parents.Add(start.gridCoordinates, new Vector2Int(-1, -1));

        Func<GridTile, bool> Filter = t => (t.tag.map != TileMap.room || t.gridCoordinates == grid.FromCorridorToRoomMap(end).gridCoordinates);
        //Func<GridTile, bool> Filter = t => ( (t.tag.map != TileMap.room && !t.tag.avoideInPathFinding) || t.gridCoordinates == grid.FromCorridorToRoomMap(end).gridCoordinates);

        while (openList.Count != 0)
        {
            //int r = UnityEngine.Random.Range(0, Math.Min(openList.Count, 5)); // that line gives some path randomnes
            int r;
            if (straightPath)
                r = 0;
            else
                r = UnityEngine.Random.Range(0, Math.Min(openList.Count, 5));

            GridTile currentNode = openList[r];
            //if (currentNode.tag.map == TileMap.corridor && UnityEngine.Random.value > 0.1)
            //continue;

            openList.RemoveAt(r);
            closedList.Add(currentNode);

            if (currentNode.gridCoordinates == end.gridCoordinates)
            {
                return Backpropagate(grid, start, end, parents);
            }

            List<GridTile> adjacent = grid.GetAdjacentTiles(currentNode);
            for (int i = 0; i < adjacent.Count; i++)
            {
                //if (!closedList.Contains(adjacent[i]) && Filter(grid.FromCorridorToRoomMap(adjacent[i])))

                if (!closedList.Contains(adjacent[i]))
                //(grid.CanConvertCorridorToRoomMap(adjacent[i]) || Filter(grid.FromCorridorToRoomMap(adjacent[i]))))
                {
                    if (avoidIntersection && adjacent[i].tag.avoideInPathFinding)
                        continue;
                    if (grid.CanConvertCorridorToRoomMap(adjacent[i]) && !Filter(grid.FromCorridorToRoomMap(adjacent[i])))
                        continue;
                    //Debug.Log(grid.CanConvertCorridorToRoomMap(adjacent[i]) + " " + adjacent[i].gridCoordinates);
                    if (!openList.Contains(adjacent[i]))
                    {

                        //int newG = grid.GetCorridorMapTile(currentNode.gridCoordinates).g + (int)Vector2Int.Distance(currentNode.gridCoordinates, adjacent[i].gridCoordinates);
                        int newG = currentNode.g + (int)Vector2Int.Distance(currentNode.gridCoordinates, adjacent[i].gridCoordinates);
                        adjacent[i].g = newG;
                        adjacent[i].f = newG + Heuristic(end, adjacent[i]);
                        parents[adjacent[i].gridCoordinates] = currentNode.gridCoordinates;
                        openList.Add(adjacent[i]);
                        openList.OrderBy(x => x.f).ToList();
                    }
                }
            }
        }
        //Debug.LogError("Cant find path from " + start.gridCoordinates + " to " + end.gridCoordinates);
        throw new OperationCanceledException("Cant find path from " + start.gridCoordinates + " to " + end.gridCoordinates);
        //return new Tuple<List<GridTile>, List<GridTile>>(new List<GridTile>(), new List<GridTile>());
    }


    static Tuple<List<GridTile>, List<GridTile>> Backpropagate(LevelGrid grid,
        GridTile start, GridTile end, Dictionary<Vector2Int, Vector2Int> parents)
    {
        List<GridTile> path = new List<GridTile>() { end };
        List<GridTile> adjacent = new List<GridTile>();
        GridTile curNode = end;
        while (true)
        {
            Vector2Int parentCoordinates = parents[curNode.gridCoordinates];
            if ((parentCoordinates == start.gridCoordinates) ||
                 (parentCoordinates == new Vector2Int(-1, -1))) // mb use Equal
                break;

            GridTile parent = grid.GetCorridorMapTile(parentCoordinates);

            /*if ( (parent.gridCoordinates == start.gridCoordinates) ||
                 (parent.gridCoordinates == new Vector2Int(-1, -1)) ) // mb use Equal
                break;*/
            path.Add(parent);
            curNode = parent;
        }
        path.Add(start);
        path.Reverse();

        for (int i = 0; i < path.Count; i++)
        {
            if (i == 0 || i == path.Count - 1)
            {
                int x = path[i].gridCoordinates.x;
                int y = path[i].gridCoordinates.y;

                GridTile up = grid.GetCorridorMapTile(x, y - 1); // u
                GridTile right = grid.GetCorridorMapTile(x + 1, y); // r
                GridTile down = grid.GetCorridorMapTile(x, y + 1); // d
                GridTile left = grid.GetCorridorMapTile(x - 1, y); // l

                //Func<GridTile, bool> filter = t => !path.Contains(t) && (!grid.IsValidRoomMapCoordinates(x, y) || !grid.GetRoomMapTile(up.gridCoordinates).tag.IsFloor());
                Func<GridTile, bool> filter = t => !path.Contains(t) &&
                (!grid.CanConvertCorridorToRoomMap(t) || (!grid.FromCorridorToRoomMap(t).tag.IsFloor()));

                if (filter(up))
                    adjacent.Add(up);
                if (filter(right))
                    adjacent.Add(right);
                if (filter(down))
                    adjacent.Add(down);
                if (filter(left))
                    adjacent.Add(left);

            }
            else
            {
                foreach (GridTile tile in grid.GetAdjacentTiles(path[i]))
                {
                    if (!path.Contains(tile))
                        adjacent.Add(tile);
                }
            }
        }


        //return new Tuple<List<GridTile>, List<GridTile>>(path, adjacent.Skip(1).ToList());
        return new Tuple<List<GridTile>, List<GridTile>>(path, adjacent);
    }
}
