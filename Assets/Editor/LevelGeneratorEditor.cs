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
        GUIStyle headStyle = new GUIStyle();

        

        private void OnEnable()
        {
            script = (LevelGenerator)target;

            headStyle.fontSize = 15;
            headStyle.normal.textColor = Color.white;
        }

            
         
        private void DrawEditor()
        {
            SerializedProperty generationSettings = serializedObject.FindProperty("generationSettings");
            EditorGUILayout.PropertyField(generationSettings, true);
            serializedObject.ApplyModifiedProperties();

        }

        public override void OnInspectorGUI()
        {
            
            DrawEditor();

            if (GUILayout.Button("Generate"))
                script.Generate();

            /*if (GUILayout.Button("Generate With Hub"))
            {
                UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
                script.GenerateHub();
            }
            if (GUILayout.Button("Generate With Main Path"))
            {
                UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
                script.GenerateMainPath();
            }*/


        }
    }
}
