using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public abstract class Item : ScriptableObject,ISerializationCallbackReceiver
{
    public string Type { get { return this.GetType().ToString(); } }
    [SerializeField]
    Texture2D icon;
    public Texture2D Icon { get { return icon; } }

    [SerializeField]
    [TextArea]
    string description;
    public string Description { get { return description; } }
    
    [SerializeField]
    string itemId;
    public string ItemId { get { return itemId; } }

    public static Dictionary<string,Item> getItemById=new Dictionary<string,Item>();

    private void OnEnable()
    {
     
    }

    public void OnBeforeSerialize()
    {
       
    }
    
    public void OnAfterDeserialize()
    {
        if (itemId == null || (getItemById.ContainsKey(itemId) && getItemById[itemId] != this) || itemId == "")
        {
            itemId = Guid.NewGuid().ToString();
            
        }
        getItemById[itemId] = this;

    }

}
