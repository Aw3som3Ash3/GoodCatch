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
    const string FOLDER_NAME = "Saves";
    const int SLOT_AMOUNT = 3;
    const int MAX_SAVES_PER_CATEGORY = 3;
    //static int currentSaveNum = 1;
    const string SLOT_FOLDER_NAME="Slot";
    static int currentSlot=1;
    static public string SavePath { get { return Path.Combine(Application.persistentDataPath, FOLDER_NAME, SLOT_FOLDER_NAME+" "+ currentSlot); } }
    static GameData data;
   
    public enum SaveMode
    {
        QuickSave,
        AutoSave,
        ManualSave
    }

    [Serializable]
    class GameData
    {
        [SerializeField]
        SerializableDictionary<int, SerializableDictionary<string, string>> SceneData=new();

        //[SerializeField]
        //SerializableDictionary<string, string> SaveableObject;
        
        [SerializeField]
        int currentScene;

        public string GetSaveable(string objectId,int sceneId)
        {
            if (SceneData.ContainsKey(sceneId))
            {
                if (!SceneData[sceneId].ContainsKey(objectId))
                {
                    return null;
                }

                return SceneData[sceneId][objectId];
            }
            else
            {
                return null;
            }

           
        }

        public void AddSaveable(ISaveable saveable,int sceneId)
        {
            if (!SceneData.ContainsKey(sceneId) || SceneData[sceneId] == null)
            {
                SceneData[sceneId] = new SerializableDictionary<string, string>();
            }
            Debug.Log(sceneId);
            Debug.Log(saveable.ID);
            SceneData[sceneId][saveable.ID]= JsonUtility.ToJson(saveable.DataToSave);
        }
        public void SetScene()
        {
            currentScene=SceneManager.GetActiveScene().buildIndex;
            Debug.Log("current scene index: " + SceneManager.GetActiveScene().buildIndex);
        }
        public int GetScene()
        {
            return currentScene;
        }

    }
  
    public static void SaveSelf(ISaveable saveable,int sceneId,bool writeData = false)
    {
        data.AddSaveable(saveable, sceneId);
    }
    public static void SaveGame(SaveMode saveMode)
    {

        var saveables = GameObject.FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>();
        if (data== null)
        {
            data = new();
        }
        data.SetScene();
        foreach(var saveable in saveables)
        {
            int sceneId = (saveable as MonoBehaviour).gameObject.scene.buildIndex;
            data.AddSaveable(saveable, sceneId);
            
        }
        if (saveMode == SaveMode.ManualSave)
        {
            WriteSave(DateTime.Now.ToLocalTime().ToString("yyyyMMdd_hhmmss"));
        }
        else
        {
            int saveNumber = (GetLatestFileNumber(saveMode)% MAX_SAVES_PER_CATEGORY) + 1;
            WriteSave(saveMode.ToString()+ saveNumber);
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
    public static void ReadData()
    {
        var info = new DirectoryInfo(SavePath);
        var file = info.GetFiles().OrderByDescending((x) => x.LastWriteTime).First();
        string json = File.ReadAllText(file.FullName);
        data = JsonUtility.FromJson<GameData>(json);
    }
    public static void LoadGame(int slotNum)
    {
        currentSlot= slotNum;
        LoadGame();
    }
    public static void LoadGame()
    {

        if (data == null)
        {
            ReadData();
        }
        SceneManager.LoadSceneAsync(data.GetScene());
        
        SceneManager.sceneLoaded += OnSceneLoad;


    }
    public static void LoadGame(string saveName=SAVE_FILE)
    {

        ReadData(saveName);
        //SceneManager.LoadScene("LoadingScreen");
       
        

        SceneManager.LoadSceneAsync(data.GetScene());
        SceneManager.sceneLoaded += OnSceneLoad;
  
       
    }
    static int GetLatestFileNumber(SaveMode saveMode)
    {
       
        var info = new DirectoryInfo(SavePath);
        if (!info.Exists)
        {
            return 0;
        }
        if (info.GetFiles().Length <= 0)
        {
            return 0;
        }
        var files = info.GetFiles().Where((x) =>
        {
            string fileName = Path.GetFileNameWithoutExtension(x.Name);
            return fileName.Remove(fileName.Length - 1) == saveMode.ToString();
        });
        if (files.Count() <= 0)
        {
            return 0;
        }
        var file= files.OrderByDescending((x) => x.LastWriteTime).First();
        string fileName = Path.GetFileNameWithoutExtension(file.Name);
        string fileNum =fileName.Substring(fileName.Length-1);
        int intValue = int.Parse(fileNum);
        return intValue;
    }
    static void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
        if (scene.buildIndex != data.GetScene()||scene.buildIndex==0)
        {
            return;
        }
        //Debug.LogError("scene loaded");
        var sceneLoader = GameObject.FindObjectOfType<SceneLoader>(true);
        Debug.Log("current slot: " +currentSlot);
        if (sceneLoader == null)
        {
            var saveables = GameObject.FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>();
            foreach (var saveable in saveables)
            {
                int sceneId = (saveable as MonoBehaviour).gameObject.scene.buildIndex;
                Debug.Log(saveable.ID + ":" + saveable +" scene id"+sceneId);
                saveable.Load(data.GetSaveable(saveable.ID, sceneId));
                Time.timeScale = 1;
            }
            return;
        }
        sceneLoader.AllScenesLoaded += () =>
        {
           
            var saveables = GameObject.FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>();
            foreach (var saveable in saveables)
            {
                int sceneId = (saveable as MonoBehaviour).gameObject.scene.buildIndex;
                Debug.Log(saveable.ID + ":" + saveable);
                saveable.Load(data.GetSaveable(saveable.ID, sceneId));
                Time.timeScale= 1;
            }
            sceneLoader.AllScenesLoaded = null;
        };
       


    }

    //static void LoadObjectData()
    //{

    //}
    
    public static void LoadSelf<T>(T saveable,string ID) where T: ISaveable
    {
        if (data == null)
        {
            ReadData(SAVE_FILE);
        }
        int sceneId = (saveable as MonoBehaviour).gameObject.scene.buildIndex;
        saveable.Load(data.GetSaveable(ID, sceneId));
    
        
    }

    public static bool HasSlot(int slot)
    {
        DirectoryInfo directoryInfo=new DirectoryInfo(Path.Combine(Application.persistentDataPath, FOLDER_NAME, SLOT_FOLDER_NAME + " " + slot));
        return directoryInfo.Exists && directoryInfo.GetFiles().Length > 0;
    }
    public static void SetSlot(int slot)
    {
        currentSlot = slot;
    }

    public static void ClearSlot(int slot)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(Application.persistentDataPath, FOLDER_NAME, SLOT_FOLDER_NAME + " " + slot));
        if (directoryInfo.Exists)
        {
            directoryInfo.Delete(true);
        }
        directoryInfo.Create();
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