using System;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Scripting.Runtime.Api;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Case build function controller</summary>
public class CaseBuildController<TFunc> : CaseChangeController<TFunc, CaseBuildFunctionAttribute, CaseBuildScriptAttribute>
    where TFunc : CaseBuildFunction
{
    /// <summary>Initializes a new instance of the <see cref="CaseBuildController{T}"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="configuration">The scripting configuration</param>
    public CaseBuildController(PayrollHttpClient httpClient, ScriptConfiguration configuration) :
        base(httpClient, configuration)
    {
    }

    /// <summary>Build a case</summary>
    /// <param name="caseName">Name of the case</param>
    /// <returns>True if the specified case is valid</returns>
    public CaseBuildFunctionResult Build(string caseName)
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
        return Build(caseSet);
    }

    /// <summary>Build a case</summary>
    /// <param name="caseSet">The case to build</param>
    /// <returns>True if the specified case is valid</returns>
    public CaseBuildFunctionResult Build(CaseSet caseSet)
    {
        if (caseSet == null)
        {
            throw new ArgumentNullException(nameof(caseSet));
        }
        if (string.IsNullOrWhiteSpace(caseSet.Name))
        {
            throw new ArgumentException("Case set without name.", nameof(caseSet));
        }

        var method = GetScriptMethod(caseSet.Name);
        var context = NewScriptingContext();

        // runtime and function
        var runtime = new CaseBuildRuntime(HttpClient, context, Tenant.Id, User.Id, Payroll.Id, caseSet, Employee?.Id);
        var function = Activator.CreateInstance(typeof(TFunc), runtime);
        var build = method.Invoke(function, null) as bool?;

        // result
        var result = new CaseBuildFunctionResult
        {
            Build = build,
            Calendar = context.Calendar,
            Tenant = Tenant,
            Employee = Employee,
            Payroll = Payroll,
            CaseSet = caseSet
        };
        return result;
    }
}