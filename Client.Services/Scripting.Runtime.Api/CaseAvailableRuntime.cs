﻿using PayrollEngine.Client.Model;
using PayrollEngine.Client.Scripting.Function;

namespace PayrollEngine.Client.Scripting.Runtime.Api;

/// <summary>Runtime for the case available function</summary>
public class CaseAvailableRuntime : CaseRuntimeBase, ICaseAvailableRuntime
{
    /// <summary>Initializes a new instance of the <see cref="CaseAvailableRuntime"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="scriptCalendar">The calendar</param>
    /// <param name="tenantId">The tenant id</param>
    /// <param name="userId">The user id</param>
    /// <param name="payrollId">The payroll id</param>
    /// <param name="case">The runtime case</param>
    /// <param name="employeeId">The employee id</param>
    public CaseAvailableRuntime(PayrollHttpClient httpClient, ScriptCalendar scriptCalendar, int tenantId,
        int userId, int payrollId, Case @case, int? employeeId = null) :
        base(httpClient, scriptCalendar, tenantId, userId, payrollId, @case, employeeId)
    {
    }

    /// <summary>The log owner type</summary>
    protected override string LogOwnerType => nameof(CaseAvailableFunction);

    /// <inheritdoc />
    public string[] GetAvailableActions() =>
        Case.AvailableActions == null ? [] :
            Case.AvailableActions.ToArray();

}