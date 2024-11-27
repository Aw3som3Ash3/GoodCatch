using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class TransformSaver : SaveableObject,ISaveable
{
    public override object DataToSave => Matrix4x4.TRS(this.transform.position,this.transform.rotation,this.transform.localScale);




    public override void Load(string json)
    {
        var data =JsonUtility.FromJson<Matrix4x4>(json);
        this.transform.position=data.GetPosition();
        this.transform.rotation = data.rotation;
        this.transform.localScale = data.lossyScale;
    }

    

}
