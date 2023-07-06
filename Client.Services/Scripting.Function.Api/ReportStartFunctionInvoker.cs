using System;
using System.Data;
using System.IO;
using System.Text.Json;
using PayrollEngine.Client.Model;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Report start function invoker</summary>
public class ReportStartFunctionInvoker<TFunction> : FunctionInvokerBase
    where TFunction : ReportStartFunction
{
    /// <summary>Query output file name</summary>
    public string QueryFileName { get; }

    /// <summary>Invoker constructor</summary>
    /// <param name="httpClient">The Payroll http configuration</param>
    /// <param name="configuration">The scripting configuration</param>
    /// <param name="queryFileName">Query output file name</param>
    public ReportStartFunctionInvoker(PayrollHttpClient httpClient, ScriptConfiguration configuration,
        string queryFileName = null) :
        base(httpClient, configuration)
    {
        QueryFileName = queryFileName;
    }
    
    /// <summary>Build the report request using the function attributes</summary>
    /// <param name="reportName">Name of the report</param>
    public ReportRequest BuildReportRequest(string reportName) =>
        new ReportStartController<TFunction>(HttpClient).BuildReportRequest(reportName);

    /// <summary>Report start</summary>
    /// <param name="reportName">Name of the report</param>
    /// <returns>The function result</returns>
    public ReportStartFunctionResult Start(string reportName = null)
    {
        if (string.IsNullOrWhiteSpace(reportName))
        {
            throw new ArgumentException(nameof(reportName));
        }
        var result = NewReportStartController(QueryFileName).Execute(reportName);
        WriteQueryDataSet(result.DataSet, QueryFileName);
        return result;
    }

    /// <summary>Report start</summary>
    /// <param name="reportName">Name of the report</param>
    /// <param name="reportRequest">The report request</param>
    /// <returns>The function result</returns>
    public ReportStartFunctionResult Start(string reportName, ReportRequest reportRequest)
    {
        if (string.IsNullOrWhiteSpace(reportName))
        {
            throw new ArgumentException(nameof(reportName));
        }
        if (reportRequest == null)
        {
            throw new ArgumentNullException(nameof(reportRequest));
        }
        var result = NewReportStartController(QueryFileName).Execute(reportName, reportRequest);
        WriteQueryDataSet(result.DataSet, QueryFileName);
        return result;
    }

    /// <summary>Report start</summary>
    /// <param name="report">The report</param>
    public ReportStartFunctionResult Start(ReportSet report)
    {
        if (report == null)
        {
            throw new ArgumentNullException(nameof(report));
        }
        var result = NewReportStartController(QueryFileName).Execute(report);
        WriteQueryDataSet(result.DataSet, QueryFileName);
        return result;
    }

    /// <summary>Report start</summary>
    /// <param name="report">The report</param>
    /// <param name="reportRequest">The report request</param>
    public ReportStartFunctionResult Start(ReportSet report, ReportRequest reportRequest)
    {
        if (report == null)
        {
            throw new ArgumentNullException(nameof(report));
        }
        if (reportRequest == null)
        {
            throw new ArgumentNullException(nameof(reportRequest));
        }
        var result = NewReportStartController(QueryFileName).Execute(report, reportRequest);
        WriteQueryDataSet(result.DataSet, QueryFileName);
        return result;
    }

    private static void WriteQueryDataSet(DataSet dataSet, string fileName)
    {
        if (dataSet == null || string.IsNullOrWhiteSpace(fileName))
        {
            return;
        }

        // file cleanup
        if (File.Exists(fileName))
        {
            File.Delete(fileName);
        }

        // serialize payroll data set fo file (security issue with System.Data.DataSet)
        var payrollDataSet = Data.DataSetExtensions.ToPayrollDataSet(dataSet);
        var json = JsonSerializer.Serialize(payrollDataSet, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(fileName, json);
    }

    private ReportStartController<TFunction> NewReportStartController(string fileName)
    {
        var controller = new ReportStartController<TFunction>(HttpClient)
        {
            ExecuteQueries = !string.IsNullOrWhiteSpace(fileName)
        };
        return controller;
    }
}