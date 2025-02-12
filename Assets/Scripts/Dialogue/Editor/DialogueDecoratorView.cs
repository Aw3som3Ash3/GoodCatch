using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class DialogueDecoratorView : Node
{
    protected VisualElement contents;
    public DialogueDecorator dialogueDecorator;
    public Port input;
    public event Action OnEdited;
    public DialogueDecoratorView(DialogueDecorator dialogueDecorator) : base("Assets/Scripts/Dialogue/Editor/DialogueDecoratorView.uxml")
    {
        this.dialogueDecorator = dialogueDecorator;
        contents = this.Q("contents");
        this.viewDataKey = dialogueDecorator.guid;
        UseDefaultStyling();
        this.Q<Label>("title-label").text = dialogueDecorator.GetType().Name;
        input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(DialogueDecorator));
        input.portName = "";
        input.style.alignSelf = Align.Center;
        inputContainer.Add(input);
        
    }
    protected void NodeModified()
    {
        OnEdited?.Invoke();
    } 
    public abstract void Save();
    public abstract void UpdateFields();
}


public class QuestNodeView : DialogueDecoratorView
{
    public QuestNodeView(DialogueDecorator dialogueDecorator) : base(dialogueDecorator)
    {
        
        var field = new ObjectField("Quest");
        field.objectType = typeof(Quest);
        field.value =  (dialogueDecorator as GiveQuestDecorator).quest;
        field.RegisterValueChangedCallback((evt) =>
        {
            
            (dialogueDecorator as GiveQuestDecorator).quest = evt.newValue as Quest;
            NodeModified();
            //AssetDatabase.SaveAssets();
        });
        contents.Add(field);

    }

    public override void Save()
    {
        AssetDatabase.SaveAssets();
    }

    public override void UpdateFields()
    {
        //throw new System.NotImplementedException();
    }
}

public class DialogueEventNodeView : DialogueDecoratorView
{
    public DialogueEventNodeView(DialogueDecorator dialogueDecorator) : base(dialogueDecorator)
    {
        
        Label label = new Label((dialogueDecorator as DialogueEventDecorator).dialogueEvent.name);
        contents.Add(label);
        

    }

    public override void Save()
    {
        //throw new System.NotImplementedException();
        EditorUtility.SetDirty((dialogueDecorator as DialogueEventDecorator).dialogueEvent);
        //AssetDatabase.SaveAssets();
    }

    public override void UpdateFields()
    {
        
        
    }


}

public class CompleteArbritaryQuestDecoratorView : DialogueDecoratorView
{

    TextField textField = new();
    public CompleteArbritaryQuestDecoratorView(DialogueDecorator dialogueDecorator) : base(dialogueDecorator)
    {
        var completeArbritaryQuestDecorator = dialogueDecorator as CompleteArbritaryQuestDecorator;
        textField.label = "Requirement Tag";
        textField.value= completeArbritaryQuestDecorator.questTag;
        textField.RegisterValueChangedCallback((evt) => { completeArbritaryQuestDecorator.questTag = evt.newValue; NodeModified(); });
        contents.Add(textField);



    }

    public override void Save()
    {
        //throw new NotImplementedException();
    }

    public override void UpdateFields()
    {
        //throw new NotImplementedException();
    }
}
