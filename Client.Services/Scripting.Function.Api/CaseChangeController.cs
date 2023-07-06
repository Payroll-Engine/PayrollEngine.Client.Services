namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Case change function controller</summary>
public abstract class CaseChangeController<TFunc, TFuncAttribute, TScriptAttribute> : CaseController<TFunc, TFuncAttribute, TScriptAttribute>
    where TFunc : CaseChangeFunction
    where TFuncAttribute : CaseChangeFunctionAttribute
    where TScriptAttribute : CaseChangeScriptAttribute
{
    /// <summary>Initializes a new instance of the <see cref="CaseChangeController{TFunc, TFuncAttribute, TScriptAttribute}"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="configuration">The scripting configuration</param>
    protected CaseChangeController(PayrollHttpClient httpClient, ScriptConfiguration configuration) :
        base(httpClient, configuration)
    {
    }
}