using System;
using PayrollEngine.Client.Model;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Case relation validate function invoker</summary>
public class CaseRelationValidateFunctionInvoker<TFunction> : FunctionInvokerBase
    where TFunction : CaseRelationValidateFunction
{
    /// <summary>Invoker constructor</summary>
    /// <param name="httpClient">The Payroll http configuration</param>
    /// <param name="configuration">The scripting configuration</param>
    public CaseRelationValidateFunctionInvoker(PayrollHttpClient httpClient, ScriptConfiguration configuration) :
        base(httpClient, configuration)
    {
    }

    /// <summary>Validate a case relation</summary>
    /// <param name="sourceCaseName">The source case name</param>
    /// <param name="targetCaseName">The target case name</param>
    /// <returns>The function result</returns>
    public CaseRelationValidateFunctionResult Validate(string sourceCaseName, string targetCaseName) =>
        Validate(sourceCaseName, targetCaseName, Configuration);

    /// <summary>Validate a case relation</summary>
    /// <param name="sourceCaseSet">The source case set</param>
    /// <param name="targetCaseSet">The target case set</param>
    /// <returns>The function result</returns>
    public CaseRelationValidateFunctionResult Validate(CaseSet sourceCaseSet, CaseSet targetCaseSet) =>
        Validate(sourceCaseSet, targetCaseSet, Configuration);

    /// <summary>Validate a case relation</summary>
    /// <param name="sourceCaseName">The source case name</param>
    /// <param name="targetCaseName">The target case name</param>
    /// <param name="configuration">The scripting configuration</param>
    /// <returns>The function result</returns>
    public CaseRelationValidateFunctionResult Validate(string sourceCaseName, string targetCaseName,
        ScriptConfiguration configuration)
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
        var controller = new CaseRelationValidateController<TFunction>(HttpClient, configuration);
        return controller.Validate(sourceCaseName, targetCaseName);
    }

    /// <summary>Validate a case relation</summary>
    /// <param name="sourceCaseSet">The source case set</param>
    /// <param name="targetCaseSet">The target case set</param>
    /// <param name="configuration">The scripting configuration</param>
    /// <returns>The function result</returns>
    public CaseRelationValidateFunctionResult Validate(CaseSet sourceCaseSet, CaseSet targetCaseSet,
        ScriptConfiguration configuration)
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
        var controller = new CaseRelationValidateController<TFunction>(HttpClient, configuration);
        return controller.Validate(sourceCaseSet, targetCaseSet);
    }
}