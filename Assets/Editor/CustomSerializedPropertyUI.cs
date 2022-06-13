using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class CustomSerializedPropertyUI
{
    private static GUIContent
        moveButtonContent = new GUIContent("\u21b4", "move down"),
        duplicateButtonContent = new GUIContent("+", "duplicate"),
        deleteButtonContent = new GUIContent("-", "delete");

    private static GUILayoutOption miniButtonWidth = GUILayout.Width(20f);

    public static void ShowArray(SerializedProperty array, bool showButtons=true)
    {
        if (!array.isArray)
        {
            EditorGUILayout.HelpBox(array.name + " is neither an array nor a list!", MessageType.Error);
            return;
        }
        array.isExpanded = EditorGUILayout.Foldout(array.isExpanded, array.displayName);
        EditorGUILayout.LabelField("Size", array.FindPropertyRelative("Array.size").intValue.ToString());
        if (array.isExpanded)
        {
            EditorGUI.indentLevel += 1;
            if (array.isExpanded)
            {
                for (int i = 0; i < array.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(array.GetArrayElementAtIndex(i));
                }
            }
            EditorGUI.indentLevel -= 1;
        }
        if (showButtons)
        {
            EditorGUILayout.BeginHorizontal();
            ShowButtons(array);
            EditorGUILayout.EndHorizontal();
        }
    }

    private static void ShowButtons(SerializedProperty array)
    {
        int oldSize = array.arraySize;
        if (GUILayout.Button(duplicateButtonContent, EditorStyles.miniButtonLeft, miniButtonWidth))
        {
            array.InsertArrayElementAtIndex(Mathf.Max(0, array.arraySize-1));
        }
        if (GUILayout.Button(deleteButtonContent, EditorStyles.miniButtonRight, miniButtonWidth))
        {
            if (array.arraySize == 0)
                return;

            array.DeleteArrayElementAtIndex(array.arraySize-1);
            if (array.arraySize == oldSize) { array.DeleteArrayElementAtIndex(array.arraySize-1); }
        }
    }


}
