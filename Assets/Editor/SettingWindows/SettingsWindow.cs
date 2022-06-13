using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class SettingsWindow : EditorWindow
{
    private SerializedObject serializedObject;

    // layout parameters
    const float sectionsOffset = 5f;
    const float borderOffset = 7f;
    const float propertyFieldHeight = 18;

    private Color headerSectionColor = new Color(115f / 255f, 115f / 255f, 115f / 255f); // 13f / 255f, 32f / 255f, 44f / 255f
    private Color tabsSectionColor = new Color(115f / 255f, 115f / 255f, 115f / 255f);
    private Color editorSectionColor = new Color(56f / 255f, 56f / 255f, 56f / 255f); //105f / 255f, 105f / 255f, 105f / 255f


    //private Color headerSectionColor = new Color(0f / 255f, 255f / 255f, 44f / 255f);
    //private Color tabsSectionColor = new Color(255f / 255f, 115f / 255f, 115f / 255f);
    //private Color editorSectionColor = new Color(105f / 255f, 105f / 255f, 255f / 255f);

    private Rect headerSection;
    private Rect tabsSection;
    private Rect editorSection;
    
    private Texture2D headerSectionTextrue;
    private Texture2D tabsSectionTexture;
    private Texture2D editorSectionTextrue;

    static Vector2 windowSize = new Vector2(900, 685);
    static int selecedCustomRoom = 0;
    Vector2 scrollPos;

    // params
    static SettingsWindow window;
    private static string settingsName;
    private static SerializedObject lastOpenedSetting;

    const float fieldsGap = 22;

    //properties
    //default properties
    SerializedProperty center;
    SerializedProperty cellSize;
    SerializedProperty straightPath;
    SerializedProperty minRoomsAmount;
    SerializedProperty maxRoomsAmount;
    SerializedProperty minRandomRoomSize;
    SerializedProperty maxRandomRoomSize;
    SerializedProperty seed;

    //main path properties
    SerializedProperty minSideRoomsAmount;
    SerializedProperty maxSideRoomsAmount;
   
    //hub properties
    SerializedProperty hubConnection;

    //labels
    string centerLabel = "Grid Center";
    string cellSizeLabel = "Cell Size";
    string straightPathLabel = "Straight Path Building";
    string minRoomsAmountLabel = "Minimal Rooms Number";
    string maxRoomsAmountLabel = "Maximal Rooms Number";
    string minRandomRoomSizeLabel = "Minimal Random Room Size";
    string maxRandomRoomSizeLabel = "Maximal Random Room Size";
    string minSideRoomsAmountLabel = "Minimal Side Rooms Number";
    string maxSideRoomsAmountLabel = "Maximal Side Rooms Number";
    string hubConnectionLabel = "Rooms Connected To Hub";
    string seedLabel = "Generating Seed";

    public enum GeneratorType
    {
        Default, 
        MainPath,
        Hub
    }

    private enum Tabs
    {
        General,
        RandomRoomPrefabs,
        CorridorPrefabs,
        CustomTab1,
        CustomTab2
    }

    private Tabs selectedTab;
    private GeneratorType selectedType;

    public static void OpenWindow()
    {
        window = (SettingsWindow)GetWindow(typeof(SettingsWindow));
        window.serializedObject = lastOpenedSetting;
        window.maxSize = windowSize;
        window.minSize = window.maxSize;
        window.Show();
    }

    public static void OpenWindow(GenerationSettings genSetts)
    {
        window = (SettingsWindow)GetWindow(typeof(SettingsWindow));
        settingsName = genSetts.name;
        window.serializedObject = new SerializedObject(genSetts);
        window.maxSize = windowSize;
        window.minSize = window.maxSize;
        window.Show();
    }

    public void Validate()
    {
        //cellSize.floatValue = cellSize.floatValue > 0.1f ? cellSize.floatValue : 0.1f;
        minRoomsAmount.intValue = Math.Min(minRoomsAmount.intValue, maxRoomsAmount.intValue);
        maxRoomsAmount.intValue = Math.Max(minRoomsAmount.intValue, maxRoomsAmount.intValue);
        Vector2Int minRandomRoomSizeValue = minRandomRoomSize.vector2IntValue;
        Vector2Int maxRandomRoomSizeValue = maxRandomRoomSize.vector2IntValue;
        minRandomRoomSizeValue.x = Math.Min(Math.Max(4, minRandomRoomSizeValue.x), maxRandomRoomSizeValue.x);
        minRandomRoomSizeValue.y = Math.Min(Math.Max(4, minRandomRoomSizeValue.y), maxRandomRoomSizeValue.y);
        maxRandomRoomSizeValue.x = Math.Max(Math.Max(4, minRandomRoomSizeValue.x), maxRandomRoomSizeValue.x);
        maxRandomRoomSizeValue.y = Math.Max(Math.Max(4, minRandomRoomSizeValue.y), maxRandomRoomSizeValue.y);
        minSideRoomsAmount.intValue = Math.Min(minSideRoomsAmount.intValue, maxSideRoomsAmount.intValue);
        maxSideRoomsAmount.intValue = Math.Max(minSideRoomsAmount.intValue, maxSideRoomsAmount.intValue);
        minRandomRoomSize.vector2IntValue = minRandomRoomSizeValue;
        maxRandomRoomSize.vector2IntValue = maxRandomRoomSizeValue;
    }

    private Rect DrawPropertyField(Rect position, SerializedProperty property, GUIContent label)
    {
        position = EditorGUI.PrefixLabel(position, label);
        EditorGUI.PropertyField(position, property, new GUIContent(""));
        return position;
    }

    private void DrawPropertyWithScroll(Rect position, string propertyName)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        float propertyHeight = EditorGUI.GetPropertyHeight(property) + 100;

        Rect section = new Rect(position.x + 10, position.y + 10, position.width - 40, propertyHeight);
        scrollPos = GUI.BeginScrollView(
            new Rect(section.x, section.y, section.width + 25, position.height - 20),
            scrollPos,
            new Rect(section.x, section.y, section.width, propertyHeight),
            false,
            true
            );

        GUILayout.BeginArea(section);
        {
            EditorGUILayout.PropertyField(property, true);
        }
        GUILayout.EndArea();
        GUI.EndScrollView();
    }



    void DrawLayouts()
    {
        headerSection.x = borderOffset;
        headerSection.y = borderOffset;
        headerSection.width = windowSize.x - borderOffset*2;
        headerSection.height = 55;//35;

        tabsSection.x = borderOffset;
        tabsSection.y = headerSection.x + headerSection.height + sectionsOffset;
        tabsSection.width = borderOffset + 150;
        tabsSection.height = windowSize.y - headerSection.height - sectionsOffset - borderOffset;

        editorSection.x = tabsSection.x + tabsSection.width + sectionsOffset;
        editorSection.y = headerSection.x + headerSection.height + sectionsOffset;
        editorSection.width = windowSize.x - editorSection.x - borderOffset;
        editorSection.height = windowSize.y - headerSection.height - sectionsOffset - borderOffset;


        GUI.DrawTexture(headerSection, headerSectionTextrue);
        GUI.DrawTexture(tabsSection, tabsSectionTexture);
        GUI.DrawTexture(editorSection, editorSectionTextrue);
    }

    private void DrawHeader()
    {
        //var types = Enum.GetValues(typeof(GeneratorType));
        SerializedProperty type = serializedObject.FindProperty("selectedType");
        string[] names = Enum.GetNames(typeof(GeneratorType));
        selectedType = (GeneratorType)type.intValue;
        GUILayout.BeginArea(headerSection);
        {
            selectedType = (GeneratorType)EditorGUILayout.Popup((int)selectedType, names);
            GUI.Label(new Rect(headerSection.size / 2 - new Vector2(50, 7.5f), new Vector2(230, 15)), settingsName + " (" + selectedType + "Type)");
        }
        type.intValue = (int)selectedType;
        GUILayout.EndArea();
    }

    private void DrawTabs(string[] tabNames)
    {
        float borderOffset = 3f;
        Vector2 buttonSize = new Vector2(tabsSection.width - borderOffset*2, 122);
        var tabs = Enum.GetValues(typeof(Tabs));
        Debug.Assert(tabNames.Length == tabs.Length);
        GUILayout.BeginArea(tabsSection);
        {
            Vector2 selectionGridPos = new Vector2(borderOffset, borderOffset);
            Vector2 selectionGridSize = new Vector2(buttonSize.x, buttonSize.y * tabs.Length);
            selectedTab = (Tabs)GUI.SelectionGrid(new Rect(selectionGridPos, selectionGridSize), (int)selectedTab, tabNames, 1);
        }
        GUILayout.EndArea();
    }

    private void DrawEditor(List<Action> tabsContent)
    {
        var tabs = Enum.GetValues(typeof(Tabs));
        Debug.Assert(tabsContent.Count == tabs.Length);
        switch (selectedTab)
        {
            case Tabs.General:
                tabsContent[0]();
                break;
            case Tabs.RandomRoomPrefabs:
                tabsContent[1]();
                break;
            case Tabs.CorridorPrefabs:
                tabsContent[2]();
                break;
            case Tabs.CustomTab1:
                tabsContent[3]();
                break;
            case Tabs.CustomTab2:
                tabsContent[4]();
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void GetProperties()
    {
        center = serializedObject.FindProperty("center");
        cellSize = serializedObject.FindProperty("cellSize");
        straightPath = serializedObject.FindProperty("straightPath");
        minRoomsAmount = serializedObject.FindProperty("minRoomsAmount");
        maxRoomsAmount = serializedObject.FindProperty("maxRoomsAmount");
        minRandomRoomSize = serializedObject.FindProperty("minimumRandomRoomSize");
        maxRandomRoomSize = serializedObject.FindProperty("maximumRandomRoomSize");
        seed = serializedObject.FindProperty("seed");

        minSideRoomsAmount = serializedObject.FindProperty("minSideRoomsAmount");
        maxSideRoomsAmount = serializedObject.FindProperty("maxSideRoomsAmount");

        hubConnection = serializedObject.FindProperty("hubConnections");
    }

    private Rect DrawGeneralEditorDefault()
    {

        float propertyFieldWidth = editorSection.width - 50;        
        Vector2 fieldSize = new Vector2(propertyFieldWidth, propertyFieldHeight);
        
        Rect section = new Rect(editorSection.x + 10, editorSection.y + 10, editorSection.width - 40, editorSection.height);
        Rect cursor = new Rect(Vector2.zero, fieldSize);
        GUILayout.BeginArea(section);
        {
            EditorGUIUtility.labelWidth = 250;

            cursor = EditorGUI.PrefixLabel(cursor, new GUIContent(centerLabel));
            EditorGUI.PropertyField(cursor, center, new GUIContent(""));
            cursor = new Rect(new Vector2(0, cursor.y + fieldsGap), fieldSize);

            cursor = EditorGUI.PrefixLabel(cursor, new GUIContent(cellSizeLabel));
            EditorGUI.PropertyField(cursor, cellSize, new GUIContent(""));
            cursor = new Rect(new Vector2(0, cursor.y + fieldsGap), fieldSize);

            cursor = EditorGUI.PrefixLabel(cursor, new GUIContent(straightPathLabel));
            EditorGUI.PropertyField(cursor, straightPath, new GUIContent(""));
            cursor = new Rect(new Vector2(0, cursor.y + fieldsGap), fieldSize);

            cursor = EditorGUI.PrefixLabel(cursor, new GUIContent(minRoomsAmountLabel));
            EditorGUI.PropertyField(cursor, minRoomsAmount, new GUIContent(""));
            cursor = new Rect(new Vector2(0, cursor.y + fieldsGap), fieldSize);

            cursor = EditorGUI.PrefixLabel(cursor, new GUIContent(maxRoomsAmountLabel));
            EditorGUI.PropertyField(cursor, maxRoomsAmount, new GUIContent(""));
            cursor = new Rect(new Vector2(0, cursor.y + fieldsGap), fieldSize);

            cursor = EditorGUI.PrefixLabel(cursor, new GUIContent(minRandomRoomSizeLabel));
            EditorGUI.PropertyField(cursor, minRandomRoomSize, new GUIContent(""));
            cursor = new Rect(new Vector2(0, cursor.y + fieldsGap), fieldSize);

            cursor = EditorGUI.PrefixLabel(cursor, new GUIContent(maxRandomRoomSizeLabel));
            EditorGUI.PropertyField(cursor, maxRandomRoomSize, new GUIContent(""));
            cursor = new Rect(new Vector2(0, cursor.y + fieldsGap), fieldSize);

            cursor = EditorGUI.PrefixLabel(cursor, new GUIContent(seedLabel));
            EditorGUI.PropertyField(cursor, seed, new GUIContent(""));
            cursor = new Rect(new Vector2(0, cursor.y + fieldsGap), fieldSize);
        }
        GUILayout.EndArea();
        Validate();
        return cursor;
    }

    private void DrawGeneralEditorHub()
    {
        float propertyFieldWidth = editorSection.width - 50;
        Vector2 fieldSize = new Vector2(propertyFieldWidth, propertyFieldHeight);

        Rect section = new Rect(editorSection.x + 10, editorSection.y + 10, editorSection.width - 40, editorSection.height);
        Rect cursor = new Rect(Vector2.zero, fieldSize);
        GUILayout.BeginArea(section);
        {
            EditorGUIUtility.labelWidth = 250;

            cursor = EditorGUI.PrefixLabel(cursor, new GUIContent(centerLabel));
            EditorGUI.PropertyField(cursor, center, new GUIContent(""));
            cursor = new Rect(new Vector2(0, cursor.y + fieldsGap), fieldSize);

            cursor = EditorGUI.PrefixLabel(cursor, new GUIContent(cellSizeLabel));
            EditorGUI.PropertyField(cursor, cellSize, new GUIContent(""));
            cursor = new Rect(new Vector2(0, cursor.y + fieldsGap), fieldSize);

            cursor = EditorGUI.PrefixLabel(cursor, new GUIContent(straightPathLabel));
            EditorGUI.PropertyField(cursor, straightPath, new GUIContent(""));
            cursor = new Rect(new Vector2(0, cursor.y + fieldsGap), fieldSize);

            cursor = EditorGUI.PrefixLabel(cursor, new GUIContent(minRoomsAmountLabel));
            EditorGUI.PropertyField(cursor, minRoomsAmount, new GUIContent(""));
            cursor = new Rect(new Vector2(0, cursor.y + fieldsGap), fieldSize);

            cursor = EditorGUI.PrefixLabel(cursor, new GUIContent(maxRoomsAmountLabel));
            EditorGUI.PropertyField(cursor, maxRoomsAmount, new GUIContent(""));
            cursor = new Rect(new Vector2(0, cursor.y + fieldsGap), fieldSize);

            cursor = EditorGUI.PrefixLabel(cursor, new GUIContent(minRandomRoomSizeLabel));
            EditorGUI.PropertyField(cursor, minRandomRoomSize, new GUIContent(""));
            cursor = new Rect(new Vector2(0, cursor.y + fieldsGap), fieldSize);

            cursor = EditorGUI.PrefixLabel(cursor, new GUIContent(maxRandomRoomSizeLabel));
            EditorGUI.PropertyField(cursor, maxRandomRoomSize, new GUIContent(""));
            cursor = new Rect(new Vector2(0, cursor.y + fieldsGap), fieldSize);

            cursor = EditorGUI.PrefixLabel(cursor, new GUIContent(hubConnectionLabel));
            EditorGUI.IntSlider(cursor, hubConnection, 1, 8, "");
            cursor = new Rect(new Vector2(0, cursor.y + fieldsGap), fieldSize);

            cursor = EditorGUI.PrefixLabel(cursor, new GUIContent(seedLabel));
            EditorGUI.PropertyField(cursor, seed, new GUIContent(""));
            cursor = new Rect(new Vector2(0, cursor.y + fieldsGap), fieldSize);


        }
        GUILayout.EndArea();
        Validate();
    }

    private void DrawGeneralEditorMainPath()
    {
        float propertyFieldWidth = editorSection.width - 50;
        Vector2 fieldSize = new Vector2(propertyFieldWidth, propertyFieldHeight);

        Rect section = new Rect(editorSection.x + 10, editorSection.y + 10, editorSection.width - 40, editorSection.height);
        Rect cursor = new Rect(Vector2.zero, fieldSize);
        GUILayout.BeginArea(section);
        {
            EditorGUIUtility.labelWidth = 250;

            cursor = EditorGUI.PrefixLabel(cursor, new GUIContent(centerLabel));
            EditorGUI.PropertyField(cursor, center, new GUIContent(""));
            cursor = new Rect(new Vector2(0, cursor.y + fieldsGap), fieldSize);

            cursor = EditorGUI.PrefixLabel(cursor, new GUIContent(cellSizeLabel));
            EditorGUI.PropertyField(cursor, cellSize, new GUIContent(""));
            cursor = new Rect(new Vector2(0, cursor.y + fieldsGap), fieldSize);

            cursor = EditorGUI.PrefixLabel(cursor, new GUIContent(straightPathLabel));
            EditorGUI.PropertyField(cursor, straightPath, new GUIContent(""));
            cursor = new Rect(new Vector2(0, cursor.y + fieldsGap), fieldSize);

            cursor = EditorGUI.PrefixLabel(cursor, new GUIContent(minRoomsAmountLabel));
            EditorGUI.PropertyField(cursor, minRoomsAmount, new GUIContent(""));
            cursor = new Rect(new Vector2(0, cursor.y + fieldsGap), fieldSize);

            cursor = EditorGUI.PrefixLabel(cursor, new GUIContent(maxRoomsAmountLabel));
            EditorGUI.PropertyField(cursor, maxRoomsAmount, new GUIContent(""));
            cursor = new Rect(new Vector2(0, cursor.y + fieldsGap), fieldSize);

            cursor = EditorGUI.PrefixLabel(cursor, new GUIContent(minRandomRoomSizeLabel));
            EditorGUI.PropertyField(cursor, minRandomRoomSize, new GUIContent(""));
            cursor = new Rect(new Vector2(0, cursor.y + fieldsGap), fieldSize);

            cursor = EditorGUI.PrefixLabel(cursor, new GUIContent(maxRandomRoomSizeLabel));
            EditorGUI.PropertyField(cursor, maxRandomRoomSize, new GUIContent(""));
            cursor = new Rect(new Vector2(0, cursor.y + fieldsGap), fieldSize);

            cursor = EditorGUI.PrefixLabel(cursor, new GUIContent(minSideRoomsAmountLabel));
            EditorGUI.PropertyField(cursor, minSideRoomsAmount, new GUIContent(""));
            cursor = new Rect(new Vector2(0, cursor.y + fieldsGap), fieldSize);

            cursor = EditorGUI.PrefixLabel(cursor, new GUIContent(maxSideRoomsAmountLabel));
            EditorGUI.PropertyField(cursor, maxSideRoomsAmount, new GUIContent(""));
            cursor = new Rect(new Vector2(0, cursor.y + fieldsGap), fieldSize);

            cursor = EditorGUI.PrefixLabel(cursor, new GUIContent(seedLabel));
            EditorGUI.PropertyField(cursor, seed, new GUIContent(""));
            cursor = new Rect(new Vector2(0, cursor.y + fieldsGap), fieldSize);

            
        }
        GUILayout.EndArea();
        Validate();
    }

    private void DrawRandomRoomEditor()
    {
        DrawPropertyWithScroll(editorSection, "defaultRoomPrefabsSets");
    }

    private void DrawCorridorEditor()
    {
        DrawPropertyWithScroll(editorSection, "corridorsPrefabsSet");
    }


    private void DrawCustomRoomEditor()
    {
        string[] customRoomsList = CustomRoomData.GetAllCustomRoomsNames();
        EditorGUIUtility.labelWidth = 250;
        float step = 20;

        
        SerializedProperty minRoomsAmountPropert = serializedObject.FindProperty("minRoomsAmount");
        SerializedProperty maxRoomsAmountPropert = serializedObject.FindProperty("maxRoomsAmount");
        SerializedProperty property = serializedObject.FindProperty("customRoomPrefabsSets");

        int roomsAmount = 0;
        float propertyHeight = 300;
        for (int i = 0; i < property.arraySize; i++)
        {
            if (property.GetArrayElementAtIndex(i).FindPropertyRelative("generationChance").floatValue == 1f)
                roomsAmount += property.GetArrayElementAtIndex(i).FindPropertyRelative("maxAmount").intValue;
            propertyHeight += EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(i));
        }

        //float sectionHeight = (EditorGUI.GetPropertyHeight(property) + 300) * property.arraySize;
        Rect section = new Rect(editorSection.x + 10, editorSection.y + 10, editorSection.width - 40, propertyHeight);
        Rect pos = new Rect(0, 0, section.width, propertyFieldHeight);
        scrollPos = GUI.BeginScrollView(
            new Rect(section.x, section.y, section.width+25, editorSection.height-20),
            scrollPos,
            new Rect(section.x, section.y, section.width, propertyHeight),
            false, 
            true
            );
        GUILayout.BeginArea(section);
        {
            if (roomsAmount > maxRoomsAmountPropert.intValue)
            {
                EditorGUILayout.HelpBox("Number of rooms can't be greater then maximal rooms number", MessageType.Error);
                pos.y += step*2;
            }
            EditorGUI.LabelField(pos,  new GUIContent("Number of rooms: Min " + minRoomsAmountPropert.intValue + "; Max " + maxRoomsAmountPropert.intValue));
            pos.y += step;
            EditorGUI.LabelField(pos, new GUIContent("Number of rooms with 100% appearance chance: " + roomsAmount));
            pos.y += step;
            EditorGUILayout.Space(pos.y);
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
        }
        GUILayout.EndArea();
        GUI.EndScrollView();
    }

    private void DrawCustomSideRoomEditor()
    {
        string[] customRoomsList = CustomRoomData.GetAllCustomRoomsNames();
        EditorGUIUtility.labelWidth = 250;
        float step = 20;


        SerializedProperty minRoomsAmountPropert = serializedObject.FindProperty("minSideRoomsAmount");
        SerializedProperty maxRoomsAmountPropert = serializedObject.FindProperty("maxSideRoomsAmount");
        SerializedProperty property = serializedObject.FindProperty("customSideRoomPrefabsSets");

        int roomsAmount = 0;
        float propertyHeight = 300;
        for (int i = 0; i < property.arraySize; i++)
        {
            if (property.GetArrayElementAtIndex(i).FindPropertyRelative("generationChance").floatValue == 1f)
                roomsAmount += property.GetArrayElementAtIndex(i).FindPropertyRelative("maxAmount").intValue;
            propertyHeight += EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(i));
        }

        //float sectionHeight = (EditorGUI.GetPropertyHeight(property) + 300) * property.arraySize;
        Rect section = new Rect(editorSection.x + 10, editorSection.y + 10, editorSection.width - 40, propertyHeight);
        Rect pos = new Rect(0, 0, section.width, propertyFieldHeight);
        scrollPos = GUI.BeginScrollView(
            new Rect(section.x, section.y, section.width + 25, editorSection.height - 20),
            scrollPos,
            new Rect(section.x, section.y, section.width, propertyHeight),
            false,
            true
            );
        GUILayout.BeginArea(section);
        {
            if (roomsAmount > maxRoomsAmountPropert.intValue)
            {
                EditorGUILayout.HelpBox("Number of rooms can't be greater then maximal rooms number", MessageType.Error);
                pos.y += step * 2;
            }
            EditorGUI.LabelField(pos, new GUIContent("Number of rooms: Min " + minRoomsAmountPropert.intValue + "; Max " + maxRoomsAmountPropert.intValue));
            pos.y += step;
            EditorGUI.LabelField(pos, new GUIContent("Number of rooms with 100% appearance chance: " + roomsAmount));
            pos.y += step;
            EditorGUILayout.Space(pos.y);
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
        }
        GUILayout.EndArea();
        GUI.EndScrollView();
    }

    private void DrawEmptyEditor() { }


    void InitTexture()
    {
        headerSectionTextrue = new Texture2D(1, 1);
        headerSectionTextrue.SetPixel(0, 0, headerSectionColor);
        headerSectionTextrue.Apply();

        tabsSectionTexture = new Texture2D(1, 1);
        tabsSectionTexture.SetPixel(0, 0, tabsSectionColor);
        tabsSectionTexture.Apply();

        editorSectionTextrue = new Texture2D(1, 1);
        editorSectionTextrue.SetPixel(0, 0, editorSectionColor);
        editorSectionTextrue.Apply();
    }

    private void OnEnable()
    {
        selectedTab = Tabs.General;
        InitTexture();
    }

    private void OnDisable()
    {
        lastOpenedSetting = window.serializedObject;
    }

    private void OnDestroy()
    {
        lastOpenedSetting = window.serializedObject;
    }

    private void OnGUI()
    {
        GetProperties();
        List<Action> tabsContent = new List<Action>();
        string[] tabs;
        switch (selectedType)
        {
            
            case GeneratorType.Default:
                tabs = new string[5] { "General", "Random Room Prefabs", "Corridor Prefabs", "Custom Rooms", "Empty" };
                tabsContent.Add(() => DrawGeneralEditorDefault());
                tabsContent.Add(() => DrawRandomRoomEditor());
                tabsContent.Add(() => DrawCorridorEditor());
                tabsContent.Add(() => DrawCustomRoomEditor());
                tabsContent.Add(() => DrawEmptyEditor());
                break;
            case GeneratorType.MainPath:
                tabs = new string[5] { "General", "Random Room Prefabs", "Corridor Prefabs", "Custom Main Rooms", "Custrom Side Rooms" };
                tabsContent.Add(() => DrawGeneralEditorMainPath());
                tabsContent.Add(() => DrawRandomRoomEditor());
                tabsContent.Add(() => DrawCorridorEditor());
                tabsContent.Add(() => DrawCustomRoomEditor());
                tabsContent.Add(() => DrawCustomSideRoomEditor());
                break;
            case GeneratorType.Hub:
                tabs = new string[5] { "General", "Random Room Prefabs", "Corridor Prefabs", "Custom Rooms", "Empty" };
                tabsContent.Add(() => DrawGeneralEditorHub());
                tabsContent.Add(() => DrawRandomRoomEditor());
                tabsContent.Add(() => DrawCorridorEditor());
                tabsContent.Add(() => DrawCustomRoomEditor());
                tabsContent.Add(() => DrawEmptyEditor());
                break;
            default:
                tabs = new string[5] { "?", "?", "?", "?", "?" };
                tabsContent.Add(() => DrawEmptyEditor());
                tabsContent.Add(() => DrawEmptyEditor());
                tabsContent.Add(() => DrawEmptyEditor());
                tabsContent.Add(() => DrawEmptyEditor());
                tabsContent.Add(() => DrawEmptyEditor());
                break;
        }
        DrawLayouts();
        DrawHeader();
        DrawTabs(tabs);
        DrawEditor(tabsContent);
    }

    
}
