using System;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Scripting.Runtime.Api;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Case build validate controller</summary>
public class CaseRelationValidateController<TFunc> : CaseRelationController<TFunc, CaseRelationValidateFunctionAttribute, CaseRelationValidateScriptAttribute>
    where TFunc : CaseRelationValidateFunction
{
    /// <summary>Initializes a new instance of the <see cref="CaseRelationValidateController{TFunc}"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="configuration">The scripting configuration</param>
    public CaseRelationValidateController(PayrollHttpClient httpClient, ScriptingConfiguration configuration) :
        base(httpClient, configuration)
    {
    }

    /// <summary>Execute the case relation build function</summary>
    /// <param name="sourceCaseName">The source case name</param>
    /// <param name="targetCaseName">The target case name</param>
    /// <returns>True if the specified case is valid</returns>
    public CaseRelationValidateFunctionResult Validate(string sourceCaseName, string targetCaseName)
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
            throw new PayrollException($"Missing validate source case {sourceCaseName}");
        }

        // target
        var targetCaseSet = GetCaseSet(targetCaseName).Result;
        if (targetCaseSet == null)
        {
            throw new PayrollException($"Missing validate target case {targetCaseName}");
        }

        // validate
        return Validate(sourceCaseSet, targetCaseSet);
    }

    /// <summary>RelationMap a case</summary>
    /// <param name="sourceCaseSet">The source case set</param>
    /// <param name="targetCaseSet">The target case set</param>
    /// <returns>True if the specified case is valid</returns>
    public CaseRelationValidateFunctionResult Validate(CaseSet sourceCaseSet, CaseSet targetCaseSet)
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
        var calendar = NewScriptingCalendar();

        // runtime and function
        var runtime = new CaseRelationValidateRuntime(HttpClient, calendar, Tenant.Id, User.Id, Payroll.Id,
            sourceCaseSet, targetCaseSet, Employee?.Id);
        var function = Activator.CreateInstance(typeof(TFunc), runtime);
        var validate = method.Invoke(function, null) as bool?;

        // result
        var result = new CaseRelationValidateFunctionResult
        {
            Valid = validate,
            Calendar = calendar,
            Tenant = Tenant,
            Employee = Employee,
            Payroll = Payroll,
            SourceCaseSet = sourceCaseSet,
            TargetCaseSet = targetCaseSet,
            Issues = runtime.Issues
        };
        return result;
    }
}