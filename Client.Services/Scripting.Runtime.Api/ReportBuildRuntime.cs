using System;
using System.Linq;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Scripting.Function;

namespace PayrollEngine.Client.Scripting.Runtime.Api;

/// <summary>Runtime for the report build function</summary>
public class ReportBuildRuntime : ReportRuntime, IReportBuildRuntime
{
    /// <summary>Initializes a new instance of the <see cref="ReportBuildRuntime"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="tenantId">The tenant id</param>
    /// <param name="userId">The user id</param>
    /// <param name="report">The report</param>
    /// <param name="reportRequest">The report request</param>
    public ReportBuildRuntime(PayrollHttpClient httpClient, int tenantId,
        int userId, ReportSet report, ReportRequest reportRequest) :
        base(httpClient, tenantId, userId, report, reportRequest)
    {
    }

    /// <summary>The log owner type</summary>
    protected override string LogOwnerType => nameof(ReportBuildFunction);

    /// <inheritdoc />
    public void SetParameter(string parameterName, string value) =>
        SetParameterInternal(parameterName, value);

    /// <inheritdoc />
    public void SetParameterAttribute(string parameterName, string attributeName, object value)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
        {
            throw new ArgumentException(nameof(parameterName));
        }
        if (string.IsNullOrWhiteSpace(attributeName))
        {
            throw new ArgumentException(nameof(attributeName));
        }

        // report parameter
        if (Report.Parameters == null)
        {
            throw new ArgumentException($"Invalid report parameter {parameterName}");
        }
        var reportParameter = Report.Parameters.FirstOrDefault(x => string.Equals(x.Name, parameterName));
        if (reportParameter == null)
        {
            throw new ArgumentException($"Unknown report parameter {parameterName}");
        }

        // add/change attribute
        if (value != null)
        {
            reportParameter.Attributes ??= new();
            reportParameter.Attributes[attributeName] = value;
        }
        else
        {
            // remove attribute
            if (reportParameter.Attributes != null && reportParameter.Attributes.ContainsKey(attributeName))
            {
                reportParameter.Attributes.Remove(attributeName);
            }
        }
    }

    /// <inheritdoc />
    public void SetParameterHidden(string parameterName, bool hidden)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
        {
            throw new ArgumentException(nameof(parameterName));
        }

        // report parameter
        if (Report.Parameters == null)
        {
            throw new ArgumentException($"Invalid report parameter {parameterName}");
        }
        var reportParameter = Report.Parameters.FirstOrDefault(x => string.Equals(x.Name, parameterName));
        if (reportParameter == null)
        {
            throw new ArgumentException($"Unknown report parameter {parameterName}");
        }
        reportParameter.Hidden = hidden;
    }
}