using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;




[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue System/DialogueTree", order = 2)]
public class Dialogue : ScriptableObject
{
    [SerializeReference]
    public StartNode rootNode;
    [SerializeReference]
    public List<DialogueNode> nodes;



#if UNITY_EDITOR

    private void OnEnable()
    {
        if (rootNode == null)
        {
            CreateRoot();
        }
    }

    public DialogueNode CreateNode(Type type,Vector2 pos)
    {
        var node= Activator.CreateInstance(type) as DialogueNode;
        node.guid = GUID.Generate().ToString();
        nodes.Add(node);
        node.position = pos;
        
        return node;
    }
    public void DeleteNode(DialogueNode node)
    {
        nodes.Remove(node);
        
    }
    public void CreateRoot()
    {
        var node = new StartNode();
        node.guid = GUID.Generate().ToString();
        rootNode = node;
    }
    //public void MakeRoot(DialogueNode node) 
    //{
    //    if (nodes.Contains(node))
    //    {
    //        rootNode = node;
    //    }
    //}

    //public void AddChild(DialogueNode parent, DialogueNode child)
    //{
    //    child.parent = parent;
    //}


#endif


    public class StartNode 
    {
        [SerializeReference]
        public DialogueNode nextNode;
        public string guid;
        public Vector2 position = new();
    }
}



