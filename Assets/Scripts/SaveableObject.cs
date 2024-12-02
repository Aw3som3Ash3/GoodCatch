using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public abstract class SaveableObject : MonoBehaviour,ISaveable
{
    public abstract object DataToSave { get; }

    [SerializeField]
    [HideInInspector]
    string id;
    public string ID => id;

    public abstract void Load(string json);
#if UNITY_EDITOR
    private void Awake()
    {

        if (!Application.isPlaying)
        {
            if (id == null||id=="")
            {

                GenerateNewId();
                Debug.Log(this);

            }
            var objects = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();
            foreach (var obj in objects)
            {

                if (obj != (ISaveable)this && obj.ID == ID)
                {
                    GenerateNewId();
                    break;
                }
            }
        }

    }

    [ContextMenu("Generate GUID")]
    void GenerateNewId()
    {
        id = GUID.Generate().ToString();
        Debug.Log("Current Save ID:" + id);
    }
#endif
}
