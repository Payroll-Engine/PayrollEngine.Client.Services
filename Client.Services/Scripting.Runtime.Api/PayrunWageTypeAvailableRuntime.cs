using PayrollEngine.Client.Scripting.Function;

namespace PayrollEngine.Client.Scripting.Runtime.Api;

/// <summary>Runtime for the payrun wage type available function</summary>
public class PayrunWageTypeAvailableRuntime : PayrunRuntimeBase, IPayrunWageTypeAvailableRuntime
{
    /// <summary>Initializes a new instance of the <see cref="PayrunWageTypeAvailableRuntime"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="calendar">The calendar</param>
    /// <param name="tenantId">The tenant id</param>
    /// <param name="userId">The user id</param>
    /// <param name="payrollId">The payroll id</param>
    /// <param name="employeeId">The employee id</param>
    public PayrunWageTypeAvailableRuntime(PayrollHttpClient httpClient, ScriptCalendar calendar,
        int tenantId, int userId, int payrollId, int? employeeId = null) :
        base(httpClient, calendar, tenantId, userId, payrollId, employeeId)
    {
    }

    /// <summary>The log owner type</summary>
    protected override string LogOwnerType => nameof(PayrunWageTypeAvailableFunction);

    /// <inheritdoc />
    public decimal WageTypeNumber =>
        // implementation
        0;

    /// <inheritdoc />
    public object GetWageTypeAttribute(string attributeName)
    {
        // implementation
        return null;
    }
}