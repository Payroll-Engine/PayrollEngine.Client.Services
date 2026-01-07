using PayrollEngine.Client.Model;
using PayrollEngine.Client.Scripting.Function;

namespace PayrollEngine.Client.Scripting.Runtime.Api;

/// <summary>Runtime for the case build function</summary>
public class CaseBuildRuntime : CaseChangeRuntimeBase, ICaseBuildRuntime
{
    /// <summary>Initializes a new instance of the <see cref="CaseBuildRuntime"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="scriptContext">The script context</param>
    /// <param name="tenantId">The tenant id</param>
    /// <param name="userId">The user id</param>
    /// <param name="payrollId">The payroll id</param>
    /// <param name="caseSet">The runtime case set</param>
    /// <param name="employeeId">The employee id</param>
    public CaseBuildRuntime(PayrollHttpClient httpClient, ScriptContext scriptContext, int tenantId,
        int userId, int payrollId, CaseSet caseSet, int? employeeId = null) :
        base(httpClient, scriptContext, tenantId, userId, payrollId, caseSet, employeeId)
    {
    }

    /// <summary>The log owner type</summary>
    protected override string LogOwnerType => nameof(CaseBuildFunction);
}