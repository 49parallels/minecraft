using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager : MonoBehaviour
{

    public static SaveManager Instance;

    public SaveTemplate saveSlot;


    private static String directory = "/saves";
    private static String savefile = directory+"/save1.sav";
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if(Instance != null) 
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
    }


    bool IsSaved()
    {
        return Directory.Exists(Application.persistentDataPath + directory);
    }

    public void SaveGame()
    {
        if (!IsSaved())
        {
            Directory.CreateDirectory(Application.persistentDataPath + directory);
        } 
        
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + savefile);
        saveSlot.SerializeData();
        var json = JsonUtility.ToJson(saveSlot);
        bf.Serialize(file, json);
        file.Close();
    }

    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + savefile))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + savefile, FileMode.Open);
            JsonUtility.FromJsonOverwrite((string) bf.Deserialize(file), saveSlot);
            saveSlot.DeserializeData();
        }
        else
        {
            saveSlot.Reset();
        }
    }
    
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.L))
        {
            SaveGame();
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SaveGame();
            Application.Quit();
        }
    }
}
