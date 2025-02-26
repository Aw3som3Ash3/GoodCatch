using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


public class DevConsole : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    List<DevCommand> commands;
    [SerializeField]
    UIDocument document;
    TextField commandField;
    VisualElement root;
    [SerializeField]
    InputAction openConsole;
    void Start()
    {
        root = document.rootVisualElement;
        commandField=root.Q<TextField>("CommandField");
        commandField.RegisterCallback<NavigationSubmitEvent>((evt) => DoCommand(commandField.value));
        root.visible = false;

        openConsole.performed += (callback) => { root.visible = true; UnityEngine.Cursor.visible = true; UnityEngine.Cursor.lockState = CursorLockMode.Confined; };
        openConsole.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void DoCommand(string command)
    {
        string[] strings = command.Split(" ");
        RunCommand(strings[0], strings.Skip(1).ToArray());
        root.visible = false;
        UnityEngine.Cursor.visible = false; 
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    void RunCommand(string command ,string[] args)
    {
        commands.First((c)=>c.CommandName==command).RunCommand(args);
    }

}



public interface IDevCommand
{


    string MethodName { get; }
    string CommandName { get; }

    public abstract void RunCommand(string[] args);



}



