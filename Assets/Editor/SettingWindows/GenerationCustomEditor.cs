using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class DRGenerationHandler
{
    //Makes it able to double click to open window
    [OnOpenAsset()]
    public static bool OpenEditor(int instanceID, int line)
    {
        GenerationSettings obj = EditorUtility.InstanceIDToObject(instanceID) as GenerationSettings;

        if (obj != null)
        {
            SettingsWindow.OpenWindow(obj);
            return true;
        }

        return false;
    }
}

[CustomEditor(typeof(GenerationSettings))]
public class DRGenerationCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open Edit Mode"))
        {
            SettingsWindow.OpenWindow((GenerationSettings)target);
        }
    }
}
