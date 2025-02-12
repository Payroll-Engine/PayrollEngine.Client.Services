using System;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Scripting.Runtime.Api;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Case available function controller</summary>
public class CaseAvailableController<TFunc> : CaseController<TFunc, CaseAvailableFunctionAttribute, CaseAvailableScriptAttribute>
    where TFunc : CaseAvailableFunction
{
    /// <summary>Initializes a new instance of the <see cref="CaseAvailableController{T}"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="configuration">The scripting configuration</param>
    public CaseAvailableController(PayrollHttpClient httpClient, ScriptConfiguration configuration) :
        base(httpClient, configuration)
    {
    }

    /// <summary>Determines whether the existing case is available</summary>
    /// <param name="caseName">Name of the case</param>
    /// <returns>True if the specified case is available</returns>
    public CaseAvailableFunctionResult IsAvailable(string caseName)
    {
        if (string.IsNullOrWhiteSpace(caseName))
        {
            throw new ArgumentException(nameof(caseName));
        }
        var @case = GetCase(caseName).Result;
        if (@case == null)
        {
            throw new PayrollException($"Missing payroll case {caseName}.");
        }
        return IsAvailable(caseName, @case);
    }

    /// <summary>Determines whether the specified case is available</summary>
    /// <param name="caseName">Name of the case</param>
    /// <param name="case">The case to validate</param>
    /// <returns>True if the specified case is available</returns>
    public CaseAvailableFunctionResult IsAvailable(string caseName, Case @case)
    {
        if (string.IsNullOrWhiteSpace(caseName))
        {
            throw new ArgumentException(nameof(caseName));
        }
        if (@case == null)
        {
            throw new ArgumentNullException(nameof(@case));
        }

        var method = GetScriptMethod(caseName);
        var calendar = NewScriptingCalendar();

        // runtime and function
        var runtime = new CaseAvailableRuntime(HttpClient, calendar, Tenant.Id, User.Id, Payroll.Id, @case, Employee?.Id);
        var function = Activator.CreateInstance(typeof(TFunc), runtime);
        var available = method.Invoke(function, null) as bool?;

        // result
        var result = new CaseAvailableFunctionResult
        {
            Available = available,
            Calendar = calendar,
            Tenant = Tenant,
            Employee = Employee,
            Payroll = Payroll,
            Case = @case
        };
        return result;
    }
}