using System;
using System.Threading.Tasks;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service.Api;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Case function controller</summary>
public abstract class CaseController<TFunc, TFuncAttribute, TScriptAttribute> :
    PayrollFunctionController<TFunc, TFuncAttribute, TScriptAttribute>
    where TFunc : CaseFunction
    where TFuncAttribute : CaseFunctionAttribute
    where TScriptAttribute : CaseScriptAttribute
{
    /// <summary>Initializes a new instance of the <see cref="CaseController{TFunc, TFuncAttribute, TScriptAttribute}"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="configuration">The scripting configuration</param>
    protected CaseController(PayrollHttpClient httpClient, ScriptingConfiguration configuration) :
        base(httpClient, configuration)
    {
    }

    /// <summary>Load the payroll case</summary>
    /// <param name="caseName">Name of the case</param>
    /// <returns>The case</returns>
    public async Task<Case> GetCase(string caseName) =>
        await GetCaseSet<CaseSet>(caseName);

    /// <summary>Load the payroll case</summary>
    /// <param name="caseName">Name of the case</param>
    /// <returns>The case</returns>
    public async Task<CaseSet> GetCaseSet(string caseName) =>
        await GetCaseSet<CaseSet>(caseName);

    /// <summary>Load the payroll case</summary>
    /// <param name="caseName">Name of the case</param>
    /// <returns>The case</returns>
    public async Task<T> GetCaseSet<T>(string caseName) where T : CaseSet
    {
        if (string.IsNullOrWhiteSpace(caseName))
        {
            throw new ArgumentException(nameof(caseName));
        }
        return await new PayrollService(HttpClient).BuildCaseAsync<T>(new(Tenant.Id, Payroll.Id),
            caseName, User.Id, Employee.Id);
    }
}