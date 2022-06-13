using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(WrappedObject))]
public class WrappedPropertyDrawer : PropertyDrawer
{
    private void DrawPropertyField(Rect position, SerializedProperty property, GUIContent label)
    {
        position = EditorGUI.PrefixLabel(position, label);
        EditorGUI.PropertyField(position, property, new GUIContent(""));

    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        int oldIndentLevel = EditorGUI.indentLevel;
        Rect gameObjecrFieldRect = new Rect(position.x, position.y+5, position.width, 18);
        Rect floatFieldRect = new Rect(position.x, position.y+25, position.width, 18);
        Rect vectorFieldRect = new Rect(position.x, position.y+45, position.width, 18);
        
        EditorGUI.indentLevel = 1;
        DrawPropertyField(gameObjecrFieldRect, property.FindPropertyRelative("gameObject"), new GUIContent("Prefab"));
        DrawPropertyField(floatFieldRect, property.FindPropertyRelative("tileAlignment"), new GUIContent("Alignment"));
        DrawPropertyField(vectorFieldRect, property.FindPropertyRelative("defaultEulerAngles"), new GUIContent("Rotation"));
        EditorGUI.indentLevel = oldIndentLevel;
        EditorGUI.EndProperty();
    }

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement container = new VisualElement();

        PropertyField gameObjectField = new PropertyField(property.FindPropertyRelative("gameObject"));
        PropertyField floatField = new PropertyField(property.FindPropertyRelative("tileAlignment"));
        PropertyField vectorField = new PropertyField(property.FindPropertyRelative("defaultEulerAngles"), "Fancy Name");

        // Add fields to the container.
        container.Add(gameObjectField);
        container.Add(floatField);
        container.Add(vectorField);

        return container;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 65;
    }

}

[CustomPropertyDrawer(typeof(CustomRoomPrefabsSet))]
public class CustomRoomPrefabsSetDrawer : PropertyDrawer
{
    const float additionalHight = 30;
    const float step = 22;
    const float wrappedPropertyHeight = 70;
    const float expandedOffset = 50;
    const float defaultFieldSize = 18;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        
        EditorGUI.BeginProperty(position, label, property);
        int oldIndentLevel = EditorGUI.indentLevel;
        SerializedProperty isStartRoomProperty = property.FindPropertyRelative("isStartRoom");
        SerializedProperty isEndRoomProperty = property.FindPropertyRelative("isEndRoom");
        SerializedProperty generationChanceProperty = property.FindPropertyRelative("generationChance");
        SerializedProperty maxAmountProperty = property.FindPropertyRelative("maxAmount");
        SerializedProperty floorTilesProperty = property.FindPropertyRelative("floorTiles");
        SerializedProperty wallTilesProperty = property.FindPropertyRelative("wallTiles");
        SerializedProperty cornerWallTilesProperty = property.FindPropertyRelative("cornerWallTiles");
        SerializedProperty tripleWallTilesProperty = property.FindPropertyRelative("parallelWallTiles");
        SerializedProperty parallelWallTilesProperty = property.FindPropertyRelative("tripleWallTiles");
        SerializedProperty roomNameProperty = property.FindPropertyRelative("roomName");



        Rect pos = position;
        pos.y += 5;
        
        Rect isStartRoomRect = new Rect(pos.x, pos.y, pos.width, defaultFieldSize);
        pos.y += defaultFieldSize + step / 2;
        Rect isEndRoomRect = new Rect(pos.x, pos.y, pos.width, defaultFieldSize);
        pos.y += defaultFieldSize + step / 2;
        Rect generationChanceRect = new Rect(pos.x, pos.y, pos.width, defaultFieldSize);
        pos.y += defaultFieldSize + step/2;
        Rect maxAmountRect = new Rect(pos.x, pos.y, pos.width, defaultFieldSize);
        pos.y += defaultFieldSize + step/2;
        Rect floorTilesRect = new Rect(pos.x, pos.y, pos.width, defaultFieldSize);
        pos.y += floorTilesProperty.isExpanded ? step + expandedOffset + floorTilesProperty.arraySize * wrappedPropertyHeight : step;
        Rect wallTilesdRect = new Rect(pos.x, pos.y, pos.width, defaultFieldSize);
        pos.y += wallTilesProperty.isExpanded ? step + expandedOffset + wallTilesProperty.arraySize * wrappedPropertyHeight : step;
        Rect cornerWallTilesRect = new Rect(pos.x, pos.y, pos.width, defaultFieldSize);
        pos.y += cornerWallTilesProperty.isExpanded ? step + expandedOffset + cornerWallTilesProperty.arraySize * wrappedPropertyHeight : step;
        Rect parallelWallTilesRect = new Rect(pos.x, pos.y, pos.width, defaultFieldSize);
        pos.y += parallelWallTilesProperty.isExpanded ? step + expandedOffset + parallelWallTilesProperty.arraySize * wrappedPropertyHeight : step;
        Rect tripleWallTilesRect = new Rect(pos.x, pos.y, pos.width, defaultFieldSize);
        pos.y += tripleWallTilesProperty.isExpanded ? step + expandedOffset + tripleWallTilesProperty.arraySize * wrappedPropertyHeight : step;
        Rect roomNameRect = new Rect(pos.x, pos.y, position.width, defaultFieldSize);
        pos.y += defaultFieldSize;
        //totalHight += additionalHight;



