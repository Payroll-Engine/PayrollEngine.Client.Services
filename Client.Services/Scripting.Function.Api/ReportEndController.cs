using System;
using System.Data;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Scripting.Runtime.Api;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Report end function controller</summary>
public class ReportEndController<TFunc> : ReportFunctionController<TFunc, ReportEndFunctionAttribute, ReportEndScriptAttribute>
    where TFunc : ReportEndFunction
{
    /// <summary>Initializes a new instance of the <see cref="ReportEndController{T}"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    public ReportEndController(PayrollHttpClient httpClient) :
        base(httpClient)
    {
    }

    /// <summary>Report end</summary>
    /// <param name="reportName">Name of the report</param>
    /// <param name="dataSet">The report data set</param>
    public ReportEndFunctionResult Execute(string reportName, DataSet dataSet = null) =>
        Execute(reportName, BuildReportRequest(reportName), dataSet);

    /// <summary>Report end</summary>
    /// <param name="report">The report</param>
    /// <param name="dataSet">The report data set</param>
    public ReportEndFunctionResult Execute(ReportSet report, DataSet dataSet = null) =>
        Execute(report, BuildReportRequest(report.Name), dataSet);

    /// <summary>Report end</summary>
    /// <param name="reportName">Name of the report</param>
    /// <param name="reportRequest">The report request</param>
    /// <param name="dataSet">The report data set</param>
    public ReportEndFunctionResult Execute(string reportName, ReportRequest reportRequest,
        DataSet dataSet = null) =>
        Execute(GetReport(reportName, reportRequest), reportRequest, dataSet);

    /// <summary>Report end</summary>
    /// <param name="report">The report</param>
    /// <param name="reportRequest">The report request</param>
    /// <param name="dataSet">The report data set</param>
    public ReportEndFunctionResult Execute(ReportSet report, ReportRequest reportRequest,
        DataSet dataSet = null)
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
        var runtime = new ReportEndRuntime(HttpClient, Tenant.Id, User.Id, report, reportRequest, dataSet);
        var function = Activator.CreateInstance(typeof(TFunc), runtime);
        method.Invoke(function, null);

        // result
        var result = new ReportEndFunctionResult
        {
            DataSet = runtime.DataSet
        };
        return result;
    }
}