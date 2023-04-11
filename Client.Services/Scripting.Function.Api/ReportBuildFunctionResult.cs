using PayrollEngine.Client.Model;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Result of the report build function</summary>
public class ReportBuildFunctionResult : FunctionResultBase
{
    /// <summary>The build report</summary>
    public ReportSet Report { get; set; }
}