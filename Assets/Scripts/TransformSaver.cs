using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class TransformSaver : SaveableObject
{
    public override object DataToSave =>new Data( Matrix4x4.TRS(this.transform.position,this.transform.rotation,this.transform.localScale),this.gameObject.activeSelf);


    struct Data
    {
        public Matrix4x4 transforms;
        public bool enabled;
        public Data(Matrix4x4 transforms, bool enabled)
        {
            this.transforms = transforms;
            this.enabled = enabled;
        }
    }

    public override void Load(string json)
    {
        var data =JsonUtility.FromJson<Data>(json);
        this.transform.position=data.transforms.GetPosition();
        this.transform.rotation = data.transforms.rotation;
        this.transform.localScale = data.transforms.lossyScale;
        this.gameObject.SetActive(data.enabled);
    }

    

}
