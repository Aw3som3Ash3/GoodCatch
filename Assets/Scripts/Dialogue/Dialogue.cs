using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue System/DialogueTree", order = 2)]
public class Dialogue : ScriptableObject
{
    [SerializeField]
    //[SerializeReference]
    public StartNode rootNode;
    [SerializeField]
    [SerializeReference]
    public List<DialogueNode> nodes;
    [SerializeField]
    [SerializeReference]
    public List<DialogueDecorator> decorators;

    [SerializeField]
    [SerializeReference]
    List<DialogueEvent> events;
    public List<DialogueEvent> @Events { get { return events; } }

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
        //if(node is DialogueEventNode)
        //{
        //    events.Add((node as DialogueEventNode).dialogueEvent);
        //}
        nodes.Add(node);
        node.position = pos;
        
        return node;
    }

 

    public T CreateNode<T>( Vector2 pos) where T : DialogueNode
    {
        return CreateNode(typeof(T), pos) as T;
    }
    public DialogueDecorator CreateDecorator(Type type,Vector2 pos)
    {
        var decorator = (Activator.CreateInstance(type) as DialogueDecorator);
        decorator.position = pos;
        decorators.Add(decorator);
        decorator.guid = GUID.Generate().ToString();
        return decorator;
    }
    public DialogueEvent CreateEvent()
    {
        var @event = ScriptableObject.CreateInstance<DialogueEvent>();
        AssetDatabase.AddObjectToAsset(@event, this);
        AssetDatabase.SaveAssets();
        events.Add(@event);
        return @event;
    }
    public void DeleteNode(DialogueNode node)
    {
        nodes.Remove(node);
        
    }
    public void DeleteDecorator(DialogueDecorator decorator)
    {
        decorators.Remove(decorator);

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



