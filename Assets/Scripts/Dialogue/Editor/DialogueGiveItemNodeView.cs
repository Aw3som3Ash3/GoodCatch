using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public class DialogueGiveItemNodeView : DialogueLineNodeView
{
    // Start is called before the first frame update
    public DialogueGiveItemNodeView(DialogueNode dialogueNode) : base(dialogueNode)
    {
        var field = new ObjectField("Item");
        field.objectType = typeof(Item);
        field.value = (dialogueNode as DialogueGiveItemNode).item;
        field.RegisterValueChangedCallback((evt) =>
        {
            (dialogueNode as DialogueGiveItemNode).item= evt.newValue as Item;

            AssetDatabase.SaveAssets();
        });
        this.Q("extra").Add(field);
        IntegerField integerField = new IntegerField("Amount");
        integerField.value= (dialogueNode as DialogueGiveItemNode).amount;
        integerField.RegisterValueChangedCallback((evt) =>
        {
            (dialogueNode as DialogueGiveItemNode).amount = evt.newValue;

            AssetDatabase.SaveAssets();
        });
        this.Q("extra").Add(integerField);
    }
}
