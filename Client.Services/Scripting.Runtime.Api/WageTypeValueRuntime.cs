﻿using PayrollEngine.Client.Scripting.Function;

namespace PayrollEngine.Client.Scripting.Runtime.Api;

/// <summary>Runtime for the wage type value function</summary>
public class WageTypeValueRuntime : WageTypeRuntimeBase, IWageTypeValueRuntime
{
    /// <summary>Initializes a new instance of the <see cref="WageTypeValueRuntime"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="calendar">The calendar</param>
    /// <param name="tenantId">The tenant id</param>
    /// <param name="userId">The user id</param>
    /// <param name="payrollId">The payroll id</param>
    /// <param name="employeeId">The employee id</param>
    public WageTypeValueRuntime(PayrollHttpClient httpClient, ScriptCalendar calendar, int tenantId,
        int userId, int payrollId, int? employeeId = null) :
        base(httpClient, calendar, tenantId, userId, payrollId, employeeId)
    {
    }

    /// <summary>The log owner type</summary>
    protected override string LogOwnerType => nameof(WageTypeValueFunction);

    /// <inheritdoc />
    public int ExecutionCount =>
        // implementation
        0;

    /// <inheritdoc />
    public void RestartExecution()
    {
        // implementation
    }
}