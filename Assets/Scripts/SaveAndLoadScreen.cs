using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SaveAndLoadScreen : VisualElement
{
    Button saveButton;
    TextField saveField;
    ListView fileList;
    FileInfo[] files;
    Mode currentMode;
    public enum Mode
    {
        save, load
    }
    public new class UxmlFactory : UxmlFactory<SaveAndLoadScreen, CombatUI.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    public SaveAndLoadScreen()
    {
        Init();
        //
        SetList();

    }
    void Init()
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/SaveAndLoadScreen");
        visualTreeAsset.CloneTree(root);
        //saveField = this.Q<TextField>("SaveField");
        //saveButton = this.Q<Button>("SaveButton");
        fileList = this.Q<ListView>("SaveList");
        fileList.selectionChanged += FileList_selectionChanged;
        //saveButton.clicked += OnSaveOrLoad;
    }

    private void OnSaveOrLoad()
    {
        if (currentMode == Mode.save)
        {
            SavingSystem.SaveGame(SavingSystem.SaveMode.ManualSave);
            
        }else if (currentMode == Mode.load)
        {
            SavingSystem.LoadGame(saveField.value);
        }
        DisplaySaves();
    }

    private void FileList_selectionChanged(IEnumerable<object> obj)
    {
        Debug.Log(fileList.selectedItem);
        var file= fileList.selectedItem as FileInfo;  
        SavingSystem.LoadGame(Path.GetFileNameWithoutExtension(file.Name));
        
    }

    void SetList()
    {
        fileList.makeItem = () =>
        {
            return new Label();
        };

        // Set up bind function for a specific list entry
        fileList.bindItem = (item, index) =>
        {

            (item as Label).text = Path.GetFileNameWithoutExtension(files[index].Name);

        };


        // Set a fixed item height matching the height of the item provided in makeItem. 
        // For dynamic height, see the virtualizationMethod property.
        fileList.fixedItemHeight = 45;
    }
    void DisplaySaves()
    {
        string path = Path.Combine(Application.persistentDataPath, SavingSystem.SavePath);
        var directoryInfo= new DirectoryInfo(path);
        files = directoryInfo.GetFiles("*.Data");

        // Set the actual item's source list/array
        fileList.itemsSource = files.OrderByDescending((x)=>x.LastWriteTime).ToList();
        
        //fileList.Rebuild();
        
    }
    public void DisplaySaves(Mode mode)
    {
        DisplaySaves();

        currentMode=mode;
        //if (mode == Mode.save)
        //{
        //    saveButton.text = "SAVE";
        //    saveField.SetEnabled(true);
        //}
        //else
        //{
        //    //saveButton.text = "LOAD";
        //   // saveField.SetEnabled(false);
        //}


    }


}
