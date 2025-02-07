using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System;
using static BranchingDialogue;

public class DialogueGraphView : GraphView
{

    Dialogue dialogueTree;
    public Dictionary<Port, Port> outputToPort;
    // Start is called before the first frame update

    public DialogueGraphView() 
    {

        //this.StretchToParentSize();
        Insert(0, new GridBackground());
        //

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Dialogue/Editor/DialogueGraphView.uss");
        styleSheets.Add(styleSheet);
       
        
        
    }
    

    public void Setup(Dialogue dialogue)
    {
        
        this.dialogueTree= dialogue;
        graphViewChanged += OnGraphViewChanged;
        Debug.Log(dialogueTree.nodes.Count);
        CreateRootNodeView(dialogueTree.rootNode);

        foreach (var node in dialogueTree.nodes)
        {
            CreateNodeView(node, node.position);
        }
        var rootNodeView= GetNodeByGuid(dialogueTree.rootNode.guid) as RootNodeView;
        if (dialogueTree.rootNode.nextNode != null)
        {
           AddElement(rootNodeView.output.ConnectTo(FindNodeView(dialogueTree.rootNode.nextNode).input));
        }
        foreach (var decorator in dialogueTree.decorators)
        {
            CreateDecoratorView(decorator, decorator.position);
        }


        foreach (var node in dialogueTree.nodes)
        {
            DialogueNodeView parentView = FindNodeView(node);
            //DialogueNodeView childView=null;
            if(node is BasicDialogue)
            {
                if (((BasicDialogue)node).nextNode != null)
                {
                    var childView = FindNodeView(((BasicDialogue)node).nextNode);
                    AddElement((parentView as DialogueLineNodeView).output.ConnectTo(childView.input));
                }
               
            }
            else if(node is BranchingDialogue)
            {
                var decisions = (node as BranchingDialogue).decisions;
                for (int i=0;i < decisions.Count;i++)
                {
                    if (decisions[i].choiceNode != null)
                    {
                        var input = FindNodeView(decisions[i].choiceNode).input;
                        Port output = parentView.outputContainer.Children().ToArray()[i].Q<Port>();
                        AddElement(output.ConnectTo(input));
                    }
                        
                }
               

            }


            foreach(var decorator in node.decorators)
            {
                var childView = FindDecoratorView(decorator);
                Debug.Log(childView);
                AddElement(parentView.decoratorPort.ConnectTo(childView.input));

            }
            


        }
        
    }

    private void CreateRootNodeView(Dialogue.StartNode rootNode)
    {
        var nodeView = new RootNodeView(rootNode);
        AddElement(nodeView);
        nodeView.SetPosition(new Rect(rootNode.position, new Vector2(4, 2)));
    }
    DialogueDecoratorView FindDecoratorView(DialogueDecorator decorator)
    {
        return GetNodeByGuid(decorator.guid) as DialogueDecoratorView;
    }
    DialogueNodeView FindNodeView(DialogueNode node)
    {

        return GetNodeByGuid(node.guid) as DialogueNodeView;
    }
    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.movedElements != null)
        {
            foreach (Node node in graphViewChange.movedElements)
            {
                if(node is DialogueNodeView)
                {
                    (node as DialogueNodeView) .dialogueNode.position = node.GetPosition().position;
                }

                if (node is RootNodeView)
                {
                    (node as RootNodeView).rootNode.position = node.GetPosition().position;
                }
                if(node is DialogueDecoratorView)
                {
                    (node as DialogueDecoratorView).dialogueDecorator.position = node.GetPosition().position;
                }
            }
            
            
        }
       
        if (graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach(elem =>
            {
                DialogueNodeView nodeView = elem as DialogueNodeView;
                if (nodeView != null)
                {
                    dialogueTree.DeleteNode(nodeView.dialogueNode);
                    //nodeView.OnNodeEdited -= GraphEdited;
                }
                DialogueDecoratorView decoratorView = elem as DialogueDecoratorView;
                if (decoratorView != null)
                {
                    dialogueTree.DeleteDecorator(decoratorView.dialogueDecorator);
                }

                Edge edge = elem as Edge;
                if (edge != null)
                {
                    DialogueNodeView parentView = edge.output.node as DialogueNodeView;
                    DialogueNodeView childView = edge.input.node as DialogueNodeView;
                    DialogueDecoratorView childDecoratorView = edge.input.node as DialogueDecoratorView;
                   
                    if(edge.output.node is RootNodeView)
                    {
                        (edge.output.node as RootNodeView).rootNode.nextNode = null;
                    }
                    if (parentView != null && parentView.dialogueNode is BasicDialogue)
                    {
                        if (childView != null) 
                        {
                            (parentView.dialogueNode as BasicDialogue).nextNode = null;
                        }
                        
                       
                    }
                    else if (parentView != null && parentView is DialogueBranchNodeView)
                    {
                        if (childView != null)
                        {
                            foreach (DialogueBranchView branchView in (parentView as DialogueBranchNodeView).outputContainer.Children())
                            {
                                if (branchView.output == edge.output)
                                {
                                    branchView.decision.choiceNode = null;
                                }

                            }
                        }
                            
                    }
                    if (parentView != null && childDecoratorView != null)
                    {
                        parentView.dialogueNode.decorators.Remove(childDecoratorView.dialogueDecorator);
                    }

                    //childView.dialogueNode.parent = null;
                    //dialogueTree.RemoveChild(parentView.dialogueNode, childView.dialogueNode, parentView, edge.output);
                }

            });
        }


        if (graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(edge =>
            {
                DialogueNodeView parentView = edge.output.node as DialogueNodeView;
                DialogueNodeView childView = edge.input.node as DialogueNodeView;
                DialogueDecoratorView childDecoratorView = edge.input.node as DialogueDecoratorView;
                //Decision decisionPort = edge.output.node as DecisionPort;
                if (edge.output.node is RootNodeView)
                {
                    (edge.output.node as RootNodeView).rootNode.nextNode = childView.dialogueNode;
                    Debug.Log((edge.output.node as RootNodeView).rootNode.nextNode);
                }
                if (parentView !=null && parentView.dialogueNode is BasicDialogue)
                {
                    if(childView != null)
                    {
                        (parentView.dialogueNode as BasicDialogue).nextNode = childView.dialogueNode;
                    }
                    
                } 
                else if(parentView != null && parentView is DialogueBranchNodeView)
                {
                    foreach (DialogueBranchView branchView in (parentView as DialogueBranchNodeView).outputContainer.Children())
                    {
                        if(branchView.output == edge.output)
                        {
                            
                            branchView.decision.choiceNode = childView.dialogueNode;
                        }
                        
                    }
                }
                if (parentView != null && childDecoratorView != null)
                {
                    parentView.dialogueNode.decorators.Add(childDecoratorView.dialogueDecorator);
                }

                //dialogueTree.AddChild(parentView.dialogueNode, childView.dialogueNode, parentView, edge.output);
                //childView.dialogueNode.parent = parentView.dialogueNode;
                //dialogueTree.AddChild(, );
            });
        }
        UpdateValues();
        EditorUtility.SetDirty(dialogueTree);
        AssetDatabase.SaveAssets();
        return graphViewChange;
        //throw new NotImplementedException();
    }
    public void UpdateValues()
    {
        foreach (var node in nodes)
        {
            if (node is DialogueNodeView)
            {
                (node as DialogueNodeView).UpdateFields();
            }

        }
    }
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        base.BuildContextualMenu(evt);
        var types = TypeCache.GetTypesDerivedFrom<DialogueNode>();
        Vector2 mousePos = MouseToContent(evt.localMousePosition);
       

        foreach (var type in types)
        {
            Debug.Log(evt.localMousePosition);

            evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type, mousePos));

        }
        foreach (var type in TypeCache.GetTypesDerivedFrom<DialogueDecorator>())
        {
            if (type == typeof(DialogueEventNode))
            {
                continue;
            }
            evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateDecorator(type, mousePos));

        }
        foreach (var _event in dialogueTree.Events)
        {
            var eventNode = dialogueTree.CreateDecorator(typeof(DialogueEventNode), mousePos)as DialogueEventNode;
            eventNode.SetEvent(_event);
            evt.menu.AppendAction($"[Event] {_event.name}", (a)=>CreateDecoratorView(eventNode, mousePos));
        }
        

    }
    public override EventPropagation DeleteSelection()
    {
        selection.Remove(selection.Find((x) => x is RootNodeView));
        return base.DeleteSelection();
    }
    private Vector2 MouseToContent(Vector2 position)
    {
        
        position.x = (position.x - contentViewContainer.localBound.x) / scale;
        position.y = (position.y - contentViewContainer.localBound.y) / scale;
        return position;
    }

    private void CreateNode(Type type, Vector2 mousePos)
    {

        DialogueNode node = dialogueTree.CreateNode(type, mousePos);
        CreateNodeView(node, mousePos);
    }
    private void CreateDecorator(Type type, Vector2 mousePos)
    {
        DialogueDecorator decorator = dialogueTree.CreateDecorator(type, mousePos);
        CreateDecoratorView(decorator, mousePos);
    }

    void CreateDecoratorView(DialogueDecorator decorator, Vector2 mousePos)
    {
        DialogueDecoratorView decoratorView=null;
        if (decorator is DialogueGiveItemNode)
        {
            decoratorView = new DialogueGiveItemNodeView(decorator);
        }
        else
            if (decorator is GiveQuest)
        {
            decoratorView = new QuestNodeView(decorator);
        }
        else if (decorator is DialogueEventNode)
        {
            decoratorView = new DialogueEventNodeView(decorator);
        }
        if (decoratorView != null)
        {
            AddElement(decoratorView);
        }

        decoratorView.SetPosition(new Rect(mousePos, new Vector2(4, 2)));
    }
    void CreateNodeView(DialogueNode dialogueNode, Vector2 mousePos)
    {
        //var nodeView = new Node();
        DialogueNodeView nodeView = null;
        
        if (dialogueNode is BasicDialogue)
        {

            nodeView = new DialogueLineNodeView(dialogueNode);
        }
        else if (dialogueNode is BranchingDialogue)
        {
            nodeView = new DialogueBranchNodeView(dialogueNode);
        }


        
        nodeView.SetPosition(new Rect(mousePos,new Vector2(4,2)));
        //Debug.Log("should create node");
        //DialogueNodeView nodeView = new DialogueNodeView(dialogueNode);
        //nodeView.OnNodeSeletected = OnNodeSeletected;
        //nodeView.OnNodeEdited += GraphEdited;
        AddElement(nodeView);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
    }
}
