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
    public ScriptConfiguration Configuration { get; }

    /// <summary>Initializes a new instance of the <see cref="PayrollFunctionController{TFunc,TFuncAttribute,TScriptAttribute}"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="configuration">The scripting configuration</param>
    protected PayrollFunctionController(PayrollHttpClient httpClient, ScriptConfiguration configuration) :
        base(httpClient)
    {
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>The tenant</summary>
    protected ITenant Tenant
    {
        get
        {
            if (field == null)
            {
                field = new TenantService(HttpClient).GetAsync<Tenant>(new(), Function.TenantIdentifier).Result;
                if (field == null)
                {
                    throw new ScriptException($"Unknown tenant {Function.TenantIdentifier}.");
                }
            }
            return field;
        }
    }

    /// <summary>The user</summary>
    protected IUser User
    {
        get
        {
            if (field == null)
            {
                field = new UserService(HttpClient).GetAsync<User>(new(Tenant.Id), Function.UserIdentifier).Result;
                if (field == null)
                {
                    throw new ScriptException($"Unknown user {Function.UserIdentifier}.");
                }
            }
            return field;
        }
    }

    /// <summary>The payroll</summary>
    protected IPayroll Payroll
    {
        get
        {
            if (field == null)
            {
                field = new PayrollService(HttpClient).GetAsync<Payroll>(
                    new(Tenant.Id), Function.PayrollName).Result;
                if (field == null)
                {
                    throw new ScriptException($"Unknown payroll {Function.PayrollName}.");
                }
            }
            return field;
        }
    }

    /// <summary>The employee</summary>
    protected IEmployee Employee
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Function.EmployeeIdentifier))
            {
                return null;
            }
            if (field == null)
            {
                field = new EmployeeService(HttpClient).GetAsync<Employee>(
                    new(Tenant.Id), Function.EmployeeIdentifier).Result;
                if (field == null)
                {
                    throw new ScriptException($"Unknown employee {Function.EmployeeIdentifier}.");
                }
            }
            return field;
        }
    }

    /// <summary>New scripting context</summary>
    protected ScriptContext NewScriptingContext() =>
        new()
        {
            Calendar = NewScriptingCalendar()
        };

    /// <summary>New scripting calendar using the tenant calendar</summary>
    protected ScriptCalendar NewScriptingCalendar()
    {
        var calendarName = Tenant.Calendar;
        var functionCalendar = new ScriptCalendar(Configuration, calendarName);
        return functionCalendar;
    }
}