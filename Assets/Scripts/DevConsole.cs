using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class DevConsole : MonoBehaviour
{
    // Start is called before the first frame update
    //[SerializeField]
    //List<DevCommand> commands;
#pragma warning disable UDR0001 // Domain Reload Analyzer
    static DevConsole Instance;
#pragma warning restore UDR0001 // Domain Reload Analyzer
    [SerializeField]
    UIDocument document;
    TextField commandField;
    VisualElement root,consolePanel;
    ScrollView consoleLog;
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
    private void Awake()
    {


        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
       

    }
    //List<IUseDevCommands> 
    void Start()
    {

        foreach (var method in TypeCache.GetMethodsWithAttribute<DevConsoleCommand>().Where((m) => m.IsStatic))
        {
            var attr = method.GetCustomAttribute<DevConsoleCommand>();
            if (consoleCommands.ContainsKey(attr.CommnadName))
            {
                consoleCommands[attr.CommnadName].Add(CommandInvoker(method));
            }
            else
            {
                consoleCommands.Add(attr.CommnadName, new() { CommandInvoker(method) });
            }

        }
        //consoleCommands
        root = document.rootVisualElement;
        consolePanel = root.Q("ConsolePanel");
        commandField =root.Q<TextField>("CommandField");
        consoleLog = root.Q<ScrollView>("ConsoleLog");
        
        Application.logMessageReceived += Application_logMessageReceived;
        commandField.value = null;

        commandField.RegisterCallback<NavigationSubmitEvent>((evt) => DoCommand(commandField.value));
        consolePanel.visible = false;

        openConsole.performed += ToggleConsole;
        openConsole.Enable();
    }

    private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
    {
        var label = new Label();
        label.text = condition;
        switch (type)
        {
            case LogType.Error:
                label.style.color = Color.red;
                break;
            case LogType.Warning:
                label.style.color = Color.yellow;
                break;
            case LogType.Assert:
                break;
            case LogType.Log:
                label.style.color = Color.white;
                break;
            case LogType.Exception:
                label.style.color = Color.red;
                break;
        }
       
        consoleLog.Add(label);
        root.schedule.Execute(() => consoleLog.ScrollTo(label)).StartingIn(10);
        consoleLog.verticalScroller.value = consoleLog.verticalScroller.highValue+100;
        
    }
    private void ToggleConsole(InputAction.CallbackContext context)
    {
       
        consolePanel.visible = !consolePanel.visible; 
        if (consolePanel.visible)
        {
            InputManager.DisablePlayer();
            root.schedule.Execute(() => commandField.Focus()).StartingIn(10);
            Time.timeScale = 0;
            //UnityEngine.Cursor.visible = true;
            //UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            CloseConsole();
        }
       
        /*UnityEngine.Cursor.visible = true; UnityEngine.Cursor.lockState = CursorLockMode.Confined;*/ 

    }
    void CloseConsole()
    {
        Time.timeScale = 1;
        InputManager.EnablePlayer();
        //UnityEngine.Cursor.visible = false;
        //UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    void DoCommand(string command)
    {
        string[] strings = command.Split(" ");
        Debug.Log(command);
        RunCommand(strings[0], strings.Skip(1).ToArray());
        commandField.value = null;
        //CloseConsole();
    }

    void RunCommand(string command ,string[] args)
    {
        
        if (command == "Help")
        {

            foreach (var item in consoleCommands)
            {
                Debug.Log(item.Key);

            }
            return;
        }

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
            
            Expression[] expressions =new Expression[paramaters.Length];
            for (int i = 0; i < paramaters.Length; i++)
            {
                arguments[i] = Expression.Parameter(typeof(string), "arg" + i);

                if (paramaters[i].ParameterType != typeof(string))
                {
                    var parseMethod = paramaters[i].ParameterType.GetMethod("Parse", new[] { typeof(string) });
                    expressions[i] = Expression.Call(parseMethod, arguments[i]);
                }
                else
                {
                    expressions[i] = Expression.Convert(arguments[i], paramaters[i].ParameterType);
                }


                Debug.Log("arugument " + i + arguments[i]);
            }
            call = Expression.Call(method, expressions);
            return Expression.Lambda(call, arguments).Compile();
           
        } else if(paramaters.Length==0)
        {
            call = Expression.Call(method);
            var argument = Expression.Parameter(typeof(string), "args");
            return Expression.Lambda(call).Compile();
        }
        {
            var argument = Expression.Parameter(typeof(string), "args");
            //Convert.ChangeType(argument, paramaters[0].ParameterType);
            Expression expression = null;
            if (paramaters[0].ParameterType != typeof(string))
            {
                var parseMethod = paramaters[0].ParameterType.GetMethod("Parse", new[] { typeof(string) });
                expression = Expression.Call(parseMethod, argument);
            }
            else
            {
                expression = Expression.Convert(argument, paramaters[0].ParameterType);
            }
            call = Expression.Call(method, expression);
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



