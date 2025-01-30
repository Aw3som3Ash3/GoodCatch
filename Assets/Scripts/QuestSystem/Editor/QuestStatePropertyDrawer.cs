using Codice.CM.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(Quest.QuestState),true)]
public class QuestStatePropertyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement root = new();

        var name = new PropertyField(property.FindPropertyRelative("name"));
        var desctription = new PropertyField(property.FindPropertyRelative("description"));
        

        root.Add(name);
        root.Add(desctription);
        Button setRequirement = new();
        Label title = new Label();
        title.text = "Requirements";
        setRequirement.text = "Add Requirement";
        setRequirement.clicked += () => ShowDropdown((requirment) => 
        {
            //property.FindPropertyRelative("requirments").managedReferenceValue = requirment;
            var requirmentProperty = property.FindPropertyRelative("requirements");
            int dest = requirmentProperty.arraySize;
            requirmentProperty.InsertArrayElementAtIndex(dest);
            requirmentProperty.GetArrayElementAtIndex(dest).managedReferenceValue = requirment;
            //requirmentProperty.array
            //Quest.QuestRequirment[] requirmentsData = val as CatchNumOfFishRequirement[];
            //requirmentsData.Append(requirment);

            //requirmentsData = requirment;
            //Debug.Log(requirmentsData);
            //Debug.Log(property.FindPropertyRelative("requirments").managedReferenceValue);
            property.serializedObject.ApplyModifiedProperties();
            //Debug.Log(property.FindPropertyRelative("requirments").managedReferenceValue);

        });
       
        root.Add(setRequirement);
        root.Add(title);
        var requirmentProperty = property.FindPropertyRelative("requirements");
        
        
        var requirmentsList= new PropertyField(property.FindPropertyRelative("requirements"));
        Debug.Log(requirmentProperty);
        //var requirmentsList= new ListView(requirmentsData);
        root.Add(requirmentsList);

        //return base.CreatePropertyGUI(property);
        return root;
    }

    void ShowDropdown(Action<Quest.QuestRequirement> callback)
    {
        GenericMenu menu = new GenericMenu();
        foreach(var requirment in Assembly.GetAssembly(typeof(Quest.QuestRequirement)).GetTypes().Where((type)=>type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Quest.QuestRequirement))))
        {
            var type = requirment;
            menu.AddItem(new GUIContent(requirment.Name), false, () => 
            {
                var requirmentData = Activator.CreateInstance(type) as Quest.QuestRequirement;
                Debug.Log(requirmentData);  
                callback?.Invoke(requirmentData); 
            });
        }
        menu.ShowAsContext();
    }
}


//[CustomPropertyDrawer(typeof(CatchNumOfFishRequirement), true)]
//public class QuestRequirmentPropertyDrawer : PropertyDrawer
//{
//    public override VisualElement CreatePropertyGUI(SerializedProperty property)
//    {
//        VisualElement root = new();
//        var name = new PropertyField(property.FindPropertyRelative("targetOfFish"));
//        root.Add(name);
//        return root;
//    }

// }
