using System;
using PayrollEngine.Client.Model;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Case build function invoker</summary>
public class CaseBuildFunctionInvoker<TFunction> : FunctionInvokerBase
    where TFunction : CaseBuildFunction
{
    /// <summary>Invoker constructor</summary>
    /// <param name="httpClient">The Payroll http configuration</param>
    /// <param name="configuration">The scripting configuration</param>
    public CaseBuildFunctionInvoker(PayrollHttpClient httpClient, ScriptConfiguration configuration) :
        base(httpClient, configuration)
    {
    }

    /// <summary>Build a case</summary>
    /// <param name="caseName">Name of the case</param>
    /// <returns>The function result</returns>
    public CaseBuildFunctionResult Build(string caseName) =>
        Build(caseName, Configuration);

    /// <summary>Build a case</summary>
    /// <param name="caseSet">The case set</param>
    /// <returns>The function result</returns>
    public CaseBuildFunctionResult Build(CaseSet caseSet) =>
        Build(caseSet, Configuration);

    /// <summary>Build a case</summary>
    /// <param name="caseName">Name of the case</param>
    /// <param name="configuration">The scripting configuration</param>
    /// <returns>The function result</returns>
    public CaseBuildFunctionResult Build(string caseName, ScriptConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(caseName))
        {
            throw new ArgumentException(nameof(caseName));
        }
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }
        var controller = new CaseBuildController<TFunction>(HttpClient, configuration);
        return controller.Build(caseName);
    }

    /// <summary>Build a case</summary>
    /// <param name="caseSet">The case set</param>
    /// <param name="configuration">The scripting configuration</param>
    /// <returns>The function result</returns>
    public CaseBuildFunctionResult Build(CaseSet caseSet, ScriptConfiguration configuration)
    {
        if (caseSet == null)
        {
            throw new ArgumentNullException(nameof(caseSet));
        }
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }
        var controller = new CaseBuildController<TFunction>(HttpClient, configuration);
        return controller.Build(caseSet);
    }
}