using System;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Scripting.Runtime.Api;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Report build function controller</summary>
public class ReportBuildController<TFunc> : ReportFunctionController<TFunc, ReportBuildFunctionAttribute, ReportBuildScriptAttribute>
    where TFunc : ReportBuildFunction
{
    /// <summary>Initializes a new instance of the <see cref="ReportBuildController{T}"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    public ReportBuildController(PayrollHttpClient httpClient) :
        base(httpClient)
    {
    }

    /// <summary>Report build</summary>
    /// <param name="reportName">Name of the report</param>
    public ReportBuildFunctionResult Execute(string reportName) =>
        Execute(reportName, BuildReportRequest(reportName));

    /// <summary>Report build</summary>
    /// <param name="report">The report</param>
    public ReportBuildFunctionResult Execute(ReportSet report) =>
        Execute(report, BuildReportRequest(report.Name));

    /// <summary>Report build</summary>
    /// <param name="reportName">Name of the report</param>
    /// <param name="reportRequest">The report request</param>
    public ReportBuildFunctionResult Execute(string reportName, ReportRequest reportRequest) =>
        Execute(GetReport(reportName, reportRequest), reportRequest);

    /// <summary>Report build</summary>
    /// <param name="report">The report</param>
    /// <param name="reportRequest">The report request</param>
    public ReportBuildFunctionResult Execute(ReportSet report, ReportRequest reportRequest)
    {
        if (report == null)
        {
            throw new ArgumentNullException(nameof(report));
        }
        if (reportRequest == null)
        {
            throw new ArgumentNullException(nameof(reportRequest));
        }

        var method = GetScriptMethod(report.Name);

        // runtime and function
        var runtime = new ReportBuildRuntime(HttpClient, Tenant.Id, User.Id, report, reportRequest);
        var function = Activator.CreateInstance(typeof(TFunc), runtime);
        method.Invoke(function, null);

        // result
        var result = new ReportBuildFunctionResult
        {
            Report = report
        };
        return result;
    }
}