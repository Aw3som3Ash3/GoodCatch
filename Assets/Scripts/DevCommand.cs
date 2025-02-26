using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public abstract class DevCommand : ScriptableObject,IDevCommand
{
    [SerializeField]
    string methodName;
    [SerializeField]
    string commandName;
    public string CommandName => commandName;
    public string MethodName => methodName;
    //protected Action<T, object> command;

    private void OnEnable()
    {


    }
    protected Action<T, object> CommandInvoker<T>()
    {
        var instance = Expression.Parameter(typeof(T), "instance");
        var argument = Expression.Parameter(typeof(object), "args");
        var method = typeof(T).GetMethod(MethodName)
            ?? throw new Exception($"method {MethodName}, not found on type{typeof(T).Name}");
        var call = Expression.Call(instance, method, Expression.Convert(argument, method.GetParameters()[0].ParameterType));
        return Expression.Lambda<Action<T, object>>(call, instance, argument).Compile();



    }
    public abstract void RunCommand(string[] args);
}
