using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;
using CultureInfo = System.Globalization.CultureInfo;

namespace PayrollEngine.Client.Scripting.Runtime.Api;

/// <summary>Runtime for the payroll function</summary>
public abstract class PayrollRuntime : Runtime, IPayrollRuntime
{
    /// <summary>The employee service</summary>
    protected IEmployeeService EmployeeService { get; }

    /// <summary>The payroll service</summary>
    protected IPayrollService PayrollService { get; }

    /// <summary>The regulation service</summary>
    protected IRegulationService RegulationService { get; }

    /// <summary>The calendar service</summary>
    protected ICalendarService CalendarService { get; }

    /// <summary>The calendar</summary>
    public ScriptingCalendar ScriptCalendar { get; }

    /// <summary>The language</summary>
    protected Language Language => ScriptCalendar.Language;

    /// <summary>Initializes a new instance of the <see cref="PayrollRuntime"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="scriptCalendar">The calendar</param>
    /// <param name="tenantId">The tenant id</param>
    /// <param name="userId">The user id</param>
    /// <param name="payrollId">The payroll id</param>
    /// <param name="employeeId">The employee id</param>
    protected PayrollRuntime(PayrollHttpClient httpClient, ScriptingCalendar scriptCalendar,
        int tenantId, int userId, int payrollId, int? employeeId = null) :
        base(httpClient, tenantId, userId)
    {
        ScriptCalendar = scriptCalendar ?? throw new ArgumentNullException(nameof(scriptCalendar));

        // employee
        if (employeeId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(employeeId));
        }
        EmployeeId = employeeId;

