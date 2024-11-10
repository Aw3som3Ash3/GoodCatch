using UnityEngine;


public abstract class Item : ScriptableObject
{
    public string Type { get { return this.GetType().ToString(); } }
    [SerializeField]
    Texture2D icon;
    public Texture2D Icon { get { return icon; } }

    [SerializeField]
    [TextArea]
    string description;
    public string Description { get { return description; } }
}
