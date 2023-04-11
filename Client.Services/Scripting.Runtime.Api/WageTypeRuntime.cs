using System;
using System.Collections.Generic;

namespace PayrollEngine.Client.Scripting.Runtime.Api;

/// <summary>Runtime for the wage type function</summary>
public abstract class WageTypeRuntime : PayrunRuntime, IWageTypeRuntime
{
    /// <summary>Initializes a new instance of the <see cref="WageTypeRuntime"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="calendar">The calendar</param>
    /// <param name="tenantId">The tenant id</param>
    /// <param name="userId">The user id</param>
    /// <param name="payrollId">The payroll id</param>
    /// <param name="employeeId">The employee id</param>
    protected WageTypeRuntime(PayrollHttpClient httpClient, ScriptingCalendar calendar, int tenantId,
        int userId, int payrollId, int? employeeId = null) :
        base(httpClient, calendar, tenantId, userId, payrollId, employeeId)
    {
    }

    /// <inheritdoc />
    public decimal WageTypeNumber =>
        // implementation
        default;

    /// <inheritdoc />
    public string WageTypeName =>
        // implementation
        default;

    /// <inheritdoc />
    public string WageTypeDescription =>
        // implementation
        default;

    /// <inheritdoc />
    public string[] Collectors =>
        // implementation
        default;

    /// <inheritdoc />
    public string[] CollectorGroups =>
        // implementation
        default;

    /// <inheritdoc />
    public List<string> GetResultTags()
    {
        // implementation
        return default;
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
        return default;
    }

    /// <inheritdoc />
    public decimal GetWageTypeValue(decimal wageTypeNumber)
    {
        // implementation
        return default;
    }

    /// <inheritdoc />
    public decimal GetCollectorValue(string collectorName)
    {
        // implementation
        return default;
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
        return default;
    }

    /// <inheritdoc />
    public void SetResultAttribute(string name, object value)
    {
        // implementation
    }

    /// <inheritdoc />
    public void AddPayrunResult(string source, string name, string value, int valueType,
        DateTime startDate, DateTime endDate, string slot, List<string> tags, Dictionary<string, object> attributes)
    {
        // implementation
    }

    /// <inheritdoc />
    public void AddCustomResult(string source, decimal value, DateTime startDate,
        DateTime endDate, List<string> tags, Dictionary<string, object> attributes, int? valueType)
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