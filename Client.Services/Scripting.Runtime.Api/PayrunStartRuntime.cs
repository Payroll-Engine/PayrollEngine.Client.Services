using PayrollEngine.Client.Scripting.Function;

namespace PayrollEngine.Client.Scripting.Runtime.Api;

/// <summary>Runtime for the payrun employee available function</summary>
public class PayrunStartRuntime : PayrunRuntimeBase, IPayrunStartRuntime
{
    /// <summary>Initializes a new instance of the <see cref="PayrunStartRuntime"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="scriptContext">The script context</param>
    /// <param name="tenantId">The tenant id</param>
    /// <param name="userId">The user id</param>
    /// <param name="payrollId">The payroll id</param>
    /// <param name="employeeId">The employee id</param>
    public PayrunStartRuntime(PayrollHttpClient httpClient, ScriptContext scriptContext,
        int tenantId, int userId, int payrollId, int? employeeId = null) :
        base(httpClient, scriptContext, tenantId, userId, payrollId, employeeId)
    {
    }

    /// <summary>The log owner type</summary>
    protected override string LogOwnerType => nameof(PayrunStartFunction);
}