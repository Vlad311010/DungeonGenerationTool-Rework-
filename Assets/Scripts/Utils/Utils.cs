using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;

public static class Utils
{
    public static T RandomChoise<T>(List<T> list)
    {
        int idx = Random.Range(0, list.Count);
        return list[idx];
    }

    public static T RandomChoise<T>(T[] list)
    {
        if (list.Length == 0)
            throw new System.ArgumentException("Error:Trying to get random element empty list");

        int idx = Random.Range(0, list.Length);
        return list[idx];
    }

    public static string ListRepr<T>(List<T> list)
    {
        string listStr = "[";
        for (int i = 0; i < list.Count; i++)
        {
            listStr += list[i] + ", ";
        }
        listStr += "]";
        return listStr;
    }

    public static string ArrayRepr<T>(T[] array)
    {
        string listStr = "[";
        for (int i = 0; i < array.Length; i++)
        {
            listStr += array[i] + ", ";
        }
        listStr += "]";
        return listStr;
    }


    public static Vector2Int RandomVector2Int(Vector2Int minSize, Vector2Int maxSize)
    {
        return new Vector2Int(Random.Range(minSize.x, maxSize.x), Random.Range(minSize.y, maxSize.y));
    }

    public static int Size(Vector2Int vector)
    {
        return vector.x * vector.y;
    }


    public static void ShowGraph<T>(T[][] graph)
    {
        string row;
        for (int i = 0; i < graph.Length; i++)
        {
            row = i + "-- ";
            for (int j = 0; j < graph[i].Length; j++)
            {
                row += j + ":[" + graph[i][j] + "], ";
            }
            Debug.Log(row);
        }
    }

    public static void ClearLog()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }

    public static string UniteToString(params object[] args)
    {
        string str = "";
        for (int i = 0; i < args.Length; i++)
        {
            str += args[i].ToString() + " ";
        }
        return str;
    }


}
