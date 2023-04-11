using System;
using PayrollEngine.Client.Model;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Case validate function invoker</summary>
public class CaseValidateFunctionInvoker<TFunction> : FunctionInvokerBase
    where TFunction : CaseValidateFunction
{
    /// <summary>Invoker constructor</summary>
    /// <param name="httpClient">The Payroll http configuration</param>
    /// <param name="configuration">The scripting configuration</param>
    public CaseValidateFunctionInvoker(PayrollHttpClient httpClient, ScriptingConfiguration configuration) :
        base(httpClient, configuration)
    {
    }

    /// <summary>Determines whether the existing case is valid</summary>
    /// <param name="caseName">Name of the case</param>
    /// <returns>The function result</returns>
    public CaseValidateFunctionResult Validate(string caseName) =>
        Validate(caseName, Configuration);

    /// <summary>Determines whether the existing case is valid</summary>
    /// <param name="caseName">Name of the case</param>
    /// <param name="caseSet">The case set</param>
    /// <returns>The function result</returns>
    public CaseValidateFunctionResult Validate(string caseName, CaseSet caseSet) =>
        Validate(caseName, caseSet, Configuration);

    /// <summary>Determines whether the existing case is valid</summary>
    /// <param name="caseName">Name of the case</param>
    /// <param name="configuration">The scripting configuration</param>
    /// <returns>The function result</returns>
    public CaseValidateFunctionResult Validate(string caseName, ScriptingConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(caseName))
        {
            throw new ArgumentException(nameof(caseName));
        }
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }
        var controller = new CaseValidateController<TFunction>(HttpClient, configuration);
        return controller.Validate(caseName);
    }

    /// <summary>Determines whether the existing case is valid</summary>
    /// <param name="caseName">Name of the case</param>
    /// <param name="caseSet">The case set</param>
    /// <param name="configuration">The scripting configuration</param>
    /// <returns>The function result</returns>
    public CaseValidateFunctionResult Validate(string caseName, CaseSet caseSet, ScriptingConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(caseName))
        {
            throw new ArgumentException(nameof(caseName));
        }
        if (caseSet == null)
        {
            throw new ArgumentNullException(nameof(caseSet));
        }
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }
        var controller = new CaseValidateController<TFunction>(HttpClient, configuration);
        return controller.Validate(caseName, caseSet);
    }
}