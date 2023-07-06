using System;
using PayrollEngine.Client.Model;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Report build function invoker</summary>
public class ReportBuildFunctionInvoker<TFunction> : FunctionInvokerBase
    where TFunction : ReportBuildFunction
{
    /// <summary>Invoker constructor</summary>
    /// <param name="httpClient">The Payroll http configuration</param>
    /// <param name="configuration">The scripting configuration</param>
    public ReportBuildFunctionInvoker(PayrollHttpClient httpClient, ScriptConfiguration configuration) :
        base(httpClient, configuration)
    {
    }

    /// <summary>Build the report request using the function attributes</summary>
    /// <param name="reportName">Name of the report</param>
    public ReportRequest BuildReportRequest(string reportName) =>
        new ReportBuildController<TFunction>(HttpClient).BuildReportRequest(reportName);

    /// <summary>Report build</summary>
    /// <param name="reportName">Name of the report</param>
    /// <returns>The function result</returns>
    public ReportBuildFunctionResult Build(string reportName)
    {
        if (string.IsNullOrWhiteSpace(reportName))
        {
            throw new ArgumentException(nameof(reportName));
        }
        var controller = new ReportBuildController<TFunction>(HttpClient);
        return controller.Execute(reportName);
    }

    /// <summary>Report build</summary>
    /// <param name="reportName">Name of the report</param>
    /// <param name="reportRequest">The report request</param>
    /// <returns>The function result</returns>
    public ReportBuildFunctionResult Build(string reportName, ReportRequest reportRequest)
    {
        if (reportRequest == null)
        {
            throw new ArgumentNullException(nameof(reportRequest));
        }
        if (string.IsNullOrWhiteSpace(reportName))
        {
            throw new ArgumentException(nameof(reportName));
        }
        var controller = new ReportBuildController<TFunction>(HttpClient);
        return controller.Execute(reportName, reportRequest);
    }

    /// <summary>Report build</summary>
    /// <param name="report">The report</param>
    public ReportBuildFunctionResult Build(ReportSet report)
    {
        if (report == null)
        {
            throw new ArgumentNullException(nameof(report));
        }
        var controller = new ReportBuildController<TFunction>(HttpClient);
        return controller.Execute(report);
    }

    /// <summary>Report build</summary>
    /// <param name="report">The report</param>
    /// <param name="reportRequest">The report request</param>
    public ReportBuildFunctionResult Build(ReportSet report, ReportRequest reportRequest)
    {
        if (report == null)
        {
            throw new ArgumentNullException(nameof(report));
        }
        if (reportRequest == null)
        {
            throw new ArgumentNullException(nameof(reportRequest));
        }
        var controller = new ReportBuildController<TFunction>(HttpClient);
        return controller.Execute(report, reportRequest);
    }
}