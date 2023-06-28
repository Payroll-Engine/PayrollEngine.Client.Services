using System;
using System.Collections.Generic;
using System.Globalization;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;

namespace PayrollEngine.Client.Scripting.Runtime.Api;

/// <summary>Runtime for the function</summary>
public abstract class Runtime : IRuntime
{
    /// <summary>The Payroll http client</summary>
    public PayrollHttpClient HttpClient { get; }

    /// <summary>The tenant service</summary>
    protected ITenantService TenantService { get; }

    /// <summary>The user service</summary>
    protected IUserService UserService { get; }

    /// <summary>Initializes a new instance of the <see cref="Runtime"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="tenantId">The tenant id</param>
    /// <param name="userId">The user id</param>
    protected Runtime(PayrollHttpClient httpClient, int tenantId, int userId)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        // tenant
        if (tenantId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tenantId));
        }
        TenantService = new TenantService(httpClient);
        Tenant = TenantService.GetAsync<Tenant>(new(), tenantId).Result;

        // culture by priority: Tenant > System
        Culture = Tenant.Culture ?? CultureInfo.CurrentCulture.Name;

        // user
        if (userId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(userId));
        }
        UserService = new UserService(httpClient);
        User = UserService.GetAsync<User>(new(tenantId), userId).Result;
    }

    #region Culture

    /// <inheritdoc />
    public virtual string Culture { get; }

    #endregion

    #region Tenant

    /// <summary>The tenant</summary>
    public Tenant Tenant { get; }

    /// <inheritdoc />
    public int TenantId => Tenant.Id;

    /// <inheritdoc />
    public string TenantIdentifier => Tenant.Identifier;

    /// <inheritdoc />
    public object GetTenantAttribute(string attributeName) =>
        Tenant.GetAttribute(attributeName);

    #endregion

    #region User

    /// <summary>The user</summary>
    public User User { get; }

    /// <inheritdoc />
    public int UserId => User.Id;

    /// <inheritdoc />
    public string UserIdentifier => User.Identifier;

    /// <inheritdoc />
    public object GetUserAttribute(string attributeName) =>
        User.GetAttribute(attributeName);

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

}