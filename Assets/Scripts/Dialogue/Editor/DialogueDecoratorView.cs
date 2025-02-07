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

    public abstract void UpdateFields();
}


public class QuestNodeView : DialogueDecoratorView
{
    public QuestNodeView(DialogueDecorator dialogueDecorator) : base(dialogueDecorator)
    {
        
        var field = new ObjectField("Quest");
        field.objectType = typeof(Quest);
        field.value = (dialogueDecorator as GiveQuest).quest;
        field.RegisterValueChangedCallback((evt) =>
        {
            (dialogueDecorator as GiveQuest).quest = evt.newValue as Quest;

            AssetDatabase.SaveAssets();
        });
        contents.Add(field);

    }

    public override void UpdateFields()
    {
        throw new System.NotImplementedException();
    }
}

public class DialogueEventNodeView : DialogueDecoratorView
{
    TextField eventField;
    public DialogueEventNodeView(DialogueDecorator dialogueDecorator) : base(dialogueDecorator)
    {
        eventField = new TextField();
        Label label = new Label("Event Name");
        contents.Add(label);
        contents.Add(eventField);

        eventField.value = (dialogueDecorator as DialogueEventNode).dialogueEvent.name;
        eventField.RegisterValueChangedCallback(evt =>
        {
            (dialogueDecorator as DialogueEventNode).dialogueEvent.name = evt.newValue;
            EditorUtility.SetDirty((dialogueDecorator as DialogueEventNode).dialogueEvent);
            AssetDatabase.SaveAssets();
            
            //(parent.parent as DialogueGraphView).UpdateValues();
            //.SetDirty(); 
        });

    }

    public override void UpdateFields()
    {
        
        eventField.value = (dialogueDecorator as DialogueEventNode).dialogueEvent.name;
    }


}
