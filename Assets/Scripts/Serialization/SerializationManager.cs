using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializationManager
{
    public static bool DoesFileExist(string saveName)
    {
        string path = Application.persistentDataPath + Path.DirectorySeparatorChar + "saves" + Path.DirectorySeparatorChar + saveName + ".save";
        return File.Exists(path);
    }

    public static bool Save(string saveName, object saveData)
    {
        PlayerPrefs.Save();
        
        BinaryFormatter formatter = GetBinaryFormatter();

        if (!Directory.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "saves"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + Path.DirectorySeparatorChar + "saves");
        }

        string path = Application.persistentDataPath + Path.DirectorySeparatorChar + "saves" + Path.DirectorySeparatorChar + saveName + ".save";

        FileStream file = File.Create(path);
        formatter.Serialize(file, saveData);

        file.Close();
        return true;
    }

    public static object Load(string saveName)
    {
        string path = Application.persistentDataPath + Path.DirectorySeparatorChar + "saves" + Path.DirectorySeparatorChar + saveName + ".save";
        if (!File.Exists(path))
        {
            return null;
        }

        BinaryFormatter formatter = GetBinaryFormatter();
        FileStream file = File.Open(path, FileMode.Open);

        try
        {
            object save = formatter.Deserialize(file);
            file.Close();
            return save;
        }
        catch
        {
            Debug.LogErrorFormat("Failed to load file at {0}", path);
            file.Close();
            return null;
        }
    }

    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        return formatter;
    }
}