        // payroll
        if (payrollId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(payrollId));
        }
        PayrollId = payrollId;

        // services
        EmployeeService = new EmployeeService(httpClient);
        PayrollService = new PayrollService(httpClient);
        RegulationService = new RegulationService(httpClient);

        // periods
        // TODO: make cycle/period configurable

        // culture
        var culture = CultureInfo.CurrentCulture;

        // calendar
        CalendarService = new CalendarService(httpClient);
        Calendar calendar = null;
        if (!string.IsNullOrWhiteSpace(scriptCalendar.CalendarName))
        {
            calendar = new CalendarService(httpClient).
                GetAsync<Calendar>(new(tenantId), scriptCalendar.CalendarName).Result;
        }
        calendar ??= new();

        Cycle = new YearPayrollCycle(culture, calendar, scriptCalendar.PeriodDate);
        Period = new MonthPayrollPeriod(culture, calendar, scriptCalendar.PeriodDate);
    }

    #region Employee

    /// <inheritdoc />
    public int? EmployeeId { get; }

    private Employee employee;

    /// <summary>The employee</summary>
    protected Employee Employee
    {
        get
        {
            if (employee == null)
            {
                if (!EmployeeId.HasValue)
                {
                    throw new PayrollException("Employee not available");
                }
                employee = EmployeeService.GetAsync<Employee>(
                    new(TenantId), EmployeeId.Value).Result;
            }
            return employee;
        }
    }

    /// <inheritdoc />
    public string EmployeeIdentifier => Employee.Identifier;

    /// <inheritdoc />
    public string EmployeeCulture => Employee.Culture;

    /// <inheritdoc />
    public string EmployeeCalendar => Employee.Calendar;

    /// <inheritdoc />
    public virtual object GetEmployeeAttribute(string attributeName)
    {
        if (!EmployeeId.HasValue)
        {
            throw new PayrollException("Employee not available");
        }
        var jsonValue = EmployeeService.GetAttributeAsync(new(TenantId), TenantId, attributeName).Result;
        if (string.IsNullOrWhiteSpace(jsonValue))
        {
            return null;
        }
        return JsonSerializer.Deserialize<object>(jsonValue);
    }

    #endregion

    #region Payroll

    /// <summary>The payroll id</summary>
    public int PayrollId { get; }

    private Payroll payroll;

    /// <summary>The payroll</summary>
    public Payroll Payroll
    {
        get
        {
            if (payroll == null)
            {
                // load country from payroll
                payroll = PayrollService.GetAsync<Payroll>(new(TenantId), PayrollId).Result;
                if (payroll == null)
                {
                    throw new PayrollException($"Unknown payroll with id {PayrollId}");
                }
            }
            return payroll;
        }
    }

    private int? payrollCountry;
    /// <inheritdoc />
    public int PayrollCountry
    {
        get
        {
            payrollCountry ??= Payroll.Country;
            return payrollCountry.Value;
        }
    }

    #endregion

    #region Calendar

    /// <summary>The payroll cycle</summary>
    public IPayrollPeriod Cycle { get; }

    /// <summary>The payroll period</summary>
    public IPayrollPeriod Period { get; }

    /// <summary>The regulation date</summary>
    public DateTime RegulationDate => ScriptCalendar.RegulationDate;

    /// <inheritdoc />
    public DateTime EvaluationDate => ScriptCalendar.EvaluationDate;

    #endregion

    #region Period

    /// <inheritdoc />
    public virtual Tuple<DateTime, DateTime> GetEvaluationPeriod()
    {
        var period = CalendarService.GetPeriodAsync(TenantId,
            cultureName: ScriptCalendar.CultureName,
            calendarName: ScriptCalendar.CalendarName,
            periodMoment: EvaluationDate).Result;
        return new(period.Start, period.End);
    }

    /// <inheritdoc />
    public virtual Tuple<DateTime, DateTime> GetPeriod(DateTime periodMoment, int offset)
    {
        var period = CalendarService.GetPeriodAsync(TenantId,
            cultureName: ScriptCalendar.CultureName,
            calendarName: ScriptCalendar.CalendarName,
            periodMoment: periodMoment,
            offset: offset).Result;
        return new(period.Start, period.End);
    }

    #endregion

    #region Cycle

    /// <inheritdoc />
    public virtual Tuple<DateTime, DateTime> GetCycle(DateTime cycleMoment, int offset)
    {
        var cycle = CalendarService.GetCycleAsync(TenantId,
            cultureName: ScriptCalendar.CultureName,
            calendarName: ScriptCalendar.CalendarName,
            cycleMoment: cycleMoment,
            offset: offset).Result;
        return new(cycle.Start, cycle.End);
    }

    #endregion

    #region Case Field and Case Value

    /// <inheritdoc />
    public virtual int? GetCaseValueType(string caseFieldName)
    {
        if (string.IsNullOrWhiteSpace(caseFieldName))
        {
            throw new ArgumentException(nameof(caseFieldName));
        }
        var context = new PayrollServiceContext(TenantId, PayrollId);
        var caseField = PayrollService.GetCaseFieldsAsync<CaseField>(context, new[] { caseFieldName })
            .Result.FirstOrDefault();
        return caseField != null ? (int)caseField.ValueType : null;
    }

    /// <inheritdoc />
    public virtual object GetCaseFieldAttribute(string caseFieldName, string attributeName)
    {
        if (string.IsNullOrWhiteSpace(caseFieldName))
        {
            throw new ArgumentException(nameof(caseFieldName));
        }
        var context = new PayrollServiceContext(TenantId, PayrollId);
        var caseField = PayrollService.GetCaseFieldsAsync<CaseField>(context, new[] { caseFieldName })
            .Result.FirstOrDefault();
        return caseField?.Attributes?.GetValue<object>(attributeName);
    }

    /// <inheritdoc />
    public virtual object GetCaseValueAttribute(string caseFieldName, string attributeName)
    {
        if (string.IsNullOrWhiteSpace(caseFieldName))
        {
            throw new ArgumentException(nameof(caseFieldName));
        }
        var context = new PayrollServiceContext(TenantId, PayrollId);
        var caseField = PayrollService.GetCaseFieldsAsync<CaseField>(context, new[] { caseFieldName })
            .Result.FirstOrDefault();
        return caseField?.ValueAttributes?.GetValue<object>(attributeName);
    }

    /// <inheritdoc />
    public virtual List<string> GetCaseValueSlots(string caseFieldName)
    {
        if (string.IsNullOrWhiteSpace(caseFieldName))
        {
            throw new ArgumentException(nameof(caseFieldName));
        }

        // case
        var caseName = new RegulationService(HttpClient).GetCaseOfCaseFieldAsync(
            new(TenantId), caseFieldName).Result;
        if (string.IsNullOrWhiteSpace(caseName))
        {
            throw new ScriptException($"Unknown case field: {caseFieldName}");
        }
        var @case = PayrollService.BuildCaseAsync<CaseSet>(
            new(TenantId, PayrollId), caseName, UserId, EmployeeId).Result;
        if (@case == null || @case.Id == default)
        {
            return new();
        }

        // case value slots
        switch (@case.CaseType)
        {
            case PayrollEngine.CaseType.Global:
                return new GlobalCaseValueService(HttpClient).GetCaseValueSlotsAsync(
                    new(TenantId), caseFieldName).Result.ToList();
            case PayrollEngine.CaseType.National:
                return new NationalCaseValueService(HttpClient).GetCaseValueSlotsAsync(
                    new(TenantId), caseFieldName).Result.ToList();
            case PayrollEngine.CaseType.Company:
                return new CompanyCaseValueService(HttpClient).GetCaseValueSlotsAsync(
                    new(TenantId), caseFieldName).Result.ToList();
            case PayrollEngine.CaseType.Employee:
                if (!EmployeeId.HasValue)
                {
                    throw new InvalidOperationException("Missing employee");
                }
                return new EmployeeCaseValueService(HttpClient).GetCaseValueSlotsAsync(
                    new(TenantId, EmployeeId.Value), caseFieldName).Result.ToList();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <inheritdoc />
    public virtual List<string> GetCaseValueTags(string caseFieldName, DateTime valueDate)
    {
        var caseValue = GetTimeCaseValue(caseFieldName, valueDate).Result;
        return caseValue == null ? new() : caseValue.Tags;
    }

    /// <inheritdoc />
    public virtual Tuple<string, DateTime, Tuple<DateTime?, DateTime?>, object, DateTime?, List<string>, Dictionary<string, object>> GetCaseValue(
        string caseFieldName, DateTime valueDate)
    {
        var caseValue = GetTimeCaseValue(caseFieldName, valueDate).Result;
        if (caseValue == null)
        {
            return null;
        }
        return new(caseFieldName,
            caseValue.Created,
            new(caseValue.Start, caseValue.End),
            ValueConvert.ToValue(caseValue.Value, caseValue.ValueType),
            caseValue.CancellationDate,
            caseValue.Tags,
            caseValue.Attributes);
    }

    private async Task<Model.CaseValue> GetTimeCaseValue(string caseFieldName, DateTime valueDate)
    {
        var context = new PayrollServiceContext(TenantId, PayrollId);

        // case field
        var caseField = await PayrollService.GetCaseFieldsAsync<CaseField>(context, new[] { caseFieldName });
        if (caseField == null)
        {
            throw new ArgumentException($"Unknown case field {caseFieldName}");
        }

        // case
        var caseValue = (await PayrollService.GetTimeValuesAsync(context, EmployeeId, new[] { caseFieldName },
            valueDate, RegulationDate, EvaluationDate)).FirstOrDefault();
        return caseValue;
    }

    /// <inheritdoc />
    public virtual List<Tuple<string, DateTime, Tuple<DateTime?, DateTime?>, object, DateTime?, List<string>, Dictionary<string, object>>> GetCaseValues(
        string caseFieldName, DateTime? startDate = null, DateTime? endDate = null)
    {
        if (string.IsNullOrWhiteSpace(caseFieldName))
        {
            throw new ArgumentException(nameof(caseFieldName));
        }
        startDate ??= Date.MinValue;
        endDate ??= Date.MaxValue;
        if (endDate < startDate)
        {
            throw new ArgumentException($"Invalid period end date: {endDate}", nameof(endDate));
        }

        // case value periods
        var caseRef = new CaseValueReference(caseFieldName);
        var valuePeriods = PayrollService.GetCaseValuesAsync(new(TenantId, PayrollId),
            startDate.Value, endDate.Value, new[] { caseRef.CaseFieldName }, EmployeeId, caseRef.CaseSlot).Result;

        // tuple build
        var values = new List<Tuple<string, DateTime, Tuple<DateTime?, DateTime?>, object, DateTime?, List<string>, Dictionary<string, object>>>();
        foreach (var valuePeriod in valuePeriods)
        {
            values.Add(new(valuePeriod.CaseFieldName,
                valuePeriod.Created,
                new(valuePeriod.Start, valuePeriod.End),
                ValueConvert.ToValue(valuePeriod.Value, valuePeriod.ValueType),
                valuePeriod.CancellationDate,
                valuePeriod.Tags,
                valuePeriod.Attributes));
        }
        return values;
    }

    /// <inheritdoc />
    public virtual Dictionary<string, List<Tuple<DateTime, DateTime?, DateTime?, object>>> GetCasePeriodValues(DateTime startDate,
        DateTime endDate, params string[] caseFieldNames)
    {
        // period values
        var periodValues = PayrollService.GetAvailableCaseFieldValuesAsync(new(TenantId, PayrollId),
            UserId,
            employeeId: EmployeeId,
            caseFieldNames: caseFieldNames,
            startDate: startDate,
            endDate: endDate,
            regulationDate: RegulationDate,
            evaluationDate: EvaluationDate).Result;

        // ensure for any requested a case field value
        var values = caseFieldNames.ToDictionary(
            caseFieldName => caseFieldName,
            _ => new List<Tuple<DateTime, DateTime?, DateTime?, object>>());
        foreach (var periodValue in periodValues)
        {
            values[periodValue.CaseFieldName].Add(new(periodValue.Created,
                periodValue.Start, periodValue.End, ValueConvert.ToValue(periodValue.Value, periodValue.ValueType)));
        }
        return values;
    }

    #endregion

    #region Regulation Lookup

    /// <inheritdoc />
    public virtual string GetLookup(string lookupName, string lookupKey, int? languageCode = null)
    {
        var language = languageCode.HasValue ? (PayrollEngine.Language)languageCode : default;
        var value = PayrollService.GetLookupValueDataAsync(new(TenantId, PayrollId),
            lookupName: lookupName,
            lookupKey: lookupKey,
            regulationDate: RegulationDate,
            evaluationDate: EvaluationDate,
            language: language).Result;
        var result = value?.Value;
        return result;
    }

    /// <inheritdoc />
    public virtual string GetRangeLookup(string lookupName, decimal rangeValue,
        string lookupKey = null, int? languageCode = null)
    {
        var language = languageCode.HasValue ? (PayrollEngine.Language)languageCode : default;
        var value = PayrollService.GetLookupValueDataAsync(new(TenantId, PayrollId),
            lookupName: lookupName,
            lookupKey: lookupKey,
            rangeValue: rangeValue,
            regulationDate: RegulationDate,
            evaluationDate: EvaluationDate,
            language: language).Result;
        return value?.Value;
    }

    #endregion

}