using System.Collections.Generic;
using System.Data;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Result of the report start function</summary>
public class ReportStartFunctionResult : FunctionResultBase
{
    /// <summary>The report queries</summary>
    public IDictionary<string, string> Queries { get; set; }

    /// <summary>The queries results</summary>
    public DataSet DataSet { get; set; }

    /// <summary>Returns a string that represents this instance</summary>
    public override string ToString() =>
        $"ReportStart={Queries?.Count} queries";
}