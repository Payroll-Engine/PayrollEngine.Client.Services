using System;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service.Api;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Function controller</summary>
/// <typeparam name="TFunc">The function</typeparam>
/// <typeparam name="TFuncAttribute">The function attribute</typeparam>
/// <typeparam name="TScriptAttribute">The script attribute</typeparam>
public abstract class PayrollFunctionController<TFunc, TFuncAttribute, TScriptAttribute> : FunctionControllerBase<TFunc, TFuncAttribute, TScriptAttribute>
    where TFunc : PayrollFunction
    where TFuncAttribute : PayrollAttribute
    where TScriptAttribute : ScriptAttribute
{
    /// <summary>The scripting configuration</summary>
    public ScriptingConfiguration Configuration { get; }

    /// <summary>Initializes a new instance of the <see cref="PayrollFunctionController{TFunc,TFuncAttribute,TScriptAttribute}"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="configuration">The scripting configuration</param>
    protected PayrollFunctionController(PayrollHttpClient httpClient, ScriptingConfiguration configuration) :
        base(httpClient)
    {
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    private ITenant tenant;
    /// <summary>The tenant</summary>
    protected ITenant Tenant
    {
        get
        {
            if (tenant == null)
            {
                tenant = new TenantService(HttpClient).GetAsync<Tenant>(new(), Function.TenantIdentifier).Result;
                if (tenant == null)
                {
                    throw new ScriptException($"Unknown tenant {Function.TenantIdentifier}");
                }
            }
            return tenant;
        }
    }

    private IUser user;
    /// <summary>The user</summary>
    protected IUser User
    {
        get
        {
            if (user == null)
            {
                user = new UserService(HttpClient).GetAsync<User>(new(Tenant.Id), Function.UserIdentifier).Result;
                if (user == null)
                {
                    throw new ScriptException($"Unknown user {Function.UserIdentifier}");
                }
            }
            return user;
        }
    }

    private IPayroll payroll;
    /// <summary>The payroll</summary>
    protected IPayroll Payroll
    {
        get
        {
            if (payroll == null)
            {
                payroll = new PayrollService(HttpClient).GetAsync<Payroll>(
                    new(Tenant.Id), Function.PayrollName).Result;
                if (payroll == null)
                {
                    throw new ScriptException($"Unknown payroll {Function.PayrollName}");
                }
            }
            return payroll;
        }
    }

    private IEmployee employee;
    /// <summary>The employee</summary>
    protected IEmployee Employee
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Function.EmployeeIdentifier))
            {
                return null;
            }
            if (employee == null)
            {
                employee = new EmployeeService(HttpClient).GetAsync<Employee>(
                    new(Tenant.Id), Function.EmployeeIdentifier).Result;
                if (employee == null)
                {
                    throw new ScriptException($"Unknown employee {Function.EmployeeIdentifier}");
                }
            }
            return employee;
        }
    }

    /// <summary>New scripting calendar using the tenant calendar</summary>
    protected ScriptingCalendar NewScriptingCalendar()
    {
        var calendar = Tenant.Calendar;
        if (calendar == null)
        {
            throw new ScriptException("Tenant calendar is mandatory");
        }
        return NewScriptingCalendar(calendar);
    }

    /// <summary>New scripting calendar</summary>
    /// <param name="calendarConfiguration">The calendar configuration</param>
    protected ScriptingCalendar NewScriptingCalendar(CalendarConfiguration calendarConfiguration)
    {
        var payrollCalendar = new PayrollCalendar(calendarConfiguration, Tenant.Id, User?.Id);
        var functionCalendar = new ScriptingCalendar(Configuration, payrollCalendar);
        return functionCalendar;
    }
}