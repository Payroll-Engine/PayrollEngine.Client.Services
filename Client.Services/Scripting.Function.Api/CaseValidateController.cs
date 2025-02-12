using System;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Scripting.Runtime.Api;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Case validate function controller</summary>
public class CaseValidateController<TFunc> : CaseChangeController<TFunc, CaseValidateFunctionAttribute, CaseValidateScriptAttribute>
    where TFunc : CaseValidateFunction
{
    /// <summary>Initializes a new instance of the <see cref="CaseValidateController{T}"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="configuration">The scripting configuration</param>
    public CaseValidateController(PayrollHttpClient httpClient, ScriptConfiguration configuration) :
        base(httpClient, configuration)
    {
    }

    /// <summary>Determines whether the existing case is available</summary>
    /// <param name="caseName">Name of the case</param>
    /// <returns>True if the specified case is valid</returns>
    public CaseValidateFunctionResult Validate(string caseName)
    {
        if (string.IsNullOrWhiteSpace(caseName))
        {
            throw new ArgumentException(nameof(caseName));
        }
        var caseSet = GetCaseSet(caseName).Result;
        if (caseSet == null)
        {
            throw new PayrollException($"Missing payroll case {caseName}.");
        }
        return Validate(caseName, caseSet);
    }

    /// <summary>Determines whether the specified case is available</summary>
    /// <param name="caseName">Name of the case</param>
    /// <param name="caseSet">The case to validate</param>
    /// <returns>True if the specified case is valid</returns>
    public CaseValidateFunctionResult Validate(string caseName, CaseSet caseSet)
    {
        if (string.IsNullOrWhiteSpace(caseName))
        {
            throw new ArgumentException(nameof(caseName));
        }
        if (caseSet == null)
        {
            throw new ArgumentNullException(nameof(caseSet));
        }

        var method = GetScriptMethod(caseName);
        var calendar = NewScriptingCalendar();

        // runtime and function
        var runtime = new CaseValidateRuntime(HttpClient, calendar, Tenant.Id, User.Id, Payroll.Id, caseSet, Employee?.Id);
        var function = Activator.CreateInstance(typeof(TFunc), runtime);
        var valid = method.Invoke(function, null) as bool?;

        // result
        var result = new CaseValidateFunctionResult
        {
            Valid = valid,
            Calendar = calendar,
            Tenant = Tenant,
            Employee = Employee,
            Payroll = Payroll,
            CaseSet = caseSet,
            Issues = runtime.Issues
        };
        return result;
    }
}