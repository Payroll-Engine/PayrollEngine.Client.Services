using System;
using PayrollEngine.Client.Model;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Case available function invoker</summary>
public class CaseAvailableFunctionInvoker<TFunction> : FunctionInvokerBase
    where TFunction : CaseAvailableFunction
{
    /// <summary>Invoker constructor</summary>
    /// <param name="httpClient">The Payroll http configuration</param>
    /// <param name="configuration">The scripting configuration</param>
    public CaseAvailableFunctionInvoker(PayrollHttpClient httpClient, ScriptingConfiguration configuration) :
        base(httpClient, configuration)
    {
    }

    /// <summary>Determines whether the existing case is available</summary>
    /// <param name="caseName">Name of the case</param>
    /// <returns>The function result</returns>
    public CaseAvailableFunctionResult Available(string caseName) =>
        Available(caseName, Configuration);

    /// <summary>Determines whether the existing case is available</summary>
    /// <param name="caseName">Name of the case</param>
    /// <param name="case">The case to validate</param>
    /// <returns>The function result</returns>
    public CaseAvailableFunctionResult Available(string caseName, Case @case) =>
        Available(caseName, @case, Configuration);

    /// <summary>Determines whether the existing case is available</summary>
    /// <param name="caseName">Name of the case</param>
    /// <param name="configuration">The scripting configuration</param>
    /// <returns>The function result</returns>
    public CaseAvailableFunctionResult Available(string caseName, ScriptingConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(caseName))
        {
            throw new ArgumentException(nameof(caseName));
        }
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }
        var controller = new CaseAvailableController<TFunction>(HttpClient, configuration);
        return controller.IsAvailable(caseName);
    }

    /// <summary>Determines whether the existing case is available</summary>
    /// <param name="caseName">Name of the case</param>
    /// <param name="case">The case to validate</param>
    /// <param name="configuration">The scripting configuration</param>
    /// <returns>The function result</returns>
    public CaseAvailableFunctionResult Available(string caseName, Case @case, ScriptingConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(caseName))
        {
            throw new ArgumentException(nameof(caseName));
        }
        if (@case == null)
        {
            throw new ArgumentNullException(nameof(@case));
        }
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }
        var controller = new CaseAvailableController<TFunction>(HttpClient, configuration);
        return controller.IsAvailable(caseName, @case);
    }
}