        EditorGUI.indentLevel = 1;
        EditorGUI.Slider(generationChanceRect, generationChanceProperty, 0f, 1f);
        EditorGUI.PropertyField(isStartRoomRect, isStartRoomProperty);
        EditorGUI.PropertyField(isEndRoomRect, isEndRoomProperty);
        EditorGUI.PropertyField(maxAmountRect, maxAmountProperty);
        if (maxAmountProperty.intValue < 0)
            maxAmountProperty.intValue = 0;
        EditorGUI.PropertyField(floorTilesRect, floorTilesProperty);
        EditorGUI.PropertyField(wallTilesdRect, wallTilesProperty);
        EditorGUI.PropertyField(cornerWallTilesRect, cornerWallTilesProperty);
        EditorGUI.PropertyField(parallelWallTilesRect, parallelWallTilesProperty);
        EditorGUI.PropertyField(tripleWallTilesRect, tripleWallTilesProperty);
        EditorGUI.indentLevel = oldIndentLevel;
        
        GUIStyle yellow = new GUIStyle(EditorStyles.label);
        yellow.normal.textColor = Color.yellow;
        Rect labelPos = EditorGUI.PrefixLabel(roomNameRect, new GUIContent("Selected Room : "));
        EditorGUI.LabelField(labelPos, roomNameProperty.stringValue, yellow);
        GuiLine(new Rect(pos.x, pos.y+5, position.width, defaultFieldSize), 1);
        if (isStartRoomProperty.boolValue == true)
            isEndRoomProperty.boolValue = false;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty floorTilesProperty = property.FindPropertyRelative("floorTiles");
        SerializedProperty wallTilesProperty = property.FindPropertyRelative("wallTiles");
        SerializedProperty cornerWallTilesProperty = property.FindPropertyRelative("cornerWallTiles");
        SerializedProperty tripleWallTilesProperty = property.FindPropertyRelative("parallelWallTiles");
        SerializedProperty parallelWallTilesProperty = property.FindPropertyRelative("tripleWallTiles");

        float totalHight = 5;
        totalHight += defaultFieldSize + step / 2;
        totalHight += defaultFieldSize + step / 2;
        totalHight += defaultFieldSize + step / 2;
        totalHight += defaultFieldSize + step / 2;
        totalHight += floorTilesProperty.isExpanded ? step + expandedOffset + floorTilesProperty.arraySize * wrappedPropertyHeight : step;
        totalHight += wallTilesProperty.isExpanded ? step + expandedOffset + wallTilesProperty.arraySize * wrappedPropertyHeight : step;
        totalHight += cornerWallTilesProperty.isExpanded ? step + expandedOffset + cornerWallTilesProperty.arraySize * wrappedPropertyHeight : step;
        totalHight += parallelWallTilesProperty.isExpanded ? step + expandedOffset + parallelWallTilesProperty.arraySize * wrappedPropertyHeight : step;
        totalHight += tripleWallTilesProperty.isExpanded ? step + expandedOffset + tripleWallTilesProperty.arraySize * wrappedPropertyHeight : step;
        totalHight += defaultFieldSize;
        totalHight += additionalHight;

        return totalHight;
    }

    void GuiLine(Rect rect, int i_height = 1)
    {
        //Rect rect = EditorGUILayout.GetControlRect(false, i_height);

        rect.height = i_height;

        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }
}
