using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class TransformSaver : MonoBehaviour,ISaveable
{
    public object DataToSave => Matrix4x4.TRS(this.transform.position,this.transform.rotation,this.transform.localScale);

    [SerializeField]
    [HideInInspector]
    string id;
    public string ID =>id;



    public void Load(string json)
    {
        var data =JsonUtility.FromJson<Matrix4x4>(json);
        this.transform.position=data.GetPosition();
        this.transform.rotation = data.rotation;
        this.transform.localScale = data.lossyScale;
    }

    private void Awake()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (id == null)
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
#endif
    }

    [ContextMenu("Generate GUID")]
    void GenerateNewId()
    {
        id= GUID.Generate().ToString();
        Debug.Log("Current Save ID:" + id);
    }

}
