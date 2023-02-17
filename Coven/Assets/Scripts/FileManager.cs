using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[Serializable]
public class FileManager : MonoBehaviour
{
    //[Header("File Settings")]
    //public string savePath;

    //[Header("Character Stats")]
    //public int maxHP;
    //int currentHP;
    //public int hitBonus;
    //public int damageResistance;

    public static void ClearSaveData()
    {
        DirectoryInfo dataDir = new DirectoryInfo(Application.persistentDataPath);
        dataDir.Delete(true);
    }
    public static void SaveStats(string filePath, object data)
    {
        //Debug.Log(Application.persistentDataPath);
        filePath = Path.Combine(Application.persistentDataPath, filePath);
        if (!File.Exists(filePath)) { SetUpFilePath(filePath); }
        File.WriteAllText(filePath, JsonUtility.ToJson(data));
    }

    public static CharacterManager.CharacterStats GetStats(string filePath)
    {
        //Debug.Log(Application.persistentDataPath);
        filePath = Path.Combine(Application.persistentDataPath, filePath);
        if (File.Exists(filePath))
        {
            return JsonUtility.FromJson<CharacterManager.CharacterStats>(File.ReadAllText(filePath));
        }
        else
        {
            return new CharacterManager.CharacterStats(0,0,0,0, "", Vector2.zero, "");
        }
        
    }

    static void SetUpFilePath(string path)
    {
        // stip the filename from the path
        string[] folders = path.Split('/', '\\');
        path = "";
        for(int i = 0; i < folders.Length - 1; i++)
        {
            path = path + folders[i] + '/';
        }

        // create the folder structure
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        } 
    }
}
    
    
