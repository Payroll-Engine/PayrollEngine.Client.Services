﻿using System;
using System.Collections.Generic;
using System.Linq;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Scripting.Function;

namespace PayrollEngine.Client.Scripting.Runtime.Api;

/// <summary>Runtime for the case validate function</summary>
public class CaseValidateRuntime : CaseChangeRuntimeBase, ICaseValidateRuntime
{
    /// <summary>The validation issues</summary>
    public List<CaseValidationIssue> Issues { get; } = [];

    /// <summary>Initializes a new instance of the <see cref="CaseValidateRuntime"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="calendar">The calendar</param>
    /// <param name="tenantId">The tenant id</param>
    /// <param name="userId">The user id</param>
    /// <param name="payrollId">The payroll id</param>
    /// <param name="caseSet">The runtime case set</param>
    /// <param name="employeeId">The employee id</param>
    public CaseValidateRuntime(PayrollHttpClient httpClient, ScriptCalendar calendar, int tenantId,
        int userId, int payrollId, CaseSet caseSet, int? employeeId = null) :
        base(httpClient, calendar, tenantId, userId, payrollId, caseSet, employeeId)
    {
    }

    /// <summary>The log owner type</summary>
    protected override string LogOwnerType => nameof(CaseValidateFunction);

    /// <inheritdoc />
    public string[] GetValidateActions() =>
        Case.ValidateActions == null ? [] :
            Case.ValidateActions.ToArray();

    /// <inheritdoc />
    public string[] GetFieldValidateActions(string caseFieldName)
    {
        if (string.IsNullOrWhiteSpace(caseFieldName))
        {
            throw new ArgumentException(nameof(caseFieldName));
        }

        // case field
        var caseField = GetCaseFieldSet(caseFieldName);
        if (caseField == null)
        {
            throw new ArgumentException($"unknown case field {caseFieldName}.");
        }
        return caseField.ValidateActions == null ? [] : caseField.ValidateActions.ToArray();
    }

    /// <inheritdoc />
    public bool HasIssues() => Issues.Any();

    /// <inheritdoc />
    public void AddIssue(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException(nameof(message));
        }

        Issues.Add(new()
        {
            IssueType = CaseIssueType.CaseInvalid,
            CaseName = Case.Name,
            CaseSlot = Case.CaseSlot,
            Message = message
        });
    }

    /// <inheritdoc />
    public void AddIssue(string caseFieldName, string message)
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
            IssueType = CaseIssueType.CaseInvalid,
            CaseName = Case.Name,
            CaseSlot = Case.CaseSlot,
            CaseFieldName = caseFieldName,
            Message = message
        });
    }
}