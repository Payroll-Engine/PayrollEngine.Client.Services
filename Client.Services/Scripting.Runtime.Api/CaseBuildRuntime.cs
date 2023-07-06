using System;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Scripting.Function;

namespace PayrollEngine.Client.Scripting.Runtime.Api;

/// <summary>Runtime for the case build function</summary>
public class CaseBuildRuntime : CaseChangeRuntimeBase, ICaseBuildRuntime
{
    /// <summary>Initializes a new instance of the <see cref="CaseBuildRuntime"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="calendar">The calendar</param>
    /// <param name="tenantId">The tenant id</param>
    /// <param name="userId">The user id</param>
    /// <param name="payrollId">The payroll id</param>
    /// <param name="caseSet">The runtime case set</param>
    /// <param name="employeeId">The employee id</param>
    public CaseBuildRuntime(PayrollHttpClient httpClient, ScriptCalendar calendar, int tenantId,
        int userId, int payrollId, CaseSet caseSet, int? employeeId = null) :
        base(httpClient, calendar, tenantId, userId, payrollId, caseSet, employeeId)
    {
    }

    /// <summary>The log owner type</summary>
    protected override string LogOwnerType => nameof(CaseBuildFunction);

    #region Case

    /// <inheritdoc />
    public string[] GetBuildActions() =>
        Case.BuildActions == null ? Array.Empty<string>() :
            Case.BuildActions.ToArray();

    #endregion

    #region Case Field

    /// <inheritdoc />
    public string[] GetFieldBuildActions(string caseFieldName)
    {
        if (string.IsNullOrWhiteSpace(caseFieldName))
        {
            throw new ArgumentException(nameof(caseFieldName));
        }

        // case field
        var caseField = GetCaseFieldSet(caseFieldName);
        if (caseField == null)
        {
            throw new ArgumentException($"unknown case field {caseFieldName}");
        }
        return caseField.BuildActions == null ? Array.Empty<string>() : caseField.BuildActions.ToArray();
    }

    #endregion

}