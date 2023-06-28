using System;
using System.Data;
using System.Globalization;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Scripting.Runtime.Api;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Report start function controller</summary>
public class ReportStartController<TFunc> : ReportFunctionController<TFunc, ReportStartFunctionAttribute, ReportStartScriptAttribute>
    where TFunc : ReportStartFunction
{
    /// <summary>Store query results to the <see cref="ReportStartFunctionResult.DataSet"/></summary>
    public bool ExecuteQueries { get; set; }

    /// <summary>Initializes a new instance of the <see cref="ReportStartController{T}"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    public ReportStartController(PayrollHttpClient httpClient) :
        base(httpClient)
    {
    }

    /// <summary>Report start</summary>
    /// <param name="reportName">Name of the report</param>
    public ReportStartFunctionResult Execute(string reportName) =>
        Execute(reportName, BuildReportRequest(reportName));

    /// <summary>Report start</summary>
    /// <param name="report">The report</param>
    public ReportStartFunctionResult Execute(ReportSet report) =>
        Execute(report, BuildReportRequest(report.Name));

    /// <summary>Report start</summary>
    /// <param name="reportName">Name of the report</param>
    /// <param name="reportRequest">The report request</param>
    public ReportStartFunctionResult Execute(string reportName, ReportRequest reportRequest) =>
        Execute(GetReport(reportName, reportRequest), reportRequest);

    /// <summary>Report start</summary>
    /// <param name="report">The report</param>
    /// <param name="reportRequest">The report request</param>
    public ReportStartFunctionResult Execute(ReportSet report, ReportRequest reportRequest)
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
        var runtime = new ReportStartRuntime(HttpClient, Tenant.Id, User.Id, report, reportRequest);
        var function = Activator.CreateInstance(typeof(TFunc), runtime);
        method.Invoke(function, null);

        // result
        var result = new ReportStartFunctionResult
        {
            Queries = report.Queries
        };

        // query result data set
        if (ExecuteQueries && report.Queries != null)
        {
            result.DataSet = BuildQueryDataSet(report, reportRequest, runtime);
        }

        return result;
    }

    private DataSet BuildQueryDataSet(ReportSet report, ReportRequest reportRequest, ReportStartRuntime runtime)
    {
        // ensure parameter tenant id
        var parameters = reportRequest.Parameters;
        parameters ??= new();
        var tenantIdKey = $"{nameof(Tenant)}{nameof(Tenant.Id)}";
        parameters.TryAdd(tenantIdKey, Tenant.Id.ToString());

        // result data set
        DataSet dataSet = new(report.Name)
        {
            Locale = CultureInfo.InvariantCulture
        };

        // execute all queries
        foreach (var query in report.Queries)
        {
            var dataTable = runtime.ExecuteQuery(query.Key, query.Value, reportRequest.Culture, parameters);
            if (dataTable != null)
            {
                dataSet.Tables.Add(dataTable);
            }
        }

        return dataSet;
    }
}