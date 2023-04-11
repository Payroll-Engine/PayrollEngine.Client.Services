using System.Data;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Result of the report start function</summary>
public class ReportEndFunctionResult : FunctionResultBase
{
    /// <summary>The report queries</summary>
    public DataSet DataSet { get; set; }

    /// <summary>Returns a string that represents this instance</summary>
    public override string ToString() =>
        $"ReportEnd={DataSet?.Tables.Count} tables";
}