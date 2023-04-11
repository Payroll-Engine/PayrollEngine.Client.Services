using PayrollEngine.Client.Model;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Result of a function</summary>
public abstract class FunctionResultBase
{
    /// <summary>The calendar</summary>
    public ScriptingCalendar Calendar { get; set; }

    /// <summary>The tenant</summary>
    public ITenant Tenant { get; set; }

    /// <summary>The employee</summary>
    public IEmployee Employee { get; set; }

    /// <summary>The payroll</summary>
    public IPayroll Payroll { get; set; }
}