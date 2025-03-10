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
    public ObjectField objectField;
    public Port input;
    public Port decoratorPort;
    public Action<DialogueNodeView> OnNodeSeletected;
    public DialogueNode dialogueNode { get; protected set; }
    public event Action OnEdited;

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
        objectField = new("Voice Clip");
        objectField.objectType =  typeof(AudioClip);
        objectField.value = dialogueNode.voiceClip;
        objectField.RegisterValueChangedCallback((evt) =>
        {
            dialogueNode.voiceClip=evt.newValue as AudioClip;
            NodeModified();
        });
        this.Q("extra").Add(objectField);
        dialogueField = this.Q<TextField>("dialogue");
        dialogueField.multiline = true;
        dialogueField.style.whiteSpace = WhiteSpace.Normal;
        dialogueField.value = dialogueNode.dialouge;
        dialogueField.RegisterValueChangedCallback((s) => 
        { 
            dialogueNode.dialouge = s.newValue;
            NodeModified();
            //AssetDatabase.SaveAssets(); 
        }) ;

        input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(DialogueNode));
        decoratorPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(DialogueDecorator));
        this.Q("decorator-output").Add(decoratorPort);
        decoratorPort.portName = "";
        decoratorPort.style.alignSelf = Align.Center;

        input.allowMultiDrag = true;
        //input.
        //Debug.Log(input.IsSnappable());
        //input.
        input.portName = "input";
        inputContainer.Add(input);

        

    }
    protected void NodeModified()
    {
        OnEdited?.Invoke();
    }
    public virtual void UpdateFields()
    {

        dialogueField.value = dialogueNode.dialouge;
    }

    public abstract void Save();

    
  
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

    public override void Save()
    {
        //throw new NotImplementedException();
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
                NodeModified();
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
            NodeModified();
            //AssetDatabase.SaveAssets(); 
        });
        outputContainer.Add(branchView);
    }

    public override void Save()
    {
        
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
    public void Save()
    {

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
