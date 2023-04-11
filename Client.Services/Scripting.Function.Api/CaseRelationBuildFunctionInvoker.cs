using System;
using PayrollEngine.Client.Model;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Case relation build function invoker</summary>
public class CaseRelationBuildFunctionInvoker<TFunction> : FunctionInvokerBase
    where TFunction : CaseRelationBuildFunction
{
    /// <summary>Invoker constructor</summary>
    /// <param name="httpClient">The Payroll http configuration</param>
    /// <param name="configuration">The scripting configuration</param>
    public CaseRelationBuildFunctionInvoker(PayrollHttpClient httpClient, ScriptingConfiguration configuration) :
        base(httpClient, configuration)
    {
    }

    /// <summary>Build a case relation</summary>
    /// <param name="sourceCaseName">The source case name</param>
    /// <param name="targetCaseName">The target case name</param>
    /// <returns>The function result</returns>
    public CaseRelationBuildFunctionResult Build(string sourceCaseName, string targetCaseName) =>
        Build(sourceCaseName, targetCaseName, Configuration);

    /// <summary>Build a case relation</summary>
    /// <param name="sourceCaseSet">The source case set</param>
    /// <param name="targetCaseSet">The target case set</param>
    /// <returns>The function result</returns>
    public CaseRelationBuildFunctionResult Build(CaseSet sourceCaseSet, CaseSet targetCaseSet) =>
        Build(sourceCaseSet, targetCaseSet, Configuration);

    /// <summary>Build a case relation</summary>
    /// <param name="sourceCaseName">The source case name</param>
    /// <param name="targetCaseName">The target case name</param>
    /// <param name="configuration">The scripting configuration</param>
    /// <returns>The function result</returns>
    public CaseRelationBuildFunctionResult Build(string sourceCaseName, string targetCaseName,
        ScriptingConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(sourceCaseName))
        {
            throw new ArgumentException(nameof(sourceCaseName));
        }
        if (string.IsNullOrWhiteSpace(targetCaseName))
        {
            throw new ArgumentException(nameof(targetCaseName));
        }
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }
        var controller = new CaseRelationBuildController<TFunction>(HttpClient, configuration);
        return controller.Build(sourceCaseName, targetCaseName);
    }

    /// <summary>Build a case relation</summary>
    /// <param name="sourceCaseSet">The source case set</param>
    /// <param name="targetCaseSet">The target case set</param>
    /// <param name="configuration">The scripting configuration</param>
    /// <returns>The function result</returns>
    public CaseRelationBuildFunctionResult Build(CaseSet sourceCaseSet, CaseSet targetCaseSet,
        ScriptingConfiguration configuration)
    {
        if (sourceCaseSet == null)
        {
            throw new ArgumentNullException(nameof(sourceCaseSet));
        }
        if (targetCaseSet == null)
        {
            throw new ArgumentNullException(nameof(targetCaseSet));
        }

        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }
        var controller = new CaseRelationBuildController<TFunction>(HttpClient, configuration);
        return controller.Build(sourceCaseSet, targetCaseSet);
    }
}