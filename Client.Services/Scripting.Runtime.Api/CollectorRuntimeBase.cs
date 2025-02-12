using System;
using System.Collections.Generic;

namespace PayrollEngine.Client.Scripting.Runtime.Api;

/// <summary>Runtime for the collector function</summary>
public abstract class CollectorRuntimeBase : PayrunRuntimeBase, ICollectorRuntime
{
    /// <summary>Initializes a new instance of the <see cref="CollectorRuntimeBase"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="calendar">The calendar</param>
    /// <param name="tenantId">The tenant id</param>
    /// <param name="userId">The user id</param>
    /// <param name="payrollId">The payroll id</param>
    /// <param name="employeeId">The employee id</param>
    protected CollectorRuntimeBase(PayrollHttpClient httpClient, ScriptCalendar calendar, int tenantId,
        int userId, int payrollId, int? employeeId = null) :
        base(httpClient, calendar, tenantId, userId, payrollId, employeeId)
    {
    }

    /// <inheritdoc />
    public string CollectorName =>
        // implementation
        null;

    /// <inheritdoc />
    public string[] CollectorGroups =>
        // implementation
        null;

    /// <inheritdoc />
    public string CollectMode =>
        // implementation
        null;

    /// <inheritdoc />
    public bool Negated =>
        // implementation
        false;

    /// <inheritdoc />
    public decimal? CollectorThreshold =>
        // implementation
        null;

    /// <inheritdoc />
    public decimal? CollectorMinResult =>
        // implementation
        null;

    /// <inheritdoc />
    public decimal? CollectorMaxResult =>
        // implementation
        null;

    /// <inheritdoc />
    public decimal CollectorResult =>
        // implementation
        0;

    /// <inheritdoc />
    public decimal CollectorCount =>
        // implementation
        0;

    /// <inheritdoc />
    public decimal CollectorSummary =>
        // implementation
        0;

    /// <inheritdoc />
    public decimal CollectorMinimum =>
        // implementation
        0;

    /// <inheritdoc />
    public decimal CollectorMaximum =>
        // implementation
        0;

    /// <inheritdoc />
    public decimal CollectorAverage =>
        // implementation
        0;

    /// <inheritdoc />
    public decimal CollectorRange =>
        // implementation
        0;

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
    public object GetCollectorAttribute(string attributeName)
    {
        // implementation
        return null;
    }

    /// <summary>The log owner type</summary>
    protected override string LogOwner => CollectorName;

    /// <inheritdoc />
    public decimal GetCollectorValue(string collectorName)
    {
        // implementation
        return 0;
    }

    /// <inheritdoc />
    public void Reset()
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
    public void AddPayrunResult(string source, string name, string value, int valueType,
        DateTime startDate, DateTime endDate, string slot, List<string> tags, Dictionary<string, object> attributes)
    {
        // implementation
    }

    /// <inheritdoc />
    public void AddCustomResult(string source, decimal value, DateTime startDate, DateTime endDate,
        List<string> tags, Dictionary<string, object> attributes, int? valueType)
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