using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public static class CorridorsGenerator
{
    private static float[][] CreateDistancesGraph(Room[] rooms)
    {
        float[][] graphData = new float[rooms.Length][];
        for (int i = 0; i < rooms.Length; i++)
        {
            graphData[i] = new float[rooms.Length];
            for (int j = 0; j < rooms.Length; j++)
            {
                graphData[i][j] = Vector2.Distance(rooms[i].GetRoomCenterInGridCoordinates(), rooms[j].GetRoomCenterInGridCoordinates());
            }
        }

        return graphData;
    }

    private static List<Tuple<int, int, float>> GetGraphEdges(float[][] graph)
    {
        Debug.Assert(graph[0].Length == graph.Length);

        List<Tuple<int, int, float>> edges = new List<Tuple<int, int, float>>();
        for (int i = 0; i < graph.Length; i++)
        {
            for (int j = 0; j < i; j++)
            {
                if (graph[i][j] > 0)
                    edges.Add(new Tuple<int, int, float>(i, j, graph[i][j]));
            }
        }
        return edges;
    }

    private static List<Tuple<int, int, float>> GetGraphEdges(float[][] graph, List<int> ignoreNodes)
    {
        Debug.Assert(graph[0].Length == graph.Length);

        List<Tuple<int, int, float>> edges = new List<Tuple<int, int, float>>();
        for (int i = 0; i < graph.Length; i++)
        {
            for (int j = 0; j < i; j++)
            {
                if (ignoreNodes.Contains(i) || ignoreNodes.Contains(j))
                    continue;

                if (graph[i][j] > 0)
                    edges.Add(new Tuple<int, int, float>(i, j, graph[i][j]));
            }
        }
        return edges;
    }

    private static List<int> Neigbours(float[][] graph, int node, int parent = -1)
    {
        List<int> children = new List<int>();
        for (int i = 0; i < graph.Length; i++)
        {
            if (i == parent)
                continue;

            if (graph[node][i] > 0)
            {
                children.Add(i);
            }
        }
        return children;
    }
    private static bool CycleSearch(float[][] graph)
    {
        List<int> visited = new List<int>();
        List<int> finished = new List<int>();
        for (int i = 0; i < graph.Length; i++)
        {
            if (HaveCycle(graph, i, -1, ref visited, ref finished))
                return true;
        }

        return false;
    }
    private static bool HaveCycle(float[][] graph, int node, int prevNode, ref List<int> visited, ref List<int> finished)
    {
        if (finished.Contains(node))
            return false;
        if (visited.Contains(node))
            return true;
        visited.Add(node);
        List<int> children = Neigbours(graph, node, prevNode);
        for (int i = 0; i < children.Count; i++)
        {
            if (HaveCycle(graph, children[i], node, ref visited, ref finished))
                return true;
        }
        visited.Remove(node);
        finished.Add(node);
        return false;
    }


    private static float[][] BuildMinimalSpaningTree(int roomsNumber, List<Tuple<int, int, float>> edges)
    {
        edges = edges.OrderBy(x => x.Item3).ToList();
        int edgesCount = 0;
        float[][] graph = new float[roomsNumber][];
        for (int i = 0; i < roomsNumber; i++)
        {
            graph[i] = new float[roomsNumber];
            for (int j = 0; j < roomsNumber; j++)
                graph[i][j] = -1;
        }
        while (edgesCount != roomsNumber - 1)
        {
            if (edges.Count == 0) { throw new Exception("all edges has been visited"); }
            Tuple<int, int, float> newEdge = edges[0];
            edges.RemoveAt(0);


            graph[newEdge.Item1][newEdge.Item2] = newEdge.Item3;
            graph[newEdge.Item2][newEdge.Item1] = newEdge.Item3;

            if (CycleSearch(graph))
            {
                graph[newEdge.Item1][newEdge.Item2] = -1;
                graph[newEdge.Item2][newEdge.Item1] = -1;
            }
            else
                edgesCount++;
        }

        return graph;
    }

    private static float[][] BuildMinimalSpaningTree(int disconectedRooms, int roomsNumber, List<Tuple<int, int, float>> edges, float[][] graph, bool extendGraph = true)
    {
        edges = edges.OrderBy(x => x.Item3).ToList();
        int edgesCount = 0;
        if (extendGraph)
            graph = ExtendGraph(graph, disconectedRooms);

        while (edgesCount != disconectedRooms)
        {
            if (edges.Count == 0) { throw new Exception("all edges has been visited"); }

            Tuple<int, int, float> newEdge = edges[0];
            edges.RemoveAt(0);

            if (graph[newEdge.Item1][newEdge.Item2] > 0)
                continue;

            graph[newEdge.Item1][newEdge.Item2] = newEdge.Item3;
            graph[newEdge.Item2][newEdge.Item1] = newEdge.Item3;

            if (CycleSearch(graph))
            {
                graph[newEdge.Item1][newEdge.Item2] = -1;
                graph[newEdge.Item2][newEdge.Item1] = -1;
            }
            else
                edgesCount++;
        }

        return graph;
    }

    private static int[] GetNClosestNode(int node, float[][] graph, int n)
    {
        int[] indexes = new int[graph.Length];
        for (int i = 0; i < graph.Length; i++) { indexes[i] = i; }
        return indexes.Where(x => x != node).OrderBy(x => graph[node][x]).Take(n).ToArray();
    }

    private static float[][] ExtendGraph(float[][] graph, int additionalSize)
    {
        int newLength = graph.Length + additionalSize;
        float[][] newGraph = new float[newLength][];
        for (int i = 0; i < newLength; i++)
        {
            newGraph[i] = new float[newLength];
            for (int j = 0; j < newLength; j++)
                if (i < graph.Length && j < graph.Length)
                    newGraph[i][j] = graph[i][j];
                else
                    newGraph[i][j] = -1;
        }
        return newGraph;
    }

    private static float[][] AddRandomEdges(float[][] graph, int n, float[][] distGraph)
    {
        int edgesCount = 0;
        while (edgesCount < n)
        {
            int firstNode = UnityEngine.Random.Range(0, graph.Length);
            int secondNode = UnityEngine.Random.Range(0, graph.Length);
            if (graph[firstNode][secondNode] > 0 || distGraph[firstNode][secondNode] > 0)
            {
                graph[firstNode][secondNode] = distGraph[firstNode][secondNode];
                graph[secondNode][firstNode] = distGraph[firstNode][secondNode];
                edgesCount++;
            }
        }

        return graph;
    }

    private static List<int> Backtrace(int start, int end, Dictionary<int, int> parents)
    {
        //List<int> path = new List<int>() { end };
        List<int> path = new List<int>();
        int curNode = end;
        //int ec = 100;
        while (true)
        {
            /*if (--ec < 0)
            {
                return path;
                throw new Exception("LLLOOPP");
            }*/
            int prevNode = parents[curNode];
            if (prevNode == start) 
                break;

            
            path.Add(prevNode);
            curNode = prevNode;
        }
        path.Add(start);
        path.Reverse();

        return path;
    }

    private static List<int> TraverseTo(float[][] graph, int startNode, int endNode)
    {
        List<int> queue = new List<int>();
        List<int> closedNodes = new List<int>();
        Dictionary<int, int> parents = new Dictionary<int, int>();
        queue.Add(startNode);
        int prevNode = -1;
        while (queue.Count > 0)
        {
            int currentNode = queue[0];
            queue.RemoveAt(0);
            if (currentNode == endNode)
                return Backtrace(startNode, endNode, parents);
            List<int> neigbours = Neigbours(graph, currentNode, prevNode).Where(x => !closedNodes.Contains(x)).ToList();
            foreach (int v in neigbours)
            {
                if (!queue.Contains(v))
                {
                    parents[v] = currentNode;
                    //parents[currentNode] = v;
                    queue.Add(v);
                }
            }
            closedNodes.Add(currentNode);
            prevNode = currentNode;
        }
        
        throw new Exception("Disconected Graphs");
    }

    private static int[] GetLeafes(float[][] graph)
    {
        List<int> leaves = new List<int>();
        for (int i = 0; i < graph.Length; i++)
        {
            int edgesCount = 0;
            for (int j = 0; j < graph.Length; j++)
            {
                if (graph[i][j] > 0)
                    edgesCount++;
            }
            if (edgesCount <= 1)
                leaves.Add(i);
        }
        return leaves.ToArray();
    }

    private static float[][] CreateLoopsInGroup(float[][] graph, float[][] distGraph, List<int> group)
    {
        int edges = group.Count % 5;
        //int edges = 1;
        int ec = 100;
        if (group.Count <= 2)
            return graph;
        if (group.Count <= 5)
            edges = 1;
        while (edges > 0)
        {
            if (--ec < 0) { Debug.LogError(Utils.ListRepr(group) + " - " + group.Count % 5) ; break; }
            int firstNode = Utils.RandomChoise(group);
            int secondNode = Utils.RandomChoise(group.Where(x => x != firstNode).ToList());
            //Debug.Log(firstNode + " - " + secondNode);
            if (graph[firstNode][secondNode] < 0)
            {
                graph[firstNode][secondNode] = distGraph[firstNode][secondNode];
                graph[secondNode][firstNode] = distGraph[firstNode][secondNode];
                edges--;
            }
        }

        return graph;
    }

    private static float[][] ConnectGroups(float[][] graph, float[][] distGraph, List<Tuple<List<int>, Vector2>> groups)
    {
        int edgesCount = 0;
        List<Tuple<List<int>, Vector2>> disconectedGroups = new List<Tuple<List<int>, Vector2>>(groups);
        int ec = 1000;
        while (edgesCount < groups.Count-1 && disconectedGroups.Count > 0)
        {
            if (ec-- < 0)
            {
                Debug.LogError("LOOP");
                break;
            }
            Tuple<List<int>, Vector2> firstGroup = Utils.RandomChoise(disconectedGroups);

            Tuple<List<int>, Vector2> secondGroup = disconectedGroups
                .OrderBy(x => Vector2.Distance(firstGroup.Item2, x.Item2))
                .Where(x => x.Item1 != firstGroup.Item1).First();
            float shortestDist = int.MaxValue;
            Tuple<int, int> edge = new Tuple<int, int>(firstGroup.Item1[0], secondGroup.Item1[0]);
            for (int i = 0; i < firstGroup.Item1.Count; i++)
            {
                for (int j = 0; j < secondGroup.Item1.Count; j++)
                {
                    int firstNode = firstGroup.Item1[i];
                    int secondNode = secondGroup.Item1[j];
                    if (distGraph[firstNode][secondNode] < shortestDist && graph[firstNode][secondNode] < 0)
                    {
                        shortestDist = distGraph[firstNode][secondNode];
                        edge = new Tuple<int, int>(firstNode, secondNode);
                    }
                }
            }
            graph[edge.Item1][edge.Item2] = distGraph[edge.Item1][edge.Item2];
            graph[edge.Item2][edge.Item1] = distGraph[edge.Item1][edge.Item2];
            //disconectedGroups.Remove(firstGroup);
            //disconectedGroups.Remove(secondGroup);
            edgesCount++;
        }
        return graph;
    }



    private static float[][] BuildConsecventive(float[][] distanceGraph, Room[] rooms, int startRoom, int endRoom)
    {
        int roomsNumber = rooms.Length;
        float[][] graph = new float[roomsNumber][];
        for (int i = 0; i < roomsNumber; i++)
        {
            graph[i] = new float[roomsNumber];
            for (int j = 0; j < roomsNumber; j++)
                graph[i][j] = -1;
        }
        List<int> visited = new List<int>() { startRoom };
        int currentNode = startRoom;
        int edgesCount = 0;
        while (edgesCount != roomsNumber-1)
        {
            int[] unvisitedNeigbours;
            if (edgesCount < roomsNumber-2)
                unvisitedNeigbours = GetNClosestNode(currentNode, distanceGraph, roomsNumber).Where(x => !visited.Contains(x) && x != currentNode && x != endRoom).ToArray();
            else
                unvisitedNeigbours = GetNClosestNode(currentNode, distanceGraph, roomsNumber).Where(x => !visited.Contains(x) && x != currentNode).ToArray();
            
            if (unvisitedNeigbours.Length == 0)
                throw new Exception("Error");

            graph[currentNode][unvisitedNeigbours[0]] = distanceGraph[currentNode][unvisitedNeigbours[0]];
            graph[unvisitedNeigbours[0]][currentNode] = distanceGraph[currentNode][unvisitedNeigbours[0]];
            visited.Add(currentNode);
            currentNode = unvisitedNeigbours[0];
            edgesCount++;
        }

        return graph;

    }

    private static List<int[]> GetAllHubGroups(float[][] graph, int[] leafes, int hubIdx)
    {
        //Debug.Log("LE" + Utils.ArrayRepr(leafes));
        List<int[]> groups = new List<int[]>();
        for (int i = 0; i < leafes.Length; i++)
        {
            int currentNode = leafes[i];
            List<int> nodes = new List<int>() { currentNode };
            for (int j = 0; j < graph.Length; j++)
            {
                if (graph[currentNode][j] > 0)
                {
                    //Debug.Log(currentNode + " --> " + j);
                    currentNode = j;
                    if (currentNode == hubIdx)
                    {
                        groups.Add(nodes.ToArray());

                    }
                    nodes.Add(currentNode);
                }
            }
        }
        return groups;
    }

    private static float[][] BuildHubConnection(float[][] distanceGraph, Room[] rooms, Room[] allRooms, Room hub, int hubEdges)
    {
        int roomsNumber = allRooms.Length;
        int hubIdx = roomsNumber - 1;
        float[][] graph = new float[roomsNumber][];
        for (int i = 0; i < roomsNumber; i++)
        {
            graph[i] = new float[roomsNumber];
            for (int j = 0; j < roomsNumber; j++)
                graph[i][j] = -1;
        }
        int[] closest = GetNClosestNode(hubIdx, distanceGraph, hubEdges);
        closest = closest.Where(x => allRooms[x] != hub).ToArray();
        //for (int i = 0; i < edges; i++)
        for (int i = 0; i < closest.Length; i++)
        {
            graph[hubIdx][closest[i]] = distanceGraph[hubIdx][closest[i]];
            graph[closest[i]][hubIdx] = distanceGraph[hubIdx][closest[i]];
        }

        List<Tuple<int, int, float>> edges = GetGraphEdges(distanceGraph, new List<int> { allRooms.Length - 1 });
        graph = BuildMinimalSpaningTree(roomsNumber - (hubEdges+1), roomsNumber, edges, graph, false);

        return graph;
    }

    private static Corridor CreateCorridor(LevelGrid grid, Room room1, Room room2, bool avoidIntersection, bool straightPath)
    {
        Tuple<List<GridTile>, List<GridTile>> path = CreatePathBetweenRooms(grid, room1, room2, avoidIntersection, straightPath);
        Corridor corridor = new Corridor(path.Item1, path.Item2, room1, room2);
        return corridor;
    }


    private static Tuple<List<GridTile>, List<GridTile>> CreatePathBetweenRooms(LevelGrid grid, Room room1, Room room2, bool avoidIntersection, bool straightPath)
    {
        Vector2 target = (room1.GetRoomCenterInGridCoordinates() + room2.GetRoomCenterInGridCoordinates()) / 2;
        EntranceTile startEntrance = room1.GetClosestEntrancePoint(grid, target);
        EntranceTile endEntrance = room2.GetClosestEntrancePoint(grid, target);

        startEntrance.Connect(room2);
        endEntrance.Connect(room1);
       

        GridTile start = room1.FromLocalToGrid(grid, startEntrance);
        GridTile end = room2.FromLocalToGrid(grid, endEntrance);
        start = grid.FromRoomToCorridorMap(start);
        end = grid.FromRoomToCorridorMap(end);

        Tuple<List<GridTile>, List<GridTile>> path = PathFinding.AStar(grid, start, end, avoidIntersection, straightPath);
        return path;
    }

    private static void ChooseTag(LevelGrid grid, GridTile tile)
    {
        if (tile.tag.IsFloor())
            return;

        Vector2Int up = new Vector2Int(0, -1);
        Vector2Int right = new Vector2Int(1, 0);
        Vector2Int down = new Vector2Int(0, 1);
        Vector2Int left = new Vector2Int(-1, 0);
        GridTile upTile = grid.GetCorridorMapTile(tile.gridCoordinates + up);
        GridTile rightTile = grid.GetCorridorMapTile(tile.gridCoordinates + right);
        GridTile downTile = grid.GetCorridorMapTile(tile.gridCoordinates + down);
        GridTile leftTile = grid.GetCorridorMapTile(tile.gridCoordinates + left);
        int counter = 0;
        bool floorUp = upTile.tag.IsFloor();
        bool floorRight = rightTile.tag.IsFloor();
        bool floorDown = downTile.tag.IsFloor();
        bool floorLeft = leftTile.tag.IsFloor();

        counter += floorUp ? 1 : 0;
        counter += floorRight ? 1 : 0;
        counter += floorDown ? 1 : 0;
        counter += floorLeft ? 1 : 0;

        TileRotation rotation;
        TileAlignment alignment;
        if (counter == 4)
        {

        }
        else if (counter == 3)
        {
            if (!floorUp)
                rotation = TileRotation.down;
            else if (!floorRight)
                rotation = TileRotation.left;
            else if (!floorDown)
                rotation = TileRotation.up;
            else
                rotation = TileRotation.right;

            tile.SetTag(new TileTag(TileMap.corridor, TileType.tripleWall, TileAlignment.center, rotation));
        }
        else if (counter == 2)
        {
            TileType type = TileType.cornerWall;
            if (floorUp && floorRight)
            {
                rotation = TileRotation.right;
                alignment = TileAlignment.right;
            }
            else if (floorUp && floorLeft)
            {
                rotation = TileRotation.up;
                alignment = TileAlignment.up;
            }
            else if (floorUp && floorDown)
            {
                rotation = TileRotation.up;
                alignment = TileAlignment.center;
                type = TileType.parallelWall;
            }
            else if (floorRight && floorDown)
            {
                rotation = TileRotation.down;
                alignment = TileAlignment.down;
            }
            else if (floorRight && floorLeft)
            {
                rotation = TileRotation.right;
                alignment = TileAlignment.center;
                type = TileType.parallelWall;
            }
            else// if (floorDown && floorLeft)
            {
                rotation = TileRotation.left;
                alignment = TileAlignment.left;
            }

            tile.SetTag(new TileTag(TileMap.corridor, type, alignment, rotation));
        }
        else //if (counter == 1)
        {
            if (floorUp)
            {
                rotation = TileRotation.up;
                alignment = TileAlignment.up;
            }
            else if (floorRight)
            {
                rotation = TileRotation.right;
                alignment = TileAlignment.right;
            }
            else if (floorDown)
            {
                rotation = TileRotation.down;
                alignment = TileAlignment.down;
            }
            else
            {
                rotation = TileRotation.left;
                alignment = TileAlignment.left;
            }

            tile.SetTag(new TileTag(TileMap.corridor, TileType.wall, alignment, rotation));
        }
    }

    private static void PatternMatching(LevelGrid grid, GridTile cur)
    {
        Vector2Int up = new Vector2Int(0, -1);
        Vector2Int right = new Vector2Int(1, 0);
        Vector2Int down = new Vector2Int(0, 1);
        Vector2Int left = new Vector2Int(-1, 0);
        GridTile upTile = grid.GetCorridorMapTile(cur.gridCoordinates + up);
        GridTile rightTile = grid.GetCorridorMapTile(cur.gridCoordinates + right);
        GridTile downTile = grid.GetCorridorMapTile(cur.gridCoordinates + down);
        GridTile leftTile = grid.GetCorridorMapTile(cur.gridCoordinates + left);
        //Add null check

        cur.SetTag(new TileTag(
                    TileMap.corridor,
                    TileType.floor,
                    TileAlignment.center,
                    TileRotation.identity
                ));

        ChooseTag(grid, upTile);
        ChooseTag(grid, rightTile);
        ChooseTag(grid, downTile);
        ChooseTag(grid, leftTile);

    }

    private static void GenerateCorridorsFromGraph(LevelGrid grid, float[][] graph, Room[] rooms, PrefabsSet corridorsPrefabsSet,
        bool avoidIntersection=false, bool straightPath=true, int startFrom = 0)
    {
        for (int i = 0; i < graph.GetLength(0); i++)
        {
            //for (int j = i + 1; j < graph.GetLength(0); j++)
            for (int j = 0; j < i; j++)
            {
                if (i >= startFrom && graph[i][j] > 0)
                {
                    Corridor corridor = CreateCorridor(grid, rooms[i], rooms[j], avoidIntersection, straightPath);
                    grid.AddCorridor(corridor);
                }
            }
        }
        SetUpCorridorsWallsAndPrefabs(grid, corridorsPrefabsSet);
    }

    private static void SetUpCorridorsWallsAndPrefabs(LevelGrid grid, PrefabsSet corridorsPrefabsSet)
    {
        foreach (Corridor corridor in grid.corridors)
        {
            for (int i = 0; i < corridor.floorTiles.Count; i++)
            {
                GridTile cur = corridor.floorTiles[i];
                PatternMatching(grid, cur);
            }
        }
        for (int i = 0; i < grid.corridors.Count; i++)
        {
            Corridor corridor = grid.corridors[i];
            WrappedObject[] floorPrefabs = corridorsPrefabsSet.floorTiles;
            WrappedObject[] wallTiles = corridorsPrefabsSet.wallTiles;
            WrappedObject[] cornerWallTiles = corridorsPrefabsSet.cornerWallTiles;
            WrappedObject[] parallelWallTiles = corridorsPrefabsSet.parallelWallTiles;
            WrappedObject[] tripleWallTiles = corridorsPrefabsSet.tripleWallTiles;
            CorridorsGenerator.SetUpPrefabs(corridor, floorPrefabs, wallTiles, cornerWallTiles, parallelWallTiles, tripleWallTiles);
        }
    }

    private static void SetUpPrefabs(Corridor corridor, WrappedObject[] floorPrefabs, WrappedObject[] wallPrefabs,
        WrappedObject[] cornerWallPrefabs, WrappedObject[] parallelWallPrefabs, WrappedObject[] triplelWallPrefabs)
    {
        for (int i = 0; i < corridor.floorTiles.Count; i++)
        {
            if (corridor.floorTiles[i].tag.IsFloor())
            {
                corridor.floorTiles[i].SetPrefab(Utils.RandomChoise(floorPrefabs));
            }

            if (i * 2 >= corridor.wallTiles.Count)
                continue;

            GridTile wall1 = corridor.wallTiles[i * 2];
            GridTile wall2 = corridor.wallTiles[i * 2 + 1];
            if (wall1.tag.type == TileType.wall)
            {
                wall1.SetPrefab(Utils.RandomChoise(wallPrefabs));
            }
            else if (wall1.tag.type == TileType.cornerWall)
            {
                wall1.SetPrefab(Utils.RandomChoise(cornerWallPrefabs));
            }
            else if (wall1.tag.type == TileType.parallelWall)
            {
                wall1.SetPrefab(Utils.RandomChoise(parallelWallPrefabs));
            }
            else if (wall1.tag.type == TileType.tripleWall)
            {
                wall1.SetPrefab(Utils.RandomChoise(triplelWallPrefabs));
            }

            if (wall2.tag.type == TileType.wall)
            {
                wall2.SetPrefab(Utils.RandomChoise(wallPrefabs));
            }
            else if (wall2.tag.type == TileType.cornerWall)
            {
                wall2.SetPrefab(Utils.RandomChoise(cornerWallPrefabs));
            }
            else if (wall2.tag.type == TileType.parallelWall)
            {
                wall2.SetPrefab(Utils.RandomChoise(parallelWallPrefabs));
            }
            else if (wall2.tag.type == TileType.tripleWall)
            {
                wall2.SetPrefab(Utils.RandomChoise(triplelWallPrefabs));
            }
        }
    }

    public static float[][] GenerateSpaningTreeConnection(LevelGrid grid, Room[] rooms, PrefabsSet corridorsPrefabsSet, bool straightPath)
    {
        float[][] distGraph = CreateDistancesGraph(rooms);
        float[][] graph = BuildMinimalSpaningTree(rooms.Length, GetGraphEdges(distGraph));
        GenerateCorridorsFromGraph(grid, graph, rooms, corridorsPrefabsSet, false, straightPath);
        return graph;
    }

    public static float[][] GenerateMainPathConnection(LevelGrid grid, Room[] mainRooms, Room startRoom, Room endRoom,
        Room[] allRooms, PrefabsSet corridorsPrefabsSet, bool straightPath)
    {
        //int startRoomIdx = mainRooms.Select((v, i) => new { car = v, index = i }).First(myCondition).index;
        int startRoomIdx = mainRooms.Select((room, index) => (room, index)).First(pair => pair.room == startRoom).index;
        int endRoomIdx = mainRooms.Select((room, index) => (room, index)).First(pair => pair.room == endRoom).index;
        float[][] distGraph = CreateDistancesGraph(mainRooms);
        float[][] graph = BuildConsecventive(distGraph, mainRooms, startRoomIdx, endRoomIdx);
        distGraph = CreateDistancesGraph(allRooms);

        for (int i = 0; i < distGraph.Length; i++)
        {
            for (int j = 0; j < distGraph.Length; j++)
            {
                if (j == endRoomIdx || i == endRoomIdx)
                    distGraph[i][j] = -1;
            }
        }
        graph = BuildMinimalSpaningTree(allRooms.Length - mainRooms.Length, mainRooms.Length, GetGraphEdges(distGraph), graph);
        GenerateCorridorsFromGraph(grid, graph, allRooms, corridorsPrefabsSet, false, straightPath);

        return graph;
    }

    private static Vector2 CalculateAverageGroupPosition(Room[] rooms, List<int> group)
    {
        return group.Select(x => rooms[x].gridCoordinates).Aggregate(Vector2Int.zero, (prod, next) => prod + next) / rooms.Length;
    }

    public static float[][] GenerateHubConnection(LevelGrid grid, Room[] rooms, Room hub, PrefabsSet corridorsPrefabsSet, int connectedToHub, bool straightPath)
    {
        Room[] allRooms = rooms.Concat(new Room[] { hub } ).ToArray();
        int hubIdx = allRooms.Length - 1;
        float[][] distGraph = CreateDistancesGraph(allRooms);
        float[][] graph = BuildHubConnection(distGraph, rooms, allRooms, hub, connectedToHub);


        int[] leafes = GetLeafes(graph);
        //List<int>[] paths = new List<int>[leafes.Length];
        List<List<int>> groups = new List<List<int>>();
        
        foreach (int l in leafes) 
        {
            bool haveIntersection = false;
            List<int> group = TraverseTo(graph, l, hubIdx);
            for (int i = 0; i < groups.Count; i++)
            {
                if (groups[i].Intersect(group).Any())
                {
                    groups[i].AddRange(group.Except(groups[i]));
                    haveIntersection = true;
                    break;
                }
                    
            }
            if (!haveIntersection)
                groups.Add(group);
        }
        float procent = 0.5f;
        Tuple<List<int>, Vector2>[] groupsWithCoordinates = groups.Select(x => new Tuple<List<int>, Vector2>(x, CalculateAverageGroupPosition(rooms, x))).ToArray();
        List<Tuple<List<int>, Vector2>> smalestGroups = groupsWithCoordinates.OrderBy(x => x.Item1.Count).Take((int)(groups.Count * procent)).ToList();
        //List<List<int>> smalestGroups = groups.OrderBy(x => x.Count).Take((int)(groups.Count * procent)).ToList();

        /*foreach (List<int> g in smalestGroups)
        {
            Debug.Log(Utils.ListRepr(g));
        }*/

        graph = ConnectGroups(graph, distGraph, smalestGroups);
        foreach (List<int> g in groupsWithCoordinates.Except(smalestGroups).Select(x => x.Item1))
        {
            graph = CreateLoopsInGroup(graph, distGraph, g);
        }


        //List<int[]> groups = GetAllHubGroups(graph, leafes, hubIdx);

        /*Debug.Log("GRPOUPS " + groups.Count);
        foreach (int[] group in groups)
        {
            string groupStr = "";
            foreach (int idx in group)
            {
                groupStr += idx + ", ";
            }
            Debug.Log(groupStr);
        }*/


        GenerateCorridorsFromGraph(grid, graph, allRooms, corridorsPrefabsSet, false, straightPath);

        return graph;
    }
}
