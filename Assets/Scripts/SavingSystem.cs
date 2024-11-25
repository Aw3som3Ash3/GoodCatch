using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class SavingSystem
{
    const string SAVE_FILE = "Save.json";
    static string SavePath { get { return Path.Combine(Application.persistentDataPath + SAVE_FILE); } }
    static GameData data = new GameData();
    [Serializable]
    struct GameData
    {
        [SerializeField]
        SerializableDictionary<string, string> SaveableObject;

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
            SaveableObject.Add(saveable.ID, JsonUtility.ToJson(saveable.DataToSave));
        }


    }
  
    public static void SaveGame(bool writeData=false)
    {
        var saveables = GameObject.FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>();
        //GameData data=new();
        foreach(var saveable in saveables)
        {
            data.AddSaveable(saveable);
            
        }
        if (true)
        {
            WriteSave();
        }
       
    }

    static void WriteSave()
    {
        string save = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, save);
        Debug.Log(save);
    }

    public static void ReadAndLoadData()
    {
        if(File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            LoadGame();
        }
    }

    public static void LoadGame()
    {
       
        //GameData data = JsonUtility.FromJson<GameData>(json);
        var saveables = GameObject.FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>();
        foreach (var saveable in saveables)
        {

            saveable.Load(data.GetSaveable(saveable.ID));
        }
    }

}




interface ISaveable
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