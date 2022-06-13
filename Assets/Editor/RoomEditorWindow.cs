using UnityEngine;
using UnityEditor;
using System;

public class RoomEditorWindow : EditorWindow
{
    Color headerSectionColor = new Color(13f / 255f, 32f / 255f, 44f / 255f);
    Color actionSectionColor = new Color(115f / 255f, 115f / 255f, 115f / 255f);
    Color editorGridSectionColor = new Color(105f / 255f, 105f / 255f, 105f / 255f);

    Rect headerSection;
    Rect actionSection;
    Rect editorGridSection;

    Texture2D headerSectionTextrue;
    Texture2D actionSectionTexture;
    Texture2D editorGridSectionTextrue;

    Texture2D rotationDirectionImg;

    //Icons

    Texture2D rotationIdentityIcon;
    Texture2D rotationUpIcon;
    Texture2D rotationRightIcon;
    Texture2D rotationDownIcon;
    Texture2D rotationLeftIcon;

    Texture2D alignmentCenterIcon;
    Texture2D alignmentUpIcon;
    Texture2D alignmentRightIcon;
    Texture2D alignmentDownIcon;
    Texture2D alignmentLeftIcon;

    Texture2D floorIcon;

    Texture2D wallHorizontalIcon;
    Texture2D wallVerticalIcon;

    Texture2D cornerWallUpIcon;
    Texture2D cornerWallRightIcon;
    Texture2D cornerWallDownIcon;
    Texture2D cornerWallLeftIcon;

    Texture2D tripleWallUpIcon;
    Texture2D tripleWallRightIcon;
    Texture2D tripleWallDownIcon;
    Texture2D tripleWallLeftIcon;

    Texture2D enterIcon;

    GUIStyle defaultButtonStyle;

    //Window setting
    static RoomEditorWindow mainWindow;
    static Vector2 windowSize = new Vector2(600, 665);
    int cellSize = 70;
    int minimumCellSize = 20;
    int maximumCellSize = 120;
    const int scaleButtonStep = 10;
    int gridSize = 0;
    string gridSizeStr = "-";
    string roomName = "CustomRoomName";
    bool editMode = false;
    public GUISkin customSkin;
    int selectedRoomIdx = 0;
    string[] roomsList;

    //test
    Vector2 scrollPos = new Vector2(0, 0);
    public bool selectionGrid = false;
    bool[,] buttons;
    TileTag[,] tags;

    int selectedTool = 0;

    [MenuItem("Window/Room Editor")]
    static void OpenEditor()
    {
        mainWindow = (RoomEditorWindow)GetWindow(typeof(RoomEditorWindow));
        mainWindow.minSize = windowSize;
        mainWindow.maxSize = windowSize;
        mainWindow.Show();
    }

    void OpenConfirmWindow()
    {

    }


    private void OnEnable()
    {
        //InitDefaultStyle();
        InitTexture();
        PrepareGrid(0);
        roomsList = CustomRoomData.GetAllCustomRoomsNames();
    }

