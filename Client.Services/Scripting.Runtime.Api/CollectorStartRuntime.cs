﻿using PayrollEngine.Client.Scripting.Function;

namespace PayrollEngine.Client.Scripting.Runtime.Api;

/// <summary>Runtime for the collector start function</summary>
public class CollectorStartRuntime : CollectorRuntimeBase, ICollectorStartRuntime
{
    /// <summary>Initializes a new instance of the <see cref="CollectorStartRuntime"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="calendar">The calendar</param>
    /// <param name="tenantId">The tenant id</param>
    /// <param name="userId">The user id</param>
    /// <param name="employeeId">The employee id</param>
    /// <param name="payrollId">The payroll id</param>
    public CollectorStartRuntime(PayrollHttpClient httpClient, ScriptCalendar calendar, int tenantId,
        int userId, int payrollId, int? employeeId = null) :
        base(httpClient, calendar, tenantId, userId, payrollId, employeeId)
    {
    }

    /// <summary>The log owner type</summary>
    protected override string LogOwnerType => nameof(CollectorStartFunction);

    /// <inheritdoc />
    public decimal[] GetValues()
    {
        // implementation
        return null;
    }

    /// <inheritdoc />
    public void SetValues(decimal[] values)
    {
        // implementation
    }
}