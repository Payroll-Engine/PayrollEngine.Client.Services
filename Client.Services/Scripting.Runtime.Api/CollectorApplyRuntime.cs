using PayrollEngine.Client.Scripting.Function;

namespace PayrollEngine.Client.Scripting.Runtime.Api;

/// <summary>Runtime for the collector apply function</summary>
public class CollectorApplyRuntime : CollectorRuntimeBase, ICollectorApplyRuntime
{
    /// <summary>Initializes a new instance of the <see cref="CollectorApplyRuntime"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="scriptContext">The script context</param>
    /// <param name="tenantId">The tenant id</param>
    /// <param name="userId">The user id</param>
    /// <param name="payrollId">The payroll id</param>
    /// <param name="employeeId">The employee id</param>
    public CollectorApplyRuntime(PayrollHttpClient httpClient, ScriptContext scriptContext, int tenantId,
        int userId, int payrollId, int? employeeId = null) :
        base(httpClient, scriptContext, tenantId, userId, payrollId, employeeId)
    {
    }

    /// <summary>The log owner type</summary>
    protected override string LogOwnerType => nameof(CollectorApplyFunction);

    /// <inheritdoc />
    public decimal WageTypeNumber =>
        // implementation
        0;

    /// <inheritdoc />
    public string WageTypeName =>
        // implementation
        null;

    /// <inheritdoc />
    public decimal WageTypeValue =>
        // implementation
        0;

    /// <inheritdoc />
    public string[] GetApplyActions() => null;
}