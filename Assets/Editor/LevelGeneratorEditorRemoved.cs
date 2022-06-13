using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections.Generic;

public class LevelGeneratorEditorRemoved : MonoBehaviour
{
    [CustomEditor(typeof(LevelGeneratorRemoved))]
    public class LevelScriptEditor : Editor
    {
        LevelGeneratorRemoved script;

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
            script = (LevelGeneratorRemoved)target;

            customRoomsList = CustomRoomData.GetAllCustomRoomsNames();

            roomPrefabField = new AnimBool(true);

            headStyle.fontSize = 15;
            headStyle.normal.textColor = Color.white;

            minRoomSelectedValue = script.minRoomsAmount;
            maxRoomSelectedValue = script.maxRoomsAmount;
            
            selectedCustomRooms = new List<int>();
        }

        private void DrawGridPart()
        {
            //grid parameters
            GUILayout.Label("Grid Parameters", headStyle);
            EditorGUI.indentLevel++;
            script.center = EditorGUILayout.Vector3Field("Grid Center", script.center);
            script.cellSize = EditorGUILayout.FloatField("Cell Size", script.cellSize);
            script.seed = EditorGUILayout.IntField("Random Seed", script.seed);
            EditorGUI.indentLevel--;
            EditorGUILayout.Separator();
        }

        private void DrawRoomsPart()
        {
            // rooms parameters
            GUILayout.Label("Rooms Generator Parameters", headStyle);
            EditorGUI.indentLevel++;
            script.minimumRandomRoomSize = EditorGUILayout.Vector2IntField("Minimum Room Size", script.minimumRandomRoomSize);
            script.maximumRandomRoomSize = EditorGUILayout.Vector2IntField("Maximum Room Size", script.maximumRandomRoomSize);

            //maxRoomAmount = 100;
            maxRoomAmount = 200;
            //Vector2Int rSize = script.CalculateRoomMaximumSize(script.minimumRoomSize + script.partitionAdditionalSize, script.size, script.maxRoomAmount);
            //Debug.Log(rSize);

            EditorGUILayout.LabelField("Number of Rooms");
            EditorGUILayout.MinMaxSlider(ref minRoomSelectedValue, ref maxRoomSelectedValue,
                minRoomAmount, maxRoomAmount);
            script.minRoomsAmount = (int)minRoomSelectedValue;
            script.maxRoomsAmount = (int)maxRoomSelectedValue;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Minimum: " + script.minRoomsAmount);
            EditorGUILayout.LabelField("Maximum: " + script.maxRoomsAmount);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Number of Entry Poinr Per Room");
            EditorGUILayout.MinMaxSlider(ref entryPointMinSelectedValue, ref entryPointMaxSelectedValue,
                roomEntryPointMin, roomEntryPointMax);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
            EditorGUILayout.Separator();
        }

        private void DrawSerializedProperty(string propertyStr)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyStr);
            EditorGUILayout.PropertyField(property, true);
        }

        private void DrawPrefabsGroup()
        {
            EditorGUI.BeginChangeCheck();

            //SerializedProperty roomPrefabsProperty = serializedObject.FindProperty("defaultRoomPrefabsSets");
            //CustomSerializedPropertyUI.ShowArray(roomPrefabsProperty);
            DrawSerializedProperty("defaultRoomPrefabsSets");
            DrawSerializedProperty("corridorsPrefabsSet");

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }

        private void DrawDebugGroup()
        {
            DrawSerializedProperty("highLight");

            if (GUILayout.Button("Show Tile Info"))
            {
                script.ShowHighLightedTileInfo();
            }

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }

        private void DrawCustomRoomsPart()
        {
            serializedObject.Update();
            SerializedProperty property = serializedObject.FindProperty("customRoomPrefabsSets");
            EditorGUI.BeginChangeCheck();
            //EditorGUILayout.PropertyField(property, true);
            CustomSerializedPropertyUI.ShowArray(property, false);
            selecedCustomRoom = EditorGUILayout.Popup(selecedCustomRoom, customRoomsList);
            bool addCustomRoomButton = GUILayout.Button("Add Custom Room");
            if (addCustomRoomButton)
            {
                property.InsertArrayElementAtIndex(Mathf.Max(0, property.arraySize - 1));
                property.GetArrayElementAtIndex(property.arraySize - 1).FindPropertyRelative("roomName").stringValue = customRoomsList[selecedCustomRoom];
            }
            if (GUILayout.Button("Remove Custom Room"))
            {
                if (property.arraySize == 0)
                    return;

                property.DeleteArrayElementAtIndex(property.arraySize - 1);
            }

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }



        private void DrawBasicElements()
        {

            /*DrawGridPart();
            DrawRoomsPart();
            DrawPrefabsGroup();
            DrawCustomRoomsPart();*/
            DrawDebugGroup();


            //DrawOneRoomPrefabs("Room1");
            //DrawOneRoomPrefabs("Room2");
            /*roomPrefabField.target = EditorGUILayout.ToggleLeft("Show Room Field", roomPrefabField.target);
            using (EditorGUILayout.FadeGroupScope group = new EditorGUILayout.FadeGroupScope(roomPrefabField.faded))
            {
                if (group.visible)
                {
                    DrawOneRoomPrefabs();
                }
            }*/
            //setting
            DrawSerializedProperty("generationSettings");
            serializedObject.ApplyModifiedProperties();
            //default
            GUILayout.Space(50);
            //DrawDefaultInspector();
        }

        public override void OnInspectorGUI()
        {

            DrawBasicElements();
            /*serializedObject.Update();
            //CustomSerializedPropertyUI.ShowArray(serializedObject.FindProperty("testField"));
            CustomSerializedPropertyUI.ShowArray(serializedObject.FindProperty("defaultRoomPrefabsSets"));
            serializedObject.ApplyModifiedProperties();*/

            /*GUI.backgroundColor = Application.isPlaying ? Color.white : Color.red;
            if (GUILayout.Button("Generate") && Application.isPlaying)
            {
                UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
                script.Generate();
                //Utils.ClearLog();
             
            }
            if (GUILayout.Button("Generate With Hub") && Application.isPlaying)
            {
                UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
                script.GenerateHub();
            }
            if (GUILayout.Button("Generate With Main Path") && Application.isPlaying)
            {
                UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
                script.GenerateMainPath();
            }*/


            /*if (GUILayout.Button("Show Tile Info") && Application.isPlaying)
            {
                UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
                script.ShowTileInfo(script.highLight);

            }*/
            GUI.backgroundColor = Color.white;

            //DrawDefaultInspector();
            //EditorGUILayout.HelpBox("Generate button works only on runtime\n" + "partitions max: " + maxPartitions, MessageType.Info);
        }
    }
}
