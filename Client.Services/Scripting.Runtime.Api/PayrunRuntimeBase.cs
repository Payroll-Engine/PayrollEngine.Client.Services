using System;
using System.Collections.Generic;

namespace PayrollEngine.Client.Scripting.Runtime.Api;

/// <summary>Runtime for the payrun function</summary>
public abstract class PayrunRuntimeBase : PayrollRuntime, IPayrunRuntime
{
    /// <summary>Initializes a new instance of the <see cref="PayrunRuntimeBase"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="calendar">The calendar</param>
    /// <param name="tenantId">The tenant id</param>
    /// <param name="userId">The user id</param>
    /// <param name="employeeId">The employee id</param>
    /// <param name="payrollId">The payroll id</param>
    protected PayrunRuntimeBase(PayrollHttpClient httpClient, ScriptCalendar calendar, int tenantId,
        int userId, int payrollId, int? employeeId = null) :
        base(httpClient, calendar, tenantId, userId, payrollId, employeeId)
    {
    }

    /// <summary>The log owner type</summary>
    protected override string LogOwner => PayrunName;

    #region Payrun

    /// <inheritdoc />
    public int PayrunId =>
        // implementation
        0;

    /// <inheritdoc />
    public string PayrunName =>
        // implementation
        null;

    #endregion

    #region PayrunJob

    /// <inheritdoc />
    public int ExecutionPhase =>
        // implementation
        0;

    /// <inheritdoc />
    public Tuple<DateTime, DateTime> RetroPeriod =>
        // implementation
        null;

    /// <inheritdoc />
    public string Forecast =>
        // implementation
        null;

    /// <inheritdoc />
    public string CycleName =>
        // implementation
        null;

    /// <inheritdoc />
    public string PeriodName =>
        // implementation
        null;

    /// <inheritdoc />
    public object GetPayrunJobAttribute(string attributeName)
    {
        // implementation
        return null;
    }

    /// <inheritdoc />
    public void SetPayrunJobAttribute(string attributeName, object value)
    {
        // implementation
    }

    /// <inheritdoc />
    public bool RemovePayrunJobAttribute(string attributeName)
    {
        // implementation
        return false;
    }

    #endregion

    #region Runtime Values

    /// <inheritdoc />
    public bool HasPayrunRuntimeValue(string key)
    {
        // implementation
        return false;
    }

    /// <inheritdoc />
    public string GetPayrunRuntimeValue(string key)
    {
        // implementation
        return null;
    }

    /// <inheritdoc />
    public void SetPayrunRuntimeValue(string key, string value)
    {
        // implementation
    }

    /// <inheritdoc />
    public bool HasEmployeeRuntimeValue(string key)
    {
        // implementation
        return false;
    }

    /// <inheritdoc />
    public string GetEmployeeRuntimeValue(string key)
    {
        // implementation
        return null;
    }

    /// <inheritdoc />
    public void SetEmployeeRuntimeValue(string key, string value)
    {
        // implementation
    }

    #endregion

    #region Payrun Results

    /// <inheritdoc />
    public void AddPayrunResult(string source, string name, string value, int valueType,
        DateTime startDate, DateTime endDate, string slot, List<string> tags,
        Dictionary<string, object> attributes, string culture)
    {
        // implementation
    }

    #endregion

    #region Wage Type Results

    /// <inheritdoc />
    public IList<Tuple<decimal, string, Tuple<DateTime, DateTime>, decimal, List<string>, Dictionary<string, object>>> GetWageTypeResults(
        IList<decimal> wageTypeNumbers, DateTime start, DateTime end, string forecast = null, int? jobStatus = null, IList<string> tags = null)
    {
        // implementation
        return null;
    }

    /// <inheritdoc />
    public IList<Tuple<decimal, string, Tuple<DateTime, DateTime>, decimal, List<string>, Dictionary<string, object>>> GetConsolidatedWageTypeResults(
        IList<decimal> wageTypeNumbers, DateTime periodMoment, string forecast = null, int? jobStatus = null, IList<string> tags = null)
    {
        // implementation
        return null;
    }

    /// <inheritdoc />
    public IList<Tuple<decimal, string, string, Tuple<DateTime, DateTime>, decimal, List<string>, Dictionary<string, object>>> GetWageTypeCustomResults(IList<decimal> wageTypeNumbers, DateTime start, DateTime end, string forecast = null,
        int? jobStatus = null, IList<string> tags = null)
    {
        // implementation
        return null;
    }

    /// <inheritdoc />
    public IList<Tuple<decimal, string, string, Tuple<DateTime, DateTime>, decimal, List<string>, Dictionary<string, object>>> GetConsolidatedWageTypeCustomResults(IList<decimal> wageTypeNumbers,
        DateTime periodMoment, string forecast = null, int? jobStatus = null, IList<string> tags = null)
    {
        // implementation
        return null;
    }

    /// <inheritdoc />
    public IList<decimal> GetRetroWageTypeResults(decimal wageTypeNumber,
        string forecast = null, int? jobStatus = null, IList<string> tags = null)
    {
        // implementation
        return null;
    }

    #endregion

    #region Collector Results

    /// <inheritdoc />
    public IList<Tuple<string, Tuple<DateTime, DateTime>, decimal, List<string>, Dictionary<string, object>>> GetCollectorResults(IList<string> collectorNames,
        DateTime start, DateTime end, string forecast = null, int? jobStatus = null, IList<string> tags = null)
    {
        // implementation
        return null;
    }

    /// <inheritdoc />
    public IList<Tuple<string, Tuple<DateTime, DateTime>, decimal, List<string>, Dictionary<string, object>>> GetConsolidatedCollectorResults(
        IList<string> collectorNames, DateTime periodMoment, string forecast = null, int? jobStatus = null, IList<string> tags = null)
    {
        // implementation
        return null;
    }

    /// <inheritdoc />
    public IList<Tuple<string, string, Tuple<DateTime, DateTime>, decimal, List<string>, Dictionary<string, object>>> GetCollectorCustomResults(IList<string> collectorNames, DateTime start, DateTime end, string forecast = null,
        int? jobStatus = null, IList<string> tags = null)
    {
        // implementation
        return null;
    }

    /// <inheritdoc />
    public IList<Tuple<string, string, Tuple<DateTime, DateTime>, decimal, List<string>, Dictionary<string, object>>> GetConsolidatedCollectorCustomResults(
        IList<string> collectorNames, DateTime periodMoment, string forecast = null, int? jobStatus = null, IList<string> tags = null)
    {
        // implementation
        return null;
    }

    #endregion

}