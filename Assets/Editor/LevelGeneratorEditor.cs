using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections.Generic;

public class LevelGeneratorEditor : MonoBehaviour
{
    [CustomEditor(typeof(LevelGenerator))]
    public class LevelScriptEditor : Editor
    {
        LevelGenerator script;

        readonly int minRoomAmount = 0;
        int maxRoomAmount = 5;
        float minRoomSelectedValue = 0;
        float maxRoomSelectedValue = 0;

        readonly int roomEntryPointMin = 1;
        int roomEntryPointMax = 5;
        float entryPointMinSelectedValue = 1;
        float entryPointMaxSelectedValue = 3;

        string[] customRoomsList;
        List<int> selectedCustomRooms;
        int selecedCustomRoom;


        GUIStyle headStyle = new GUIStyle();

        AnimBool roomPrefabField;

        private void OnEnable()
        {
            script = (LevelGenerator)target;

            headStyle.fontSize = 15;
            headStyle.normal.textColor = Color.white;

        }

        private void DrawSerializedProperty(string propertyStr)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyStr);
            EditorGUILayout.PropertyField(property, true);
        }

        private void DrawDebugGroup()
        {
            /*DrawSerializedProperty("highLight");

            if (GUILayout.Button("Show Tile Info"))
            {
                script.ShowHighLightedTileInfo();
            }*/

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }


        private void DrawEditor()
        {
            DrawSerializedProperty("generationSettings");
            serializedObject.ApplyModifiedProperties();

            DrawDebugGroup();

            //GUILayout.Space(50);
            //DrawDefaultInspector();
        }

        public override void OnInspectorGUI()
        {

            DrawEditor();

            //GUI.backgroundColor = Application.isPlaying ? Color.white : Color.red;
            /*if (GUILayout.Button("Generate") && Application.isPlaying)
            {
                UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
                script.Generate();
            }*/
            if (GUILayout.Button("Generate With Hub"))
            {
                UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
                script.GenerateHub();
            }
            if (GUILayout.Button("Generate With Main Path"))
            {
                UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
                script.GenerateMainPath();
            }

            //GUI.backgroundColor = Color.white;

        }
    }
}
