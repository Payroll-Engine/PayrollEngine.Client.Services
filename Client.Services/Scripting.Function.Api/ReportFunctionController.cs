using System;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service.Api;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Function controller</summary>
/// <typeparam name="TFunc">The function</typeparam>
/// <typeparam name="TFuncAttribute">The function attribute</typeparam>
/// <typeparam name="TScriptAttribute">The script attribute</typeparam>
public abstract class ReportFunctionController<TFunc, TFuncAttribute, TScriptAttribute> : FunctionControllerBase<TFunc, TFuncAttribute, TScriptAttribute>
    where TFunc : ReportFunction
    where TFuncAttribute : ReportFunctionAttribute
    where TScriptAttribute : ReportScriptAttribute
{
    /// <summary>Initializes a new instance of the <see cref="ReportFunctionController{TFunc,TFuncAttribute,TScriptAttribute}"/> class</summary>
    /// <param name="httpClient">The Report http client</param>
    protected ReportFunctionController(PayrollHttpClient httpClient) :
        base(httpClient)
    {
    }

    private IRegulation regulation;
    /// <summary>The regulation</summary>
    protected IRegulation Regulation =>
        regulation ??= new RegulationService(HttpClient).GetAsync<Regulation>(new(Tenant.Id),
            Function.RegulationName).Result;

    private ITenant tenant;
    /// <summary>The tenant</summary>
    protected ITenant Tenant
    {
        get
        {
            if (tenant == null)
            {
                tenant = new TenantService(HttpClient).GetAsync<Tenant>(new(), Function.TenantIdentifier).Result;
                if (tenant == null)
                {
                    throw new ScriptException($"Unknown tenant {Function.TenantIdentifier}.");
                }
            }
            return tenant;
        }
    }

    private IUser user;
    /// <summary>The user</summary>
    protected IUser User
    {
        get
        {
            if (user == null)
            {
                user = new UserService(HttpClient).GetAsync<User>(new(Tenant.Id), Function.UserIdentifier).Result;
                if (user == null)
                {
                    throw new ScriptException($"Unknown user {Function.UserIdentifier}.");
                }
            }
            return user;
        }
    }

    /// <summary>Get report</summary>
    /// <param name="reportName">Name of the report</param>
    protected ReportSet GetReport(string reportName)
    {
        var report = GetReport(reportName, new() { UserId = User.Id });
        if (report == null)
        {
            throw new PayrollException($"Invalid report {reportName}.");
        }
        return report;
    }

    /// <summary>Build the report request using the function attributes</summary>
    /// <param name="reportName">Name of the report</param>
    public ReportRequest BuildReportRequest(string reportName)
    {
        if (string.IsNullOrWhiteSpace(reportName))
        {
            throw new ArgumentException(nameof(reportName));
        }

        // script attribute (method)
        var scriptAttribute = GetScriptAttribute(reportName);
        var request = new ReportRequest
        {
            UserId = User.Id,
            Culture = scriptAttribute.Culture,
            Parameters = scriptAttribute.Parameters
        };

        // function attribute (class)
        request.Parameters ??= new();
        request.Parameters.Add("TenantId", Function.TenantIdentifier);
        request.Parameters.Add("UserId", Function.UserIdentifier);
        request.Parameters.Add("RegulationId", Function.RegulationName);
        return request;
    }

    /// <summary>Get report</summary>
    /// <param name="reportName">Name of the report</param>
    /// <param name="reportRequest">The report request</param>
    protected ReportSet GetReport(string reportName, ReportRequest reportRequest) =>
        new ReportSetService(HttpClient).GetAsync<ReportSet>(new(Tenant.Id, Regulation.Id),
            reportName, reportRequest).Result;
}