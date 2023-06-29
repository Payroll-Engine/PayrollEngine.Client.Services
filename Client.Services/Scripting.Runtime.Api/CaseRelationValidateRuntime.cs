using System;
using System.Collections.Generic;
using System.Linq;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Scripting.Function;

namespace PayrollEngine.Client.Scripting.Runtime.Api;

/// <summary>Runtime for the case relation validate function</summary>
public class CaseRelationValidateRuntime : CaseRelationRuntimeBase, ICaseRelationValidateRuntime
{
    /// <summary>The validation issues</summary>
    public List<CaseValidationIssue> Issues { get; } = new();

    /// <summary>Initializes a new instance of the <see cref="CaseRelationValidateRuntime"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="calendar">The calendar</param>
    /// <param name="tenantId">The tenant id</param>
    /// <param name="userId">The user id</param>
    /// <param name="payrollId">The payroll id</param>
    /// <param name="sourceCaseSet">The source case set</param>
    /// <param name="targetCaseSet">The target case set</param>
    /// <param name="employeeId">The employee id</param>
    public CaseRelationValidateRuntime(PayrollHttpClient httpClient, ScriptingCalendar calendar, int tenantId,
        int payrollId, int userId, CaseSet sourceCaseSet, CaseSet targetCaseSet, int? employeeId = null) :
        base(httpClient, calendar, tenantId, userId, payrollId, sourceCaseSet, targetCaseSet, employeeId)
    {
    }

    /// <summary>The log owner type</summary>
    protected override string LogOwnerType => nameof(CaseRelationValidateFunction);

    /// <inheritdoc />
    public string[] GetValidateActions() => Array.Empty<string>();

    /// <inheritdoc />
    public bool HasIssues() => Issues.Any();

    /// <inheritdoc />
    public void AddIssue(string message, int number)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException(nameof(message));
        }

        Issues.Add(new()
        {
            IssueType = CaseIssueType.CaseRelationInvalid,
            Number = number,
            SourceCaseName = SourceCaseSet.Name,
            SourceCaseSlot = SourceCaseSet.CaseSlot,
            TargetCaseName = TargetCaseSet.Name,
            TargetCaseSlot = TargetCaseSet.CaseSlot,
            Message = message
        });
    }

    /// <inheritdoc />
    public void AddIssue(string caseFieldName, string message, int number)
    {
        if (string.IsNullOrWhiteSpace(caseFieldName))
        {
            throw new ArgumentException(nameof(caseFieldName));
        }
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException(nameof(message));
        }

        Issues.Add(new()
        {
            IssueType = CaseIssueType.CaseRelationInvalid,
            Number = number,
            CaseFieldName = caseFieldName,
            SourceCaseName = SourceCaseSet.Name,
            SourceCaseSlot = SourceCaseSet.CaseSlot,
            TargetCaseName = TargetCaseSet.Name,
            TargetCaseSlot = TargetCaseSet.CaseSlot,
            Message = message
        });
    }
}