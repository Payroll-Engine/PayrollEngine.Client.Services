using System;
using System.Data;
using System.IO;
using System.Text.Json;
using PayrollEngine.Client.Model;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Report end function invoker</summary>
public class ReportEndFunctionInvoker<TFunction> : FunctionInvokerBase
    where TFunction : ReportEndFunction
{
    /// <summary>Query input file name</summary>
    public string QueryFileName { get; }

    /// <summary>Result output file name</summary>
    public string ResultFileName { get; }

    /// <summary>Invoker constructor</summary>
    /// <param name="httpClient">The Payroll http configuration</param>
    /// <param name="configuration">The scripting configuration</param>
    /// <param name="queryFileName">Query input file name</param>
    /// <param name="resultFileName">Result output file name</param>
    public ReportEndFunctionInvoker(PayrollHttpClient httpClient, ScriptConfiguration configuration,
        string queryFileName = null, string resultFileName = null) :
        base(httpClient, configuration)
    {
        QueryFileName = queryFileName;
        ResultFileName = resultFileName;
    }

    /// <summary>Build the report request using the function attributes</summary>
    /// <param name="reportName">Name of the report</param>
    public ReportRequest BuildReportRequest(string reportName) =>
        new ReportEndController<TFunction>(HttpClient).BuildReportRequest(reportName);

    /// <summary>Report end</summary>
    /// <param name="reportName">Name of the report</param>
    /// <param name="dataSet">The report data set</param>
    /// <returns>The function result</returns>
    public ReportEndFunctionResult End(string reportName, DataSet dataSet = null)
    {
        if (string.IsNullOrWhiteSpace(reportName))
        {
            throw new ArgumentException(nameof(reportName));
        }
        dataSet ??= ReadQueryDataSet(QueryFileName);
        var controller = new ReportEndController<TFunction>(HttpClient);
        var result = controller.Execute(reportName, dataSet);
        WriteResultDataSet(result.DataSet, ResultFileName);
        return result;
    }

    /// <summary>Report end</summary>
    /// <param name="reportName">Name of the report</param>
    /// <param name="reportRequest">The report request</param>
    /// <param name="dataSet">The report data set</param>
    /// <returns>The function result</returns>
    public ReportEndFunctionResult End(string reportName, ReportRequest reportRequest,
        DataSet dataSet = null)
    {
        if (string.IsNullOrWhiteSpace(reportName))
        {
            throw new ArgumentException(nameof(reportName));
        }
        if (reportRequest == null)
        {
            throw new ArgumentNullException(nameof(reportRequest));
        }
        dataSet ??= ReadQueryDataSet(QueryFileName);
        var controller = new ReportEndController<TFunction>(HttpClient);
        var result = controller.Execute(reportName, reportRequest, dataSet);
        WriteResultDataSet(result.DataSet, ResultFileName);
        return result;
    }

    /// <summary>Report end</summary>
    /// <param name="report">The report</param>
    /// <param name="dataSet">The report data set</param>
    /// <returns>The function result</returns>
    public ReportEndFunctionResult End(ReportSet report, DataSet dataSet = null)
    {
        if (report == null)
        {
            throw new ArgumentNullException(nameof(report));
        }
        dataSet ??= ReadQueryDataSet(QueryFileName);
        var controller = new ReportEndController<TFunction>(HttpClient);
        var result = controller.Execute(report, dataSet);
        WriteResultDataSet(result.DataSet, ResultFileName);
        return result;
    }

    /// <summary>Report end</summary>
    /// <param name="report">The report</param>
    /// <param name="reportRequest">The report request</param>
    /// <param name="dataSet">The report data set</param>
    /// <returns>The function result</returns>
    public ReportEndFunctionResult End(ReportSet report, ReportRequest reportRequest,
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
        dataSet ??= ReadQueryDataSet(QueryFileName);
        var controller = new ReportEndController<TFunction>(HttpClient);
        var result = controller.Execute(report, reportRequest, dataSet);
        WriteResultDataSet(result.DataSet, ResultFileName);
        return result;
    }

    private static DataSet ReadQueryDataSet(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
        {
            return null;
        }

        // read json from file
        var json = File.ReadAllText(fileName);
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        // deserialize data set
        // deserialize payroll data (security issue with System.Data.DataSet)
        var payrollDataSet = JsonSerializer.Deserialize<Data.DataSet>(json);
        var dataSet = Data.DataSetExtensions.ToSystemDataSet(payrollDataSet);
        return dataSet;
    }

    private static void WriteResultDataSet(DataSet dataSet, string fileName)
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
}