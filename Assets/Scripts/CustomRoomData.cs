using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;


public class CustomRoomData
{
    static readonly string folderPath = Application.persistentDataPath + "/CustomRoomsData";
    public string name { get; private set; }
    public int size { get; private set; }
    public TileTag[,] tagsData { get; private set; }

    public CustomRoomData(string name, int size, TileTag[,] tagsData)
    {
        this.name = name;
        this.size = size;
        this.tagsData = tagsData;
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        Directory.CreateDirectory(folderPath);
        FileStream file = File.Create(folderPath + string.Format("/{0}.dat", name));
        CustomRoomSaveData data = new CustomRoomSaveData();
        data.savedName = name;
        data.savedSize = size;
        data.savedTagsData = tagsData;
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Room data saved!");
    }

    public static CustomRoomData Load(string name)
    {
        if (File.Exists(folderPath + string.Format("/{0}.dat", name)))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(folderPath + string.Format("/{0}.dat", name), FileMode.Open);
            CustomRoomSaveData data = (CustomRoomSaveData)bf.Deserialize(file);
            file.Close();
            string roomName = data.savedName;
            int roomSize = data.savedSize;
            TileTag[,] roomTagsData = data.savedTagsData;
            //Debug.Log("Room data loaded!");
            return new CustomRoomData(roomName, roomSize, roomTagsData);
        }
        else
        {
            Debug.LogError("There is no save data!");
            throw new FileNotFoundException();
        }
    }

    public static void Delete(string name)
    {
        if (File.Exists(folderPath + string.Format("/{0}.dat", name)))
        {
            System.IO.File.Delete(folderPath + string.Format("/{0}.dat", name));
            Debug.Log("Deleted");
        }
        else
        {
            throw new FileNotFoundException();
        }
    }


    public static string[] GetAllCustomRoomsNames()
    {
        DirectoryInfo info = new DirectoryInfo(folderPath);
        FileInfo[] filesInfo = info.GetFiles();
        string[] names = new string[filesInfo.Length];
        for (int i = 0; i < filesInfo.Length; i++)
        {
            names[i] = filesInfo[i].Name.Substring(0, filesInfo[i].Name.Length - 4);
        }

        return names;
    }

}

[Serializable]
class CustomRoomSaveData
{
    public string savedName;
    public int savedSize;
    public TileTag[,] savedTagsData;
}
