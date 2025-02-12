using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Function controller</summary>
/// <typeparam name="TFunc">The function</typeparam>
/// <typeparam name="TFuncAttribute">The function attribute</typeparam>
/// <typeparam name="TScriptAttribute">The script attribute</typeparam>
public abstract class FunctionControllerBase<TFunc, TFuncAttribute, TScriptAttribute>
    where TFunc : Function
    where TFuncAttribute : FunctionAttribute
    where TScriptAttribute : ScriptAttribute
{
    /// <summary>The Payroll http client</summary>
    public PayrollHttpClient HttpClient { get; }

    /// <summary>The function attribute</summary>
    protected TFuncAttribute Function { get; }

    /// <summary>The function method attributes</summary>
    protected IDictionary<TScriptAttribute, MethodInfo> Methods { get; }

    /// <summary>Initializes a new instance of the <see cref="FunctionControllerBase{TFunc,TFuncAttribute,TScriptAttribute}"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    protected FunctionControllerBase(PayrollHttpClient httpClient)
    {
        // function
        var functionType = typeof(TFunc);
        Function = functionType.GetCustomAttribute<TFuncAttribute>();
        if (Function == null)
        {
            throw new ArgumentException($"Type {functionType} without function attribute.");
        }

        // methods
        Methods = GetAttributeMethods(functionType);
        if (!Methods.Any())
        {
            throw new ArgumentException($"Type {functionType} without scripting methods.");
        }

        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>Gets the script method by script key</summary>
    /// <param name="scriptKey">The script key</param>
    /// <returns>The scripting method</returns>
    public MethodInfo GetScriptMethod(string scriptKey)
    {
        if (string.IsNullOrWhiteSpace(scriptKey))
        {
            throw new ArgumentException(nameof(scriptKey));
        }

        var method = Methods.FirstOrDefault(x => string.Equals(x.Key.ScriptKey, scriptKey));
        if (method.Key == null)
        {
            throw new ArgumentException($"Scripting method with key {scriptKey} is not available.");
        }
        return method.Value;
    }

    /// <summary>Gets the script method by script key</summary>
    /// <param name="scriptKey">The script key</param>
    /// <returns>The scripting method</returns>
    public TScriptAttribute GetScriptAttribute(string scriptKey)
    {
        if (string.IsNullOrWhiteSpace(scriptKey))
        {
            throw new ArgumentException(nameof(scriptKey));
        }

        var method = Methods.FirstOrDefault(x => string.Equals(x.Key.ScriptKey, scriptKey));
        if (method.Key == null)
        {
            throw new ArgumentException($"Scripting method with key {scriptKey} is not available.");
        }
        return method.Key;
    }

    private static Dictionary<TScriptAttribute, MethodInfo> GetAttributeMethods(Type type)
    {
        var scripts = new Dictionary<TScriptAttribute, MethodInfo>();
        foreach (var method in type.GetMethods())
        {
            foreach (var attribute in method.GetCustomAttributes<TScriptAttribute>())
            {
                scripts.Add(attribute, method);
            }
        }
        return scripts;
    }
}