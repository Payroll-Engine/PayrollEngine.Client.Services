using PayrollEngine.Client.Model;
using PayrollEngine.Client.Scripting.Function;

namespace PayrollEngine.Client.Scripting.Runtime.Api;

/// <summary>Runtime for the report start function</summary>
public class ReportStartRuntime : ReportRuntime, IReportStartRuntime
{
    /// <summary>Initializes a new instance of the <see cref="ReportStartRuntime"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="tenantId">The tenant id</param>
    /// <param name="userId">The user id</param>
    /// <param name="report">The report</param>
    /// <param name="reportRequest">The report request</param>
    public ReportStartRuntime(PayrollHttpClient httpClient, int tenantId,
        int userId, ReportSet report, ReportRequest reportRequest) :
        base(httpClient, tenantId, userId, report, reportRequest)
    {
        report.Queries ??= new();
    }

    /// <summary>The log owner type</summary>
    protected override string LogOwnerType => nameof(ReportStartFunction);

    /// <inheritdoc />
    public void SetParameter(string parameterName, string value) =>
        SetParameterInternal(parameterName, value);

    /// <inheritdoc />
    public bool HasQuery(string queryName) =>
        Report.Queries.ContainsKey(queryName);

    /// <inheritdoc />
    public string GetQuery(string queryName) =>
        Report.Queries[queryName];

    /// <inheritdoc />
    public void SetQuery(string queryName, string value) =>
        Report.Queries[queryName] = value;
}