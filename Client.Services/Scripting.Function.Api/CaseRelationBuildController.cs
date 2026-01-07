using System;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Scripting.Runtime.Api;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Case relation build function controller</summary>
public class CaseRelationBuildController<TFunc> : CaseRelationController<TFunc, CaseRelationBuildFunctionAttribute, CaseRelationBuildScriptAttribute>
    where TFunc : CaseRelationBuildFunction
{
    /// <summary>Initializes a new instance of the <see cref="CaseRelationBuildController{TFunc}"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="configuration">The scripting configuration</param>
    public CaseRelationBuildController(PayrollHttpClient httpClient, ScriptConfiguration configuration) :
        base(httpClient, configuration)
    {
    }

    /// <summary>Execute the case relation build function</summary>
    /// <param name="sourceCaseName">The source case name</param>
    /// <param name="targetCaseName">The target case name</param>
    /// <returns>True if the specified case is valid</returns>
    public CaseRelationBuildFunctionResult Build(string sourceCaseName, string targetCaseName)
    {
        if (string.IsNullOrWhiteSpace(sourceCaseName))
        {
            throw new ArgumentException(nameof(sourceCaseName));
        }
        if (string.IsNullOrWhiteSpace(targetCaseName))
        {
            throw new ArgumentException(nameof(targetCaseName));
        }

        // source
        var sourceCaseSet = GetCaseSet(sourceCaseName).Result;
        if (sourceCaseSet == null)
        {
            throw new PayrollException($"Missing build source case {sourceCaseName}.");
        }

        // target
        var targetCaseSet = GetCaseSet(targetCaseName).Result;
        if (targetCaseSet == null)
        {
            throw new PayrollException($"Missing build target case {targetCaseName}.");
        }

        // build
        return Build(sourceCaseSet, targetCaseSet);
    }

    /// <summary>RelationMap a case</summary>
    /// <param name="sourceCaseSet">The source case set</param>
    /// <param name="targetCaseSet">The target case set</param>
    /// <returns>True if the specified case is valid</returns>
    public CaseRelationBuildFunctionResult Build(CaseSet sourceCaseSet, CaseSet targetCaseSet)
    {
        if (sourceCaseSet == null)
        {
            throw new ArgumentNullException(nameof(sourceCaseSet));
        }
        if (targetCaseSet == null)
        {
            throw new ArgumentNullException(nameof(targetCaseSet));
        }

        var scriptKey = sourceCaseSet.Name.ToCaseRelationKey(targetCaseSet.Name);
        var method = GetScriptMethod(scriptKey);
        var context = NewScriptingContext();

        // runtime and function
        var runtime = new CaseRelationBuildRuntime(HttpClient, context, Tenant.Id, User.Id, Payroll.Id,
            sourceCaseSet, targetCaseSet, Employee?.Id);
        var function = Activator.CreateInstance(typeof(TFunc), runtime);
        var build = method.Invoke(function, null) as bool?;

        // result
        var result = new CaseRelationBuildFunctionResult
        {
            Build = build,
            Calendar = context.Calendar,
            Tenant = Tenant,
            Employee = Employee,
            Payroll = Payroll,
            SourceCaseSet = sourceCaseSet,
            TargetCaseSet = targetCaseSet
        };
        return result;
    }
}