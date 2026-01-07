using System;
using System.Collections.Generic;

namespace PayrollEngine.Client.Scripting.Runtime.Api;

/// <summary>Runtime for the wage type function</summary>
public abstract class WageTypeRuntimeBase : PayrunRuntimeBase, IWageTypeRuntime
{
    /// <summary>Initializes a new instance of the <see cref="WageTypeRuntimeBase"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="scriptContext">The script context</param>
    /// <param name="tenantId">The tenant id</param>
    /// <param name="userId">The user id</param>
    /// <param name="payrollId">The payroll id</param>
    /// <param name="employeeId">The employee id</param>
    protected WageTypeRuntimeBase(PayrollHttpClient httpClient, ScriptContext scriptContext, int tenantId,
        int userId, int payrollId, int? employeeId = null) :
        base(httpClient, scriptContext, tenantId, userId, payrollId, employeeId)
    {
    }

    /// <inheritdoc />
    public decimal WageTypeNumber =>
        // implementation
        0;

    /// <inheritdoc />
    public string WageTypeName =>
        // implementation
        null;

    /// <inheritdoc />
    public string WageTypeDescription =>
        // implementation
        null;

    /// <inheritdoc />
    public string WageTypeCalendar =>
        // implementation
        null;

    /// <inheritdoc />
    public string[] Collectors =>
        // implementation
        null;

    /// <inheritdoc />
    public string[] CollectorGroups =>
        // implementation
        null;

    /// <inheritdoc />
    public List<string> GetResultTags()
    {
        // implementation
        return null;
    }

    /// <inheritdoc />
    public void SetResultTags(List<string> tags)
    {
        // implementation
    }

    /// <inheritdoc />
    public object GetWageTypeAttribute(string attributeName)
    {
        // implementation
        return null;
    }

    /// <inheritdoc />
    public decimal GetWageType(decimal wageTypeNumber)
    {
        // implementation
        return 0;
    }

    /// <inheritdoc />
    public decimal GetWageType(string wageTypeName)
    {
        // implementation
        return 0;
    }

    /// <inheritdoc />
    public decimal GetCollector(string collectorName)
    {
        // implementation
        return 0;
    }

    /// <inheritdoc />
    public void EnableCollector(string collectorName)
    {
        // implementation
    }

    /// <inheritdoc />
    public void DisableCollector(string collectorName)
    {
        // implementation
    }

    #region Results

    /// <inheritdoc />
    public object GetResultAttribute(string name)
    {
        // implementation
        return null;
    }

    /// <inheritdoc />
    public void SetResultAttribute(string name, object value)
    {
        // implementation
    }

    /// <inheritdoc />
    public void AddCustomResult(string source, decimal value, DateTime startDate,
        DateTime endDate, List<string> tags, Dictionary<string, object> attributes,
        int? valueType, string culture)
    {
        // implementation
    }

    #endregion

    #region Retro

    /// <inheritdoc />
    public void ScheduleRetroPayrun(DateTime scheduleDate, List<string> resultTags)
    {
        // implementation
    }

    #endregion

}