using PayrollEngine.Client.Model;
using PayrollEngine.Client.Scripting.Function;
using System;

namespace PayrollEngine.Client.Scripting.Runtime.Api;

/// <summary>Runtime for the case relation build function</summary>
public class CaseRelationBuildRuntime : CaseRelationRuntimeBase, ICaseRelationBuildRuntime
{
    /// <summary>Initializes a new instance of the <see cref="CaseRelationBuildRuntime"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="calendar">The calendar</param>
    /// <param name="tenantId">The tenant id</param>
    /// <param name="userId">The user id</param>
    /// <param name="payrollId">The payroll id</param>
    /// <param name="sourceCaseSet">The source case set</param>
    /// <param name="targetCaseSet">The target case set</param>
    /// <param name="employeeId">The employee id</param>
    public CaseRelationBuildRuntime(PayrollHttpClient httpClient, ScriptCalendar calendar, int tenantId,
        int payrollId, int userId, CaseSet sourceCaseSet, CaseSet targetCaseSet, int? employeeId = null) :
        base(httpClient, calendar, tenantId, userId, payrollId, sourceCaseSet, targetCaseSet, employeeId)
    {
    }

    /// <summary>The log owner type</summary>
    protected override string LogOwnerType => nameof(CaseRelationBuildFunction);

    /// <inheritdoc />
    public string[] GetBuildActions() => Array.Empty<string>();
}