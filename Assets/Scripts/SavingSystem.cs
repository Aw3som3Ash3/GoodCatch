using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SavingSystem
{
    const string SAVE_FILE = "QuickSave";
    const string FILE_EXTENSION = ".Data";
    public const string FOLDER_NAME = "Saves";
    const int SLOT_AMOUNT = 3;
    const string SLOT_FOLDER_NAME="Slot";
    static int currentSlot=1;
    static string SavePath { get { return Path.Combine(Application.persistentDataPath, FOLDER_NAME, SLOT_FOLDER_NAME+" "+ currentSlot); } }
    static GameData data;
    [Serializable]
    class GameData
    {
        [SerializeField]
        SerializableDictionary<string, string> SaveableObject;
        [SerializeField]
        int currentScene;

        public string GetSaveable(string id)
        {
            return SaveableObject[id];
        }

        public void AddSaveable(ISaveable saveable)
        {
            if (SaveableObject == null)
            {
                SaveableObject=new SerializableDictionary<string, string>();
            }
            SaveableObject[saveable.ID]= JsonUtility.ToJson(saveable.DataToSave);
        }
        public void SetScene()
        {
            currentScene=SceneManager.GetActiveScene().buildIndex;
        }
        public int GetScene()
        {
            return currentScene;
        }

    }
  
    public static void SaveSelf(ISaveable saveable,bool writeData = false)
    {
        data.AddSaveable(saveable);
    }
    public static void SaveGame(bool writeData = false,string SaveName= SAVE_FILE)
    {
        var saveables = GameObject.FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>();
        data=new();
        data.SetScene();
        foreach(var saveable in saveables)
        {
            data.AddSaveable(saveable);
            
        }
        if (true)
        {
            WriteSave(SaveName);
        }
       
    }

    static void WriteSave(string saveName)
    {
        Directory.CreateDirectory(SavePath);
        string save = JsonUtility.ToJson(data, true);
        File.WriteAllText( Path.Combine(SavePath, saveName + FILE_EXTENSION), save);
        Debug.Log(save);
    }

    public static void ReadData(string saveName)
    {
        string filePath= Path.Combine(SavePath, saveName+FILE_EXTENSION);
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<GameData>(json);
            //LoadGame();
        }
    }

    public static void LoadGame(string saveName=SAVE_FILE)
    {
        if (data == null)
        {
            ReadData(saveName);
        }
        //SceneManager.LoadScene("LoadingScreen");
       
        SceneManager.LoadSceneAsync(data.GetScene());
        SceneManager.sceneLoaded += OnSceneLoad;
  
       
    }

    static void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != data.GetScene())
        {
            return;
        }
        var saveables = GameObject.FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>();
        foreach (var saveable in saveables)
        {

            saveable.Load(data.GetSaveable(saveable.ID));
        }

        //SceneManager.UnloadSceneAsync("LoadingScreen");
        SceneManager.SetActiveScene(scene);
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    public static void LoadSelf<T>(T saveable,string ID) where T: ISaveable
    {
        if (data == null)
        {
            ReadData(SAVE_FILE);
        }
        saveable.Load(data.GetSaveable(ID));

        
    }
}




public interface ISaveable
{

    object DataToSave { get;}
    string ID { get; }
    public string Save() 
    {
        
        return JsonUtility.ToJson(DataToSave);
        
    }
    public void Load(string json);
    
    
}


[Serializable]
public class SerializableDictionary<Tkey, TValue> : Dictionary<Tkey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] List<Tkey> keys=new();
    [SerializeField] List<TValue> values=new();

    public void OnAfterDeserialize()
    {
        this.Clear();
        for(int i=0;i<keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear(); 
        foreach(var pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }
}