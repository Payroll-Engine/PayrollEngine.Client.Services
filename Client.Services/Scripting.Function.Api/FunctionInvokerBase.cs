using System;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Function invoker base</summary>
public class FunctionInvokerBase
{
    /// <summary>The Payroll http client</summary>
    public PayrollHttpClient HttpClient { get; }

    /// <summary>The scripting configuration</summary>
    public ScriptConfiguration Configuration { get; }

    /// <summary>Invoker constructor</summary>
    /// <param name="httpClient">The Payroll http configuration</param>
    /// <param name="configuration">The scripting configuration</param>
    protected FunctionInvokerBase(PayrollHttpClient httpClient, ScriptConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        HttpClient = httpClient;
        ArgumentNullException.ThrowIfNull(configuration);
        Configuration = configuration;
    }
}
