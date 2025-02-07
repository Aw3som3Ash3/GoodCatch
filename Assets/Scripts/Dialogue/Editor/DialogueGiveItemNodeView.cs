using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public class DialogueGiveItemNodeView : DialogueDecoratorView
{
    // Start is called before the first frame update
    public DialogueGiveItemNodeView(DialogueDecorator decorator) : base(decorator)
    {
        var field = new ObjectField("Item");
        field.objectType = typeof(Item);
        field.value = (decorator as DialogueGiveItemNode).item;
        field.RegisterValueChangedCallback((evt) =>
        {
            (decorator as DialogueGiveItemNode).item= evt.newValue as Item;

            AssetDatabase.SaveAssets();
        });
        contents.Add(field);
        IntegerField integerField = new IntegerField("Amount");
        integerField.value= (decorator as DialogueGiveItemNode).amount;
        integerField.RegisterValueChangedCallback((evt) =>
        {
            (decorator as DialogueGiveItemNode).amount = evt.newValue;

            AssetDatabase.SaveAssets();
        });
        contents.Add(integerField);
    }


    public override void UpdateFields()
    {
        throw new System.NotImplementedException();
    }
}