    private void PrepareGrid(int gridSize)
    {
        editMode = false;
        if (gridSize < 3)
            return;

        this.gridSize = gridSize;
        buttons = new bool[gridSize, gridSize];
        tags = new TileTag[gridSize, gridSize];
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                tags[i, j] = new TileTag(TileMap.room, TileType.empty, TileAlignment.center);
            }
        }
        editMode = true;
    }

    private void PrepareGrid(int gridSize, TileTag[,] tileTags)
    {
        editMode = false;
        if (gridSize < 3)
        {
            gridSizeStr = "-";
            return;
        }


        this.gridSize = gridSize;
        gridSizeStr = gridSize.ToString();
        buttons = new bool[gridSize, gridSize];
        tags = new TileTag[gridSize, gridSize];
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                tags[i, j] = tileTags[i, j];
            }
        }
        editMode = true;
    }

    private void OnGUI()
    {
        DrawLayouts();
        DrawHeader();
        DrawActionSection();
        DrawEditorGrid();


    }

    void InitDefaultStyle()
    {
        defaultButtonStyle = new GUIStyle(GUI.skin.button);
        defaultButtonStyle.fixedHeight = 20;
        defaultButtonStyle.fixedWidth = 20;
    }
    private void DrawTypeIcon(Vector2 pos, Vector2 size, TileTag tag)
    {
        if (tag.type == TileType.floor)
            GUI.DrawTexture(new Rect(pos, size), floorIcon);
        else if (tag.type == TileType.wall)
            GUI.DrawTexture(new Rect(pos, size), wallHorizontalIcon);
        else if (tag.type == TileType.cornerWall)
            GUI.DrawTexture(new Rect(pos, size), cornerWallDownIcon);
        else if (tag.type == TileType.tripleWall)
            GUI.DrawTexture(new Rect(pos, size), tripleWallDownIcon);
        else if (tag.type == TileType.enter)
            GUI.DrawTexture(new Rect(pos, size), enterIcon);
    }

    private void DrawRotationIcon(Vector2 pos, Vector2 size, TileTag tag)
    {
        if (tag.type == TileType.empty)
            return;

        switch (tag.rotation)
        {
            case TileRotation.identity:
                GUI.DrawTexture(new Rect(pos, size), rotationIdentityIcon);
                break;
            case TileRotation.up:
                GUI.DrawTexture(new Rect(pos, size), rotationUpIcon);
                break;
            case TileRotation.right:
                GUI.DrawTexture(new Rect(pos, size), rotationRightIcon);
                break;
            case TileRotation.down:
                GUI.DrawTexture(new Rect(pos, size), rotationDownIcon);
                break;
            case TileRotation.left:
                GUI.DrawTexture(new Rect(pos, size), rotationLeftIcon);
                break;
        }
    }
    void DrawAlignmentIcon(Vector2 pos, Vector2 size, TileTag tag)
    {
        if (tag.type == TileType.empty)
            return;

        switch (tag.alignment)
        {
            case TileAlignment.center:
                GUI.DrawTexture(new Rect(pos, size), alignmentCenterIcon);
                break;
            case TileAlignment.up:
                GUI.DrawTexture(new Rect(pos, size), alignmentUpIcon);
                break;
            case TileAlignment.right:
                GUI.DrawTexture(new Rect(pos, size), alignmentRightIcon);
                break;
            case TileAlignment.down:
                GUI.DrawTexture(new Rect(pos, size), alignmentDownIcon);
                break;
            case TileAlignment.left:
                GUI.DrawTexture(new Rect(pos, size), alignmentLeftIcon);
                break;
        }
    }

    void DrawIconFromTag(Vector2 pos, Vector2 size, TileTag tag)
    {
        DrawTypeIcon(pos, size, tag);
        DrawRotationIcon(pos, size, tag);
        DrawAlignmentIcon(pos, size, tag);
    }

    void ChangeTagType(TileTag tag)
    {
        if (tag.type == TileType.empty)
            tag.type = TileType.floor;
        else if (tag.type == TileType.floor)
            tag.type = TileType.wall;
        else if (tag.type == TileType.wall)
            tag.type = TileType.cornerWall;
        else if (tag.type == TileType.cornerWall)
            tag.type = TileType.tripleWall;
        else if (tag.type == TileType.tripleWall)
            tag.type = TileType.enter;
        else if (tag.type == TileType.enter)
            tag.type = TileType.empty;
    }

    void ChangeTagAlignment(TileTag tag)
    {
        tag.alignment = (TileAlignment)(((int)tag.alignment + 1) % Enum.GetValues(typeof(TileAlignment)).Length); 
    }

    void ChangeTagRotation(TileTag tag)
    {
        tag.rotation = (TileRotation)(((int)tag.rotation + 1) % Enum.GetValues(typeof(TileRotation)).Length);
    }

    GUIStyle GetStyle()
    {
        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.normal.textColor = Color.red;
        return style;
    }

    GUIStyle GetCustomStyle()
    {
        GUIStyle style = new GUIStyle(customSkin.button);
        //style.normal.textColor = Color.red;
        return style;
    }
    void InitTexture()
    {
        headerSectionTextrue = new Texture2D(1, 1);
        headerSectionTextrue.SetPixel(0, 0, headerSectionColor);
        headerSectionTextrue.Apply();

        actionSectionTexture = new Texture2D(1, 1);
        actionSectionTexture.SetPixel(0, 0, actionSectionColor);
        actionSectionTexture.Apply();

        editorGridSectionTextrue = new Texture2D(1, 1);
        editorGridSectionTextrue.SetPixel(0, 0, editorGridSectionColor);
        editorGridSectionTextrue.Apply();



        rotationDirectionImg = Resources.Load<Texture2D>("arrow-head");

        rotationIdentityIcon = Resources.Load<Texture2D>("RoomEditorIcons/rotation-identity");
        rotationUpIcon = Resources.Load<Texture2D>("RoomEditorIcons/rotation-up");
        rotationRightIcon = Resources.Load<Texture2D>("RoomEditorIcons/rotation-right");
        rotationDownIcon = Resources.Load<Texture2D>("RoomEditorIcons/rotation-down");
        rotationLeftIcon = Resources.Load<Texture2D>("RoomEditorIcons/rotation-left");


        alignmentCenterIcon = Resources.Load<Texture2D>("RoomEditorIcons/alignment-center");
        alignmentUpIcon = Resources.Load<Texture2D>("RoomEditorIcons/alignment-up");
        alignmentRightIcon = Resources.Load<Texture2D>("RoomEditorIcons/alignment-right");
        alignmentDownIcon = Resources.Load<Texture2D>("RoomEditorIcons/alignment-down");
        alignmentLeftIcon = Resources.Load<Texture2D>("RoomEditorIcons/alignment-left");

        floorIcon = Resources.Load<Texture2D>("RoomEditorIcons/floor");

        wallHorizontalIcon = Resources.Load<Texture2D>("RoomEditorIcons/wall-horizontal");
        wallVerticalIcon = Resources.Load<Texture2D>("RoomEditorIcons/wall-vertical");

        cornerWallUpIcon = Resources.Load<Texture2D>("RoomEditorIcons/cornerWall-up");
        cornerWallRightIcon = Resources.Load<Texture2D>("RoomEditorIcons/cornerWall-right");
        cornerWallDownIcon = Resources.Load<Texture2D>("RoomEditorIcons/cornerWall-down");
        cornerWallLeftIcon = Resources.Load<Texture2D>("RoomEditorIcons/cornerWall-left");

        tripleWallUpIcon = Resources.Load<Texture2D>("RoomEditorIcons/tripleWall-up");
        tripleWallRightIcon = Resources.Load<Texture2D>("RoomEditorIcons/tripleWall-right");
        tripleWallDownIcon = Resources.Load<Texture2D>("RoomEditorIcons/tripleWall-down");
        tripleWallLeftIcon = Resources.Load<Texture2D>("RoomEditorIcons/tripleWall-left");

        enterIcon = Resources.Load<Texture2D>("RoomEditorIcons/enter");
    }

    void DrawLayouts()
    {
        headerSection.x = 0;
        headerSection.y = 0;
        headerSection.width = windowSize.x;
        headerSection.height = 50;

        actionSection.x = 0;
        actionSection.y = 50;
        actionSection.width = windowSize.x;
        actionSection.height = 40;

        editorGridSection.x = 0;
        editorGridSection.y = 90;
        editorGridSection.width = windowSize.x;
        editorGridSection.height = windowSize.y - 50;

        GUI.DrawTexture(headerSection, headerSectionTextrue);
        GUI.DrawTexture(actionSection, actionSectionTexture);
        GUI.DrawTexture(editorGridSection, editorGridSectionTextrue);
    }

    void DrawHeader()
    {
        selectedTool = GUI.SelectionGrid(
            new Rect(200, 5, 140, 30),
            selectedTool, 
            new GUIContent[] {
                new GUIContent(EditorGUIUtility.IconContent("CrossIcon").image),
                new GUIContent(EditorGUIUtility.IconContent("d_Grid.PaintTool").image),
                new GUIContent(EditorGUIUtility.IconContent("d_RotateTool On").image),
                new GUIContent(EditorGUIUtility.IconContent("d_MoveTool On").image) 
            },
            4
        );
      
    }

    void DrawActionSection()
    {
        GUILayout.BeginArea(actionSection);
        float heightHalf = actionSection.height / 2;
        float widthHalf = actionSection.width / 2;
        //firstRow
        GUIContent createButtonContent = new GUIContent("Create", EditorGUIUtility.IconContent("CreateAddNew").image, "Create empty NxN room\nIt's replace opended room so make sure to save opended room");
        bool createRoomButton = GUI.Button(new Rect(0, 0, widthHalf / 2, heightHalf), createButtonContent);
        //bool createRoomButton = GUI.Button(new Rect(0, 0, widthHalf / 2, heightHalf), "Create Room");
        gridSizeStr = GUI.TextField(new Rect(widthHalf / 2, 0, widthHalf / 2, heightHalf), gridSizeStr);
        GUIContent saveButtonContent = new GUIContent("Save", EditorGUIUtility.IconContent("d_SaveAs").image, "Save room with given name\nWill override existing room with same name");
        bool saveRoomButton = GUI.Button(new Rect(widthHalf, 0, widthHalf / 2, heightHalf), saveButtonContent);
        roomName = GUI.TextField(new Rect(widthHalf + widthHalf / 2, 0, widthHalf / 2, heightHalf), roomName);
        //secondRow
        bool loadRoomButton = GUI.Button(new Rect(0, heightHalf, widthHalf / 2, heightHalf), "Load/Edit");
        GUILayout.BeginArea(new Rect(widthHalf / 2, heightHalf, widthHalf / 2 - 20, heightHalf));
        selectedRoomIdx = EditorGUILayout.Popup(selectedRoomIdx, roomsList);
        GUILayout.EndArea();
        bool deleteRoomButton = GUI.Button(new Rect(widthHalf - 20, heightHalf, 20, heightHalf), "X");
        bool scaleMinus = GUI.Button(new Rect(widthHalf, heightHalf, widthHalf / 2, heightHalf), "Scale-");
        bool scalePlus = GUI.Button(new Rect(widthHalf + widthHalf / 2, heightHalf, widthHalf / 2, heightHalf), "Scale+");
        GUILayout.EndArea();


        if (createRoomButton)
        {
            bool parsed = int.TryParse(gridSizeStr, out int newGridSize);
            if (!parsed)
                Debug.LogError("Grid size must be integer");
            else
                PrepareGrid(newGridSize);
        }
        if (saveRoomButton)
        {
            SaveRoom();
            selectedRoomIdx = roomsList.Length-1;
        }
        if (loadRoomButton)
        {
            LoadRoom(roomsList[selectedRoomIdx]);
        }
        if (deleteRoomButton)
        {
            bool confirm = EditorUtility.DisplayDialog("Delete?", "Ara you sure you want to delete room named: " + roomsList[selectedRoomIdx], "Yes", "No");
            if (confirm)
                DeleteRoom(roomsList[selectedRoomIdx]);
            selectedRoomIdx = 0;
        }
        if (scaleMinus)
        {
            cellSize = Mathf.Clamp(cellSize - scaleButtonStep, minimumCellSize, maximumCellSize);
        }
        if (scalePlus)
        {
            cellSize = Mathf.Clamp(cellSize + scaleButtonStep, minimumCellSize, maximumCellSize);
        }
    }

    private bool IsValidRoom()
    {
        foreach (TileTag tag in tags)
        {
            if (tag.type == TileType.enter)
                return true;
        }
        return false;
    }

    private void SaveRoom()
    {
        if (!editMode)
        {
            Debug.LogError("Nothing to save");
            return;
        }
        else if (string.IsNullOrWhiteSpace(roomName))
        {
            Debug.LogWarning("Empty Room Name");
            return;
        }
        else if (!IsValidRoom())
        {
            Debug.LogError("Room should have at least one entry point");
            return;
        }

        CustomRoomData roomData = new CustomRoomData(roomName, gridSize, tags);
        roomData.Save();
        roomsList = CustomRoomData.GetAllCustomRoomsNames();
    }
    private void LoadRoom(string roomName)
    {
        CustomRoomData roomData = CustomRoomData.Load(roomName);
        this.roomName = roomName;
        PrepareGrid(roomData.size, roomData.tagsData);
    }
    private void DeleteRoom(string roomName)
    {
        CustomRoomData.Delete(roomName);
        roomsList = CustomRoomData.GetAllCustomRoomsNames();

    }



    void DrawEditorGrid()
    {
        /*if (!editMode)
        {
            Debug.Log("EXIT");
            return;
        }*/
        if (!editMode)
            return;

        GUILayout.BeginArea(editorGridSection);
        Vector2Int buttonSize = new Vector2Int(cellSize, cellSize);
        Vector2Int buttonIconOffset = new Vector2Int(5, 5);
        Vector2Int buttonIconSize = new Vector2Int(cellSize - 10, cellSize - 10);
        Vector2Int gapSize = new Vector2Int(5, 5);
        Vector2Int cursor = new Vector2Int(0, 0);

        scrollPos = GUI.BeginScrollView(new Rect(0, 0, windowSize.x - 5, windowSize.y - 85), scrollPos,
            new Rect(0, 0, gridSize * (cellSize + 1 + gapSize.x), gridSize * (cellSize + 1 + gapSize.y)), true, true);

        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                //buttons[x, y] = GUI.Button(new Rect(cursor, buttonSize), debugText[x, y], GetCustomStyle());
                //buttons[x, y] = GUI.Button(new Rect(cursor, buttonSize), TagToIcon(tileTag[x,y]), GetCustomStyle());
                buttons[x, y] = GUI.Button(new Rect(cursor, buttonSize), "", GetCustomStyle());
                DrawIconFromTag(cursor + buttonIconOffset, buttonIconSize, tags[x, y]);
                //GUI.DrawTexture(new Rect(cursor+ buttonIconOffset, buttonIconSize), cornerWallRightIcon);

                if (buttons[x, y])
                {
                    if (selectedTool == 0)
                        tags[x, y] = TileTag.Empty();
                    else if (selectedTool == 1)
                        ChangeTagType(tags[x, y]);
                    else if (selectedTool == 2)
                        ChangeTagRotation(tags[x, y]);
                    else if (selectedTool == 3)
                        ChangeTagAlignment(tags[x, y]);

                    Debug.Log(tags[x, y]);
                }


                cursor.x += buttonSize.x + gapSize.x;
            }
            cursor.x = 0;
            cursor.y += buttonSize.y + gapSize.y;
        }
        GUI.EndScrollView();


        GUILayout.EndArea();


    }

}
