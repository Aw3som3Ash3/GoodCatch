using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Callbacks;
using UnityEditor;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Unity.VisualScripting;
using NUnit.Framework;

public class DialogueTreeEditor : EditorWindow
{
    static DialogueTreeEditor wnd;

    DialogueGraphView graphView;

    TwoPaneSplitView twoPaneSplitView;
    DialogueInspector inspector;
    

    [SerializeReference]
    Dialogue dialogueTree;
    // Start is called before the first frame update
    [MenuItem("Examples/My Editor Window")]
    static void OpenWindow(Dialogue dialogue)
    {

        
       
       
        wnd = GetWindow<DialogueTreeEditor>();
        wnd.dialogueTree = dialogue;
        wnd.Show();
        wnd.titleContent = new GUIContent("Dialogue Editor");
        Debug.Log(wnd);
        
        wnd.GenerateGraph();
        

    }

    public DialogueTreeEditor()
    {

    }
    
    void GenerateGraph()
    {

        
        if (dialogueTree != null)
        {
            if (twoPaneSplitView != null)
            {
                rootVisualElement.Remove(twoPaneSplitView);
                rootVisualElement.MarkDirtyRepaint();
            }
            graphView = new DialogueGraphView();
            graphView.Setup(dialogueTree);
            inspector = new DialogueInspector(dialogueTree);

            twoPaneSplitView = new(1, 1000, TwoPaneSplitViewOrientation.Horizontal);
            twoPaneSplitView.Add(inspector);
            twoPaneSplitView.Add(graphView);

            rootVisualElement.Add(twoPaneSplitView);
        }
    }
    private void CreateGUI()
    {
        wnd.GenerateGraph();

    }

  


    [OnOpenAsset]
    public static bool OnOpen(int instanceID)
    {
        if (AssetDatabase.GetMainAssetTypeAtPath(AssetDatabase.GetAssetPath(instanceID)) == typeof(Dialogue))
        {
            Dialogue tree = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            Debug.Log(tree);
            
            OpenWindow(tree);
            // We can open MyAssetHandler asset using MyAssetHandler opening method


            return true;
        }
        else return false; // The passed instance doesn't belong to MyAssetHandler type asset so we won't be able to open it using opening method inside MyAssetHandler
    }
}


public class DialogueInspector : InspectorElement
{

    Button addEvent=new();
    Dialogue dialogue;
    ListView list;

    public  DialogueInspector(Dialogue dialogue)
    {
        this.dialogue = dialogue;
        addEvent.text = "Add Event";
        Add(addEvent);
        addEvent.clicked += AddEvent;
        var serializedObject = new UnityEditor.SerializedObject(dialogue);

        //PropertyField field = new PropertyField(serializedObject.FindProperty("events"),"Events");

        list = new ListView(dialogue.Events,25,makeItem:()=>new EventViewer(),bindItem:(elem,index)=>(elem as EventViewer).SetEvent(dialogue.Events[index]));
       
        
        //list.BindProperty(serializedObject.FindProperty("events"));
        

        Debug.Log(serializedObject.FindProperty("events"));
        //Debug.Log(field);
        Add(list);

    }

    private void AddEvent()
    {
        dialogue.CreateEvent();
        list.Rebuild();
        //throw new NotImplementedException();
    }


    public class EventViewer : VisualElement
    {
        DialogueEvent dialogueEvent;
        TextField title=new();
        Button delete=new();
        Action<DialogueEvent> Delete;
        public EventViewer() 
        {
            //dialogueEvent= @event;
            style.flexDirection = FlexDirection.Row;
            style.alignContent = Align.Stretch;
            Add(title);
            delete.text = "Remove";
            title.style.flexGrow = 1;
            title.RegisterValueChangedCallback((evt) => { if (dialogueEvent != null) { dialogueEvent.name = evt.newValue;EditorUtility.SetDirty(dialogueEvent); AssetDatabase.SaveAssets(); } });
            Add(delete);

        }
        public void SetEvent(DialogueEvent @event)
        {
            dialogueEvent = @event;
            title.value = dialogueEvent.name;
            delete.clicked += () => Delete?.Invoke(dialogueEvent);
        }
    }
}




