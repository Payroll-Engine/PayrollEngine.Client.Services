using System;
using System.Globalization;
using System.Collections.Generic;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;

namespace PayrollEngine.Client.Scripting.Runtime.Api;

/// <summary>Runtime for the function</summary>
public abstract class RuntimeBase : IRuntime
{
    /// <summary>The Payroll http client</summary>
    public PayrollHttpClient HttpClient { get; }

    /// <summary>The tenant service</summary>
    protected ITenantService TenantService { get; }

    /// <summary>The user service</summary>
    protected IUserService UserService { get; }

    /// <summary>Initializes a new instance of the <see cref="RuntimeBase"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="tenantId">The tenant id</param>
    /// <param name="userId">The user id</param>
    protected RuntimeBase(PayrollHttpClient httpClient, int tenantId, int userId)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        // tenant
        if (tenantId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tenantId));
        }
        TenantService = new TenantService(httpClient);
        Tenant = TenantService.GetAsync<Tenant>(new(), tenantId).Result;
        TenantCulture = GetTenantCulture(Tenant);

        // user
        if (userId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(userId));
        }
        UserService = new UserService(httpClient);
        User = UserService.GetAsync<User>(new(tenantId), userId).Result;
        // user culture by priority: user >  system
        UserCulture = User.Culture ?? CultureInfo.CurrentCulture.Name;
    }

    #region Tenant

    /// <summary>The tenant</summary>
    public Tenant Tenant { get; }

    /// <summary>The tenant culture</summary>
    public CultureInfo TenantCulture { get; }

    /// <inheritdoc />
    public int TenantId => Tenant.Id;

    /// <inheritdoc />
    public string TenantIdentifier => Tenant.Identifier;

    /// <inheritdoc />
    public object GetTenantAttribute(string attributeName) =>
        Tenant.GetAttribute(attributeName);

    private static CultureInfo GetTenantCulture(Tenant tenant)
    {
        var culture = CultureInfo.DefaultThreadCurrentCulture ?? CultureInfo.InvariantCulture;
        if (!string.IsNullOrWhiteSpace(tenant.Culture) &&
            !string.Equals(culture.Name, tenant.Culture))
        {
            culture = new CultureInfo(tenant.Culture);
        }
        return culture;
    }

    #endregion

    #region User

    /// <summary>The user</summary>
    public User User { get; }

    /// <inheritdoc />
    public int UserId => User.Id;

    /// <inheritdoc />
    public string UserIdentifier => User.Identifier;

    /// <inheritdoc />
    public virtual string UserCulture { get; }

    /// <inheritdoc />
    public int UserType => (int)User.UserType;

    /// <inheritdoc />
    public object GetUserAttribute(string attributeName) =>
        User.GetAttribute(attributeName);

    #endregion

    #region Culture and Calendar

    /// <inheritdoc />
    public string GetDerivedCulture(int divisionId, int employeeId)
    {
        if (divisionId <= 0 && employeeId > 0)
        {
            throw new ArgumentException(nameof(employeeId));
        }

        // priority 1: employee culture
        if (employeeId > 0)
        {
            var employee = new EmployeeService(HttpClient).GetAsync<Employee>(
                new(TenantId), employeeId).Result;
            if (employee == null)
            {
                throw new ScriptException($"Invalid employee id {employeeId}");
            }
            if (!string.IsNullOrWhiteSpace(employee.Culture))
            {
                return employee.Culture;
            }
        }

        // priority 2: division culture
        if (divisionId > 0)
        {
            var division = new DivisionService(HttpClient).GetAsync<Division>(
                new(TenantId), divisionId).Result;
            if (division == null)
            {
                throw new ScriptException($"Invalid division id {divisionId}");
            }
            if (!string.IsNullOrWhiteSpace(division.Culture))
            {
                return division.Culture;
            }
        }

        // priority 3: tenant culture
        return Tenant.Culture;
    }

    /// <inheritdoc />
    public string GetDerivedCalendar(int divisionId, int employeeId)
    {
        if (divisionId <= 0 && employeeId > 0)
        {
            throw new ArgumentException(nameof(employeeId));
        }

        // priority 1: employee calendar
        if (employeeId > 0)
        {
            var employee = new EmployeeService(HttpClient).GetAsync<Employee>(
                new(TenantId), employeeId).Result;
            if (employee == null)
            {
                throw new ScriptException($"Invalid employee id {employeeId}");
            }
            if (!string.IsNullOrWhiteSpace(employee.Calendar))
            {
                return employee.Calendar;
            }
        }

        // priority 2: division calendar
        if (divisionId > 0)
        {
            var division = new DivisionService(HttpClient).GetAsync<Division>(
                new(TenantId), divisionId).Result;
            if (division == null)
            {
                throw new ScriptException($"Invalid division id {divisionId}");
            }
            if (!string.IsNullOrWhiteSpace(division.Calendar))
            {
                return division.Calendar;
            }
        }

        // priority 3: tenant calendar
        return Tenant.Calendar;
    }

    /// <inheritdoc />
    public bool IsWorkDay(string calendarName, DateTime moment) =>
        GetCalendar(calendarName).IsWorkDay(moment);

    /// <inheritdoc />
    public List<DateTime> GetPreviousWorkDays(string calendarName, DateTime moment, int count) =>
        GetCalendar(calendarName).GetPreviousWorkDays(moment);

    /// <inheritdoc />
    public List<DateTime> GetNextWorkDays(string calendarName, DateTime moment, int count) =>
        GetCalendar(calendarName).GetNextWorkDays(moment);

    /// <inheritdoc />
    public Tuple<DateTime, DateTime> GetCalendarPeriod(string calendarName, DateTime moment, int offset, string culture)
    {
        if (string.IsNullOrWhiteSpace(calendarName))
        {
            throw new ArgumentException(nameof(calendarName));
        }
        var period = new CalendarService(HttpClient).GetPeriodAsync(
            tenantId: TenantId,
            cultureName: culture,
            periodMoment: moment,
            offset: offset).Result ?? new();
        return new(period.Start, period.End);
    }

    /// <summary>
    /// Get calendar
    /// </summary>
    /// <param name="calendarName">Calendar name</param>
    /// <returns>Default calender on missing calendar</returns>
    private Model.Calendar GetCalendar(string calendarName)
    {
        var calendar = new CalendarService(HttpClient).GetAsync<Model.Calendar>(new(TenantId), calendarName).Result;
        if (calendar == null)
        {
            throw new ScriptException($"Unknown calendar {calendarName}");
        }
        return calendar;
    }

    #endregion

    #region Log and Task

    /// <summary>The log owner, the source identifier</summary>
    protected abstract string LogOwner { get; }

    /// <summary>The log owner type</summary>
    protected abstract string LogOwnerType { get; }

    /// <inheritdoc />
    public void AddLog(int level, string message, string error = null, string comment = null)
    {
        var log = new Model.Log
        {
            Level = (PayrollEngine.LogLevel)level,
            Message = message,
            Error = error,
            Comment = comment,
            User = UserIdentifier,
            Owner = LogOwner,
            OwnerType = LogOwnerType
        };
        var logService = new LogService(HttpClient);
        _ = logService.CreateAsync(new(TenantId), log).Result;
    }

    /// <inheritdoc />
    public void AddTask(string name, string instruction, DateTime scheduleDate, string category,
        Dictionary<string, object> attributes = null)
    {
        var task = new Task
        {
            ScheduledUserId = UserId,
            Name = name,
            Instruction = instruction,
            Scheduled = scheduleDate,
            Category = category,
            Attributes = attributes
        };
        var taskService = new TaskService(HttpClient);
        _ = taskService.CreateAsync(new(TenantId), task).Result;
    }

    #endregion

    #region Webhook

    /// <inheritdoc />
    public string InvokeWebhook(string requestOperation, string requestMessage = null)
    {
        throw new NotSupportedException("Webhooks are not supported through the client services.");
    }

    #endregion

}