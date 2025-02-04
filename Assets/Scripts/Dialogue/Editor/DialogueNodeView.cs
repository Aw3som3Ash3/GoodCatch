using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static BranchingDialogue;

public abstract class DialogueNodeView : Node
{
    public TextField dialogueField;
    public Port input;
    public Action<DialogueNodeView> OnNodeSeletected;
    public DialogueNode dialogueNode { get; protected set; }


    //public new class UxmlFactory : UxmlFactory<DialogueNodeView, Node.UxmlTraits> { }
    public DialogueNodeView()
    {

    }
    public DialogueNodeView(DialogueNode dialogueNode) : base("Assets/Scripts/Dialogue/Editor/DialogueNodeView.uxml")
    {
        this.viewDataKey = dialogueNode.guid;
        Debug.Log("data key" +this.viewDataKey);
        this.dialogueNode = dialogueNode;
        UseDefaultStyling();
        this.Q<Label>("title-label").text= dialogueNode.GetType().Name;
        dialogueField = this.Q<TextField>("dialogue");
        dialogueField.value = dialogueNode.dialouge;
        dialogueField.RegisterValueChangedCallback((s) => 
        { 
            dialogueNode.dialouge = s.newValue; 
            AssetDatabase.SaveAssets(); 
        }) ;

        input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(DialogueNode));
        input.allowMultiDrag = true;
        //input.
        //Debug.Log(input.IsSnappable());
        //input.
        input.portName = "input";
        inputContainer.Add(input);

    }


  
}


public class DialogueLineNodeView : DialogueNodeView
{
    public Port output { get; private set; }
    public new class UxmlFactory : UxmlFactory<DialogueLineNodeView, Node.UxmlTraits> { }
    public DialogueLineNodeView()
    {
        

    }
    public DialogueLineNodeView(DialogueNode dialogueNode):base(dialogueNode) 
    {

        output= InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(DialogueNode));
        output.portName = "output";
        outputContainer.Add(output);
        

    }




}


public class DialogueBranchNodeView : DialogueNodeView
{
    Button addOption, removeOption;
    public new class UxmlFactory : UxmlFactory<DialogueBranchNodeView, Node.UxmlTraits> { }
    public DialogueBranchNodeView()
    {


    }
    public DialogueBranchNodeView(DialogueNode dialogueNode) : base(dialogueNode)
    {
        addOption = new Button(AddNewBranch);
        addOption.text = "Add Branch";
        titleButtonContainer.Add(addOption);
       
        if ((dialogueNode as BranchingDialogue).decisions!=null)
        {
            foreach (var branch in (dialogueNode as BranchingDialogue).decisions)
            {
                AddNewBranch(branch);
            }
        }
        
        

    }
    void AddNewBranch()
    {
        var branch = new BranchingDialogue.Decision();
        (dialogueNode as BranchingDialogue).decisions.Add(branch);
        AddNewBranch(branch);

    }
    void AddNewBranch(BranchingDialogue.Decision decision)
    {
        var port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(DialogueNode));
        var branchView = new DialogueBranchView((s) => decision.choice = s.newValue, port,decision ,()=> 
        { 
            (dialogueNode as BranchingDialogue).decisions.Remove(decision); 
            AssetDatabase.SaveAssets(); 
        });
        outputContainer.Add(branchView);
    }

    
}

public class DialogueBranchView : VisualElement
{
    TextField textField = new();
    public Port output { get; private set; }
    public BranchingDialogue.Decision decision { get; private set; }
    public DialogueBranchView(EventCallback<ChangeEvent<string>> eventCallback,Port port, Decision decision,Action OnClose)
    {
        textField.RegisterValueChangedCallback(eventCallback);
        textField.value = decision.choice;
        output = port;
        Add(textField);
        var element = new VisualElement();
        var button = new Button();
        button.clicked += () => { parent.Remove(this); OnClose?.Invoke(); };
        button.text = "X";
        element.Add(output);
        element.Add(button);
        
        element.style.flexDirection = FlexDirection.RowReverse;
        
        Add(element);
        this.decision = decision;
    }
}

public class RootNodeView:Node
{
    public Dialogue.StartNode rootNode { get; protected set; }
    public Port output;

    public RootNodeView(Dialogue.StartNode rootNode): base("Assets/Scripts/Dialogue/Editor/DialogueNodeView.uxml")
    {
        this.rootNode = rootNode;
        this.viewDataKey = rootNode.guid;
        output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(DialogueNode));
        this.Q<Label>("title-label").text = "Root";
        this.Q<TextField>("dialogue").parent.Remove(this.Q<TextField>("dialogue"));
        UseDefaultStyling();
        outputContainer.Add(output);
    }

}


public class QuestNodeView : DialogueLineNodeView
{
    public QuestNodeView(DialogueNode dialogueNode) : base(dialogueNode)
    {
        var field = new ObjectField("Quest");
        field.objectType = typeof(Quest);
        field.RegisterValueChangedCallback((evt) => 
        {
            (dialogueNode as GiveQuest).quest = evt.newValue as Quest;
           
            AssetDatabase.SaveAssets();
        } );
        this.Q("extra").Add(field);

    }
}

public class DialogueEventNodeView : DialogueLineNodeView
{
    public DialogueEventNodeView(DialogueNode dialogueNode) : base(dialogueNode)
    {
        TextField eventField = new TextField();
        Label label = new Label("Event Name");
        this.Q("extra").Add(label);
        this.Q("extra").Add(eventField);
        eventField.RegisterValueChangedCallback(evt => 
        { 
            (dialogueNode as DialogueEventNode).dialogueEvent.name= evt.newValue;
            EditorUtility.SetDirty((dialogueNode as DialogueEventNode).dialogueEvent);
            AssetDatabase.SaveAssets();
            //.SetDirty(); 
        });

    }
}
//public class DecisionPort : Port
//{
//    public BranchingDialogue.Decision decision { get; private set; }
//    public DecisionPort(BranchingDialogue.Decision decision) : base(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(DialogueNode))
//    {
//        this.decision = decision;
//    }

   
//    //public DecisionPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type)
//    //{

//    //}
//}
