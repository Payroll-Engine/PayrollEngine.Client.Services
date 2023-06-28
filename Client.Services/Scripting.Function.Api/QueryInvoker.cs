using System;
using System.Data;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service.Api;
using Tasks = System.Threading.Tasks;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Query invoker</summary>
public class QueryInvoker
{
    /// <summary>The Payroll http client</summary>
    public PayrollHttpClient HttpClient { get; }

    /// <summary>Invoker constructor</summary>
    /// <param name="httpClient">The Payroll http configuration</param>
    public QueryInvoker(PayrollHttpClient httpClient)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>Get report</summary>
    /// <param name="tenantIdentifier">The tenant identifier</param>
    /// <param name="regulationName">The regulation name</param>
    /// <param name="reportName">Name of the report</param>
    /// <param name="reportRequest">The report request</param>
    public async Tasks.Task<DataSet> InvokeQueriesAsync(string tenantIdentifier,
        string regulationName, string reportName, ReportRequest reportRequest)
    {
        if (string.IsNullOrWhiteSpace(tenantIdentifier))
        {
            throw new ArgumentException(nameof(tenantIdentifier));
        }
        if (string.IsNullOrWhiteSpace(regulationName))
        {
            throw new ArgumentException(nameof(regulationName));
        }

        // tenant
        var tenant = await new TenantService(HttpClient).GetAsync<Tenant>(new(), tenantIdentifier);
        if (tenant == null)
        {
            throw new ArgumentException(nameof(tenantIdentifier));
        }

        // regulation
        var regulation = await new RegulationService(HttpClient).
            GetAsync<Regulation>(new(tenant.Id), regulationName);
        if (regulation == null)
        {
            throw new ArgumentException(nameof(regulationName));
        }

        return await InvokeQueriesAsync(tenant.Id, regulation.Id, reportName, reportRequest);
    }

    /// <summary>Get report</summary>
    /// <param name="reportName">Name of the report</param>
    /// <param name="reportRequest">The report request, including the tenant and regulation parameters</param>
    public async Tasks.Task<DataSet> InvokeQueriesAsync(string reportName, ReportRequest reportRequest)
    {
        if (reportRequest == null)
        {
            throw new ArgumentNullException(nameof(reportRequest));
        }
        if (reportRequest.Parameters == null)
        {
            throw new ArgumentNullException(nameof(reportRequest.Parameters));
        }

        // tenant
        const string tenantKey = $"{nameof(Tenant)}{nameof(Tenant.Id)}";
        var tenantParameter = reportRequest.Parameters[tenantKey];
        if (!int.TryParse(tenantParameter, out var tenantId))
        {
            var tenant = await new TenantService(HttpClient).GetAsync<Tenant>(new(), tenantParameter);
            if (tenant == null)
            {
                throw new ScriptException($"Unknown tenant {tenantParameter}");
            }
            tenantId = tenant.Id;
        }
        reportRequest.Parameters[tenantKey] = tenantId.ToString();

        // regulation
        const string regulationKey = $"{nameof(Regulation)}{nameof(Regulation.Id)}";
        var regulationParameter = reportRequest.Parameters[regulationKey];
        if (!int.TryParse(regulationParameter, out var regulationId))
        {
            var regulation = await new RegulationService(HttpClient).
                GetAsync<Regulation>(new(tenantId), regulationParameter);
            if (regulation == null)
            {
                throw new ScriptException($"Unknown regulation {regulationParameter}");
            }
            regulationId = regulation.Id;
        }
        reportRequest.Parameters[regulationKey] = regulationId.ToString();

        return await InvokeQueriesAsync(tenantId, regulationId, reportName, reportRequest);
    }

    /// <summary>Get report</summary>
    /// <param name="tenantId">The tenant id</param>
    /// <param name="regulationId">The regulation id</param>
    /// <param name="reportName">Name of the report</param>
    /// <param name="reportRequest">The report request</param>
    public async Tasks.Task<DataSet> InvokeQueriesAsync(int tenantId, int regulationId, string reportName,
        ReportRequest reportRequest)
    {
        if (tenantId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tenantId));
        }
        if (regulationId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(regulationId));
        }
        if (string.IsNullOrWhiteSpace(reportName))
        {
            throw new ArgumentException(nameof(reportName));
        }
        if (reportRequest == null)
        {
            throw new ArgumentNullException(nameof(reportRequest));
        }
        if (reportRequest.UserId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(reportRequest));
        }

        // result data set
        Data.DataSet dataSet = new(reportName);
        dataSet.Tables ??= new();

        // report
        var report = await new ReportSetService(HttpClient).
            GetAsync<ReportSet>(new(tenantId, regulationId), reportName, reportRequest);
        if (report == null)
        {
            throw new ArgumentException(nameof(reportName));
        }

        // report queries
        if (report.Queries != null)
        {
            var service = new TenantService(HttpClient);
            foreach (var query in report.Queries)
            {
                // execute report query
                var dataTable = await service.ExecuteReportQueryAsync(tenantId, query.Value, reportRequest.Culture,
                    reportRequest.Parameters);
                if (dataTable != null)
                {
                    dataSet.Tables.Add(dataTable);
                }
            }
        }

        // convert to .NET data set
        return Data.DataSetExtensions.ToSystemDataSet(dataSet);
    }
}
