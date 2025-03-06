using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
//using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
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
    Dictionary<string, Command> consoleCommands=new(StringComparer.OrdinalIgnoreCase);



    LinkedList<string> previousCommands=new();
    LinkedListNode<string> selectedCommand;
    
    //HashSet<Commands> consoleCommands = new HashSet<Commands>();

    class Command
    {
        public string commandName;
        public string description;
        public readonly List<(Delegate @delegate, (string name, Type type)[] paramaters)> commandActions;
        public Command(string name,string description, (Delegate action, (string name, Type type)[] paramaters) command )
        {
            commandName = name;
            commandActions = new(){ command };
            this.description = description;
        }
        public void AddCommand((Delegate action, (string name, Type type)[] paramaters) command)
        {
            commandActions.Add(command);
        }
        public void UpdateDescription(string newDescription)
        {
            description = newDescription;
            //Debug.Log("changed description to: "+description);
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

        foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IUseDevCommands))))
        {
            foreach (var method in type.GetMethods().Where((x) => x.IsStatic&&x.GetCustomAttribute<DevConsoleCommand>()!=null))
            {
                var attr = method.GetCustomAttribute<DevConsoleCommand>();

                var _params = method.GetParameters().Select((x) => ($"[{x.ParameterType.Name}]{x.Name}", x.ParameterType)).ToArray();


                var command = (CommandInvoker(method), _params);
                if (consoleCommands.ContainsKey(attr.CommandName.ToLower()))
                {
                    consoleCommands[attr.CommandName.ToLower()].AddCommand(command);
                }
                else
                {
                    //consoleCommands.Add(attr.CommandName,(new() { command },attr.Description) );
                    consoleCommands.Add(attr.CommandName.ToLower(), new Command(attr.CommandName, attr.Description, command));
                }


                if (string.IsNullOrEmpty(consoleCommands[attr.CommandName].description) && !string.IsNullOrEmpty(attr.Description))
                {
                    //Debug.Log("has updated description "+ attr.Description);
                    consoleCommands[attr.CommandName].UpdateDescription(attr.Description);
                    //Debug.Log("new description " + consoleCommands[attr.CommandName].description);
                }
            }
            

        }
        //consoleCommands
        root = document.rootVisualElement;
        consolePanel = root.Q("ConsolePanel");
        commandField =root.Q<TextField>("CommandField");
        consoleLog = root.Q<ScrollView>("ConsoleLog");
        
        Application.logMessageReceived += Application_logMessageReceived;
        commandField.value = null;
        commandField.RegisterCallback<KeyDownEvent>((evt) => { if (evt.character=='\n') { evt.PreventDefault(); } }  );
        commandField.RegisterCallback<NavigationSubmitEvent>((evt) =>
        {
            evt.PreventDefault();
            evt.StopImmediatePropagation();
            DoCommand(commandField.value);

        });
        commandField.RegisterCallback<NavigationMoveEvent>(OnNaviagate);
        consolePanel.visible = false;

        openConsole.performed += ToggleConsole;
        openConsole.Enable();
    }

    private void OnNaviagate(NavigationMoveEvent evt)
    {
        if (selectedCommand == null&& previousCommands.First!=null)
        {
            selectedCommand = previousCommands.First;
            commandField.value = selectedCommand.Value;
        }
        else if(selectedCommand != null)
        {
            switch (evt.direction)
            {
                case NavigationMoveEvent.Direction.None:
                    break;
                case NavigationMoveEvent.Direction.Left:
                    break;
                case NavigationMoveEvent.Direction.Up:
                    if (selectedCommand.Next != null)
                    {
                        selectedCommand = selectedCommand.Next;
                    }
                    commandField.value = selectedCommand.Value;
                    break;
                case NavigationMoveEvent.Direction.Right:
                    break;
                case NavigationMoveEvent.Direction.Down:
                    if (selectedCommand.Previous != null)
                    {
                        selectedCommand = selectedCommand.Previous;
                        commandField.value = selectedCommand.Value;
                    }
                    commandField.value ="";
                    break;
                case NavigationMoveEvent.Direction.Next:
                    break;
                case NavigationMoveEvent.Direction.Previous:
                    break;
            }
           
        }

        evt.StopImmediatePropagation();
        evt.PreventDefault();
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
        RunCommand(strings[0].ToLower(), strings.Skip(1).ToArray());
        commandField.SetValueWithoutNotify("");
       // commandField.Focus();
        previousCommands.AddFirst(command);
        selectedCommand = null;
        //CloseConsole();
    }

    void RunCommand(string command ,string[] args)
    {
        #region Commands Help
        if (command.ToLower() == "help")
        {
            print("------------------------------------------------------------------------------\n" +
                "Commands:\n\n");


            foreach (var item in consoleCommands.OrderBy((command)=>command.Key))
            {
                print($"    -{item.Key}: {item.Value.description}");

            }
            return;
        }

        if (args.Length>0 &&  args[0].ToLower() == "help" && consoleCommands.ContainsKey(command))
        {
            print("------------------------------------------------------------------------------\n" +
                $"{command}: {consoleCommands[command].description}:\n");
            foreach (var item in consoleCommands[command].commandActions.OrderBy((x)=>x.paramaters.Length))
            {
                string paramatersString = " ";
                //Debug.Log(item.@delegate.GetMethodInfo().Name);
                var paramaters = item.paramaters;
                for (int i = 0; i < paramaters.Length; i++)
                {
                    paramatersString +=paramaters[i].name+" "; 


                }

               print("  -"+command + paramatersString);
            }
            return;
        }
        #endregion


        if (consoleCommands.ContainsKey(command.ToLower()))
        {
            consoleCommands[command].commandActions.Find((command) =>
                {
                    if (command.paramaters.Length == args.Length)
                    {
                        for (int i = 0; i < command.paramaters.Length; i++)
                        {
                            
                            switch (command.paramaters[i].type)
                            {
                                case Type intType when intType == typeof(int):
                                    if (!int.TryParse(args[i],out int resultInt))
                                    {
                                        return false;
                                    }
                                    break;
                                case Type floatType when floatType == typeof(float):
                                    if (!float.TryParse(args[i], out float resultFloat))
                                    {
                                        return false;
                                    }
                                    break;

                                case Type boolType when boolType == typeof(bool):
                                    if (!bool.TryParse(args[i], out bool resultBool))
                                    {
                                        return false;
                                    }
                                    break;
                                case Type stringType when stringType == typeof(string):
                                    continue;
                                default: 
                                    return false;

                            }
                            //args[i]  command.paramaters[i].type.
                        }
                        return true;
                    }
                    return false;
                }).@delegate?.DynamicInvoke(args);
        }
        else
        {
            print($"no command of type{command} found");
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
            List<ParameterExpression> arguments = new();
            
            Expression[] expressions =new Expression[paramaters.Length];
            for (int i = 0; i < paramaters.Length; i++)
            {

                var argument = Expression.Parameter(typeof(string), "arg" + i);
                arguments.Add(argument);
                if (paramaters[i].ParameterType != typeof(string))
                {
                    var parseMethod = paramaters[i].ParameterType.GetMethod("Parse", new[] { typeof(string) });
                    
                    expressions[i] = Expression.Call(parseMethod, argument);
                    
                }
                else
                {
                    expressions[i] = Expression.Convert(argument, paramaters[i].ParameterType);
                }


                Debug.Log("argument " + i +": "+ argument.Name);
            }
            
            call = Expression.Call(method, expressions);
            return Expression.Lambda(call, arguments).Compile();
           
        } else if(paramaters.Length==0)
        {
            call = Expression.Call(method);
            var argument = Expression.Parameter(typeof(string), "args");
            return Expression.Lambda(call).Compile();
        }else
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
                //expression = Expression.Convert(argument, paramaters[0].ParameterType);
                expression = argument;
                
               
            }
            call = Expression.Call(method, expression);
            return Expression.Lambda(call, argument).Compile();

        }

       
       
       



    }

}
[System.AttributeUsage(System.AttributeTargets.Method)] // Multiuse attribute.

public class DevConsoleCommand: System.Attribute
{
    string commandName;
    public string CommandName => commandName;
    string description=null;
    public string Description=>description;
    public DevConsoleCommand(string commandName)
    {
        this.commandName = commandName;
        
    }
    public DevConsoleCommand(string commandName,string description)
    {
        this.commandName = commandName;
        this.description = description;
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




