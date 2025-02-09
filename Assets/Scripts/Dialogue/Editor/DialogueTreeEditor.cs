using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Callbacks;
using UnityEditor;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class DialogueTreeEditor : EditorWindow
{
    static DialogueTreeEditor wnd;

    DialogueGraphView graphView;

    [SerializeReference]
    Dialogue dialogueTree;
    // Start is called before the first frame update
    [MenuItem("Examples/My Editor Window")]
    void OpenWindow(Dialogue dialogue)
    {
        dialogueTree = dialogue;
        //wnd = GetWindow<DialogueTreeEditor>();
        Show();
        wnd.titleContent = new GUIContent("Dialogue Editor");
        Debug.Log(wnd);
        GenerateGraph();


    }
    void GenerateGraph()
    {
        if (graphView != null)
        {
            rootVisualElement.Remove(graphView);
            
        }
        Debug.Log(dialogueTree);
        graphView = new DialogueGraphView();
        graphView.Setup(dialogueTree);
        rootVisualElement.Add(graphView);
    }
    private void CreateGUI()
    {
        if (dialogueTree != null)
        {
            GenerateGraph();
        }
        //ShowGraphViewWindowWithTools<DialogueTreeEditor>();
        
    }

  


    [OnOpenAsset]
    public static bool OnOpen(int instanceID)
    {
        if (AssetDatabase.GetMainAssetTypeAtPath(AssetDatabase.GetAssetPath(instanceID)) == typeof(Dialogue))
        {
            Dialogue tree = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            Debug.Log(tree);
            wnd = GetWindow<DialogueTreeEditor>();
            wnd.OpenWindow(tree);
            // We can open MyAssetHandler asset using MyAssetHandler opening method


            return true;
        }
        else return false; // The passed instance doesn't belong to MyAssetHandler type asset so we won't be able to open it using opening method inside MyAssetHandler
    }
}
