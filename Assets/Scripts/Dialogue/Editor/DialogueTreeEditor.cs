using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Callbacks;
using UnityEditor;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.Internal;
using UnityEditor.ShortcutManagement;
using Unity.VisualScripting;

public class DialogueTreeEditor : EditorWindow
{
    static DialogueTreeEditor wnd;

    DialogueGraphView graphView;

    TwoPaneSplitView twoPaneSplitView;
    VisualElement leftPane, rightPane;
    DialogueInspector inspector;
    Toolbar toolbar;
    ToolbarButton saveButton;

    [SerializeReference]
    Dialogue dialogueTree;
    // Start is called before the first frame update



    

    [MenuItem("Examples/My Editor Window")]
    static void OpenWindow(Dialogue dialogue)
    {

        //wnd.Show(dialogue);
        //wnd = new();

        //wnd = new();
        wnd = GetWindow<DialogueTreeEditor>();
        wnd.dialogueTree = dialogue;
        
        wnd.titleContent = new GUIContent("Dialogue Editor");
        Debug.Log(wnd);
        wnd.GenerateGraph();
        wnd.Repaint();
        
        wnd.EndWindows();

    }

    public DialogueTreeEditor()
    {
       
    }
    
    void GenerateGraph()
    {

       
        if (dialogueTree != null)
        {
            if (graphView != null)
            {
                rightPane.Remove(graphView);
                
            }
            if (inspector != null)
            {
                leftPane.Remove(inspector);

            }
            graphView = new DialogueGraphView();
            graphView.Setup(dialogueTree);
            graphView.OnEdited += () => hasUnsavedChanges = true;
            inspector = new DialogueInspector(dialogueTree);
            inspector.OnEdited += () => hasUnsavedChanges = true;
            rightPane.Add(graphView);
            leftPane.Add(inspector);
            

            rootVisualElement.Add(twoPaneSplitView);
            EditorUtility.SetDirty(this);
        }
    }
    private void CreateGUI()
    {
        leftPane = new();
        rightPane = new();
        toolbar = new();
        rootVisualElement.Add(toolbar);
        saveButton = new();
        saveButton.text = "Save";
        saveButton.clicked+=SaveChanges;
        toolbar.Add(saveButton);
        twoPaneSplitView = new(1, 1000, TwoPaneSplitViewOrientation.Horizontal);
        twoPaneSplitView.Add(leftPane);
        twoPaneSplitView.Add(rightPane);
        GenerateGraph();
        hasUnsavedChanges = false;

    }
    private void OnGUI()
    {
        if (saveButton != null)
        {
            saveButton.text = hasUnsavedChanges ? "*Save" : "Save";
        }
       
    }
    [Shortcut("Dialogue Editor/Save")]
    public override void SaveChanges()
    {
       
        graphView.SaveAll();
        EditorUtility.SetDirty(dialogueTree);
        AssetDatabase.SaveAssets();
        base.SaveChanges();

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
    public event Action OnEdited;

    public  DialogueInspector(Dialogue dialogue)
    {
        this.dialogue = dialogue;
        addEvent.text = "Add Event";
        Add(addEvent);
        addEvent.clicked += AddEvent;
        var serializedObject = new UnityEditor.SerializedObject(dialogue);

        //PropertyField field = new PropertyField(serializedObject.FindProperty("events"),"Events");

        list = new ListView(dialogue.Events,25,makeItem:()=> { var item = new EventViewer(); item.OnEdited += OnEdited; return item; },bindItem:(elem,index)=>(elem as EventViewer).SetEvent(dialogue.Events[index]));
       
        
        //list.BindProperty(serializedObject.FindProperty("events"));
        

        Debug.Log(serializedObject.FindProperty("events"));
        //Debug.Log(field);
        Add(list);

    }

    private void AddEvent()
    {
        dialogue.CreateEvent();
        OnEdited?.Invoke();
        list.Rebuild();
        //throw new NotImplementedException();
    }


    public class EventViewer : VisualElement
    {
        DialogueEvent dialogueEvent;
        TextField title=new();
        Button delete=new();
        Action<DialogueEvent> Delete;
        public event Action OnEdited;
        public EventViewer() 
        {
            //dialogueEvent= @event;
            style.flexDirection = FlexDirection.Row;
            style.alignContent = Align.Stretch;
            Add(title);
            delete.text = "Remove";
            title.style.flexGrow = 1;
            
            Add(delete);

        }
        public void SetEvent(DialogueEvent @event)
        {
            dialogueEvent = @event;
            title.value = dialogueEvent.name;
            title.RegisterValueChangedCallback((evt) => { if (dialogueEvent != null) { dialogueEvent.name = evt.newValue; OnEdited?.Invoke(); EditorUtility.SetDirty(dialogueEvent); } });
            delete.clicked += () => Delete?.Invoke(dialogueEvent);
        }
        public void Save()
        {
            
            

        }
    }
}




