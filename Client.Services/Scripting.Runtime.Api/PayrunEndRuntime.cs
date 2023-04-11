using System.Collections.Generic;
using PayrollEngine.Client.Scripting.Function;

namespace PayrollEngine.Client.Scripting.Runtime.Api;

/// <summary>Runtime for the payrun employee available function</summary>
public class PayrunEndRuntime : PayrunRuntime, IPayrunEndRuntime
{
    /// <summary>Initializes a new instance of the <see cref="PayrunEndRuntime"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="calendar">The calendar</param>
    /// <param name="tenantId">The tenant id</param>
    /// <param name="userId">The user id</param>
    /// <param name="payrollId">The payroll id</param>
    /// <param name="employeeId">The employee id</param>
    public PayrunEndRuntime(PayrollHttpClient httpClient, ScriptingCalendar calendar,
        int tenantId, int userId, int payrollId, int? employeeId = null) :
        base(httpClient, calendar, tenantId, userId, payrollId, employeeId)
    {
    }
    /// <summary>The log owner type</summary>
    protected override string LogOwnerType => nameof(PayrunEndFunction);

    #region Runtime Values

    /// <inheritdoc />
    public Dictionary<string, string> GetPayrunRuntimeValues()
    {
        // implementation
        return default;
    }

    /// <inheritdoc />
    public List<string> GetRuntimeValuesEmployees()
    {
        // implementation
        return default;
    }

    /// <inheritdoc />
    public Dictionary<string, string> GetEmployeeRuntimeValues(string employeeIdentifier)
    {
        // implementation
        return default;
    }

    #endregion

}