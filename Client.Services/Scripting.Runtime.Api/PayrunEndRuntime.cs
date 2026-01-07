using System.Collections.Generic;
using PayrollEngine.Client.Scripting.Function;

namespace PayrollEngine.Client.Scripting.Runtime.Api;

/// <summary>Runtime for the payrun employee available function</summary>
public class PayrunEndRuntime : PayrunRuntimeBase, IPayrunEndRuntime
{
    /// <summary>Initializes a new instance of the <see cref="PayrunEndRuntime"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="scriptContext">The script context</param>
    /// <param name="tenantId">The tenant id</param>
    /// <param name="userId">The user id</param>
    /// <param name="payrollId">The payroll id</param>
    /// <param name="employeeId">The employee id</param>
    public PayrunEndRuntime(PayrollHttpClient httpClient, ScriptContext scriptContext,
        int tenantId, int userId, int payrollId, int? employeeId = null) :
        base(httpClient, scriptContext, tenantId, userId, payrollId, employeeId)
    {
    }
    /// <summary>The log owner type</summary>
    protected override string LogOwnerType => nameof(PayrunEndFunction);

    #region Runtime Values

    /// <inheritdoc />
    public Dictionary<string, string> GetPayrunRuntimeValues()
    {
        // implementation
        return null;
    }

    /// <inheritdoc />
    public List<string> GetRuntimeValuesEmployees()
    {
        // implementation
        return null;
    }

    /// <inheritdoc />
    public Dictionary<string, string> GetEmployeeRuntimeValues(string employeeIdentifier)
    {
        // implementation
        return null;
    }

    #endregion

}