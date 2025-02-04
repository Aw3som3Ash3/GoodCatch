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
    [SerializeField]
    //[SerializeReference]
    public StartNode rootNode;
    [SerializeField]
    [SerializeReference]
    public List<DialogueNode> nodes;

    //public List<Action> events { get; private set; }

#if UNITY_EDITOR

    private void OnEnable()
    {
        //if (rootNode == null)
        //{
        //    CreateRoot();
            
        //    nodes = new();
        //}
        //if (rootNode.guid == null && rootNode.guid != "root")
        //{
        //    rootNode.guid = "root";
        //}
    }

    public DialogueNode CreateNode(Type type,Vector2 pos)
    {
        var node= (Activator.CreateInstance(type) as DialogueNode).SetTree(this);
        node.guid = GUID.Generate().ToString();
        if(node is DialogueEventNode)
        {
            //events.Add((node as DialogueEventNode).Event);
        }
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
        //node.guid = GUID.Generate().ToString();
        node.guid = "root";
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

    [Serializable]
    public class StartNode 
    {
        [SerializeReference]
        public DialogueNode nextNode;
        public string guid;
        public Vector2 position = new();
        public StartNode()
        {
            guid = "root";
        }
    }
}



