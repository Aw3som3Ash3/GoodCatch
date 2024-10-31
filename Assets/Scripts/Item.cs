using UnityEngine;


public abstract class Item : ScriptableObject
{
    public string Type { get { return this.GetType().ToString(); } }
    [SerializeField]
    [TextArea]
    string description;
}
