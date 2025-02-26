using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


public class DevConsole : MonoBehaviour
{
    // Start is called before the first frame update
    //[SerializeField]
    //List<DevCommand> commands;
    [SerializeField]
    UIDocument document;
    TextField commandField;
    VisualElement root,consolePanel;
    [SerializeField]

    InputAction openConsole;

    Dictionary<string,List<Delegate>> consoleCommands=new();
    
    //HashSet<Commands> consoleCommands = new HashSet<Commands>();

    struct Commands
    {
        public string commandName;
        public Delegate commandAction;
        public Commands(string name,Delegate action)
        {
            commandName = name;
            commandAction = action;
        }
    }

    //List<IUseDevCommands> 
    void Start()
    {

        foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IUseDevCommands))))
        {

            foreach (var method in type.GetMethods().Where((t)=>t.IsStatic&&t.HasAttribute(typeof(DevConsoleCommand))))
            {
                var attr = method.GetCustomAttribute<DevConsoleCommand>();
                if (consoleCommands.ContainsKey(attr.CommnadName))
                {
                    consoleCommands[attr.CommnadName].Add(CommandInvoker(method));
                }
                else
                {
                    consoleCommands.Add(attr.CommnadName,new() { CommandInvoker(method) });
                }
                
            }
        };
        //consoleCommands
        root = document.rootVisualElement;
        consolePanel = root.Q("ConsolePanel");
        commandField =root.Q<TextField>("CommandField");
        commandField.value = null;

        commandField.RegisterCallback<NavigationSubmitEvent>((evt) => DoCommand(commandField.value));
        consolePanel.visible = false;

        openConsole.performed += ToggleConsole;
        openConsole.Enable();
    }

    private void ToggleConsole(InputAction.CallbackContext context)
    {
       
        consolePanel.visible = !consolePanel.visible; 
        if (consolePanel.visible)
        {
            InputManager.DisablePlayer();
            root.schedule.Execute(() => commandField.Focus()).StartingIn(10);
        }
        else
        {
            InputManager.EnablePlayer();
        }
       
        /*UnityEngine.Cursor.visible = true; UnityEngine.Cursor.lockState = CursorLockMode.Confined;*/ 

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
        //UnityEngine.Cursor.visible = false; 
        //UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        commandField.value = null;
        InputManager.EnablePlayer();
    }

    void RunCommand(string command ,string[] args)
    {
        Debug.Log(args.Length);
        if (consoleCommands.ContainsKey(command))
        {
            consoleCommands[command].Find((x)=> x.Method.GetParameters().Length-1==args.Length) ?.DynamicInvoke(args);
        }
        else
        {
            Debug.Log($"no command of type{command} found");
        }
       
        //commands.First((c)=>c.CommandName==command).RunCommand(args);
    }


    protected Delegate CommandInvoker(MethodInfo method)
    {
        //var instance = Expression.Parameter(type, "instance");
        
        
        //argument.Type.IsArray


        //var method = typeof(T).GetMethod(MethodName)
        //?? throw new Exception($"method {MethodName}, not found on type{typeof(T).Name}");
        
        var paramaters = method.GetParameters();
        MethodCallExpression call=null;
        if (paramaters.Length > 1)
        {
            ParameterExpression[] arguments = new ParameterExpression[paramaters.Length];
            
            UnaryExpression[] expressions =new UnaryExpression[paramaters.Length];
            for (int i = 0; i < paramaters.Length; i++)
            {
                arguments[i] = Expression.Parameter(typeof(string), "arg"+i);
                expressions[i] = Expression.Convert(arguments[i], paramaters[i].ParameterType);
                Debug.Log("arugument " + i + arguments[i]);
            }
            call = Expression.Call(method, expressions);
            return Expression.Lambda(call, arguments).Compile();
           
        }
        //else if (paramaters[0].ParameterType.IsArray)
        //{
        //    paramaters[0].arr
        //    var argument = Expression.Parameter(typeof(object), "args");

        //    call = Expression.Call(method, Expression.Convert(argument, paramaters[0].ParameterType));
        //    return Expression.Lambda(call, argument).Compile();
        //}
        else
        {
            var argument = Expression.Parameter(typeof(object), "args");

            call = Expression.Call(method, Expression.Convert(argument, paramaters[0].ParameterType));
            return Expression.Lambda(call, argument).Compile();
        }

       
       
       



    }

}
[System.AttributeUsage(System.AttributeTargets.Method)] // Multiuse attribute.

public class DevConsoleCommand: System.Attribute
{
    string commnadName;
    public string CommnadName => commnadName;
    int maxParamaters = -1;
    public int MaxParamaters => maxParamaters;
    public DevConsoleCommand(string commandName)
    {
        this.commnadName = commandName;
        
    }
    public DevConsoleCommand(string commandName,int maxParamaters)
    {
        this.commnadName = commandName;
        this.maxParamaters = maxParamaters;

    }




}

public interface IUseDevCommands
{
    public IEnumerable<DevConsoleCommand> GetCommands()
    {

        TypeInfo typeInfo = typeof(IUseDevCommands).GetTypeInfo();
        var attrs = typeInfo.GetCustomAttributes();
        return attrs.Where((x)=>x is DevConsoleCommand).Cast<DevConsoleCommand>();
    }
}


public interface IDevCommand
{


    string MethodName { get; }
    string CommandName { get; }

    public abstract void RunCommand(string[] args);



}



