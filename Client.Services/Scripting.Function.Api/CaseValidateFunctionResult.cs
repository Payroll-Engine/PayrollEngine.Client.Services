using System.Collections.Generic;
using PayrollEngine.Client.Model;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Result of the case validate function</summary>
public class CaseValidateFunctionResult : FunctionResultBase
{
    /// <summary>The validation result</summary>
    public bool? Valid { get; set; }

    /// <summary>The case set</summary>
    public CaseSet CaseSet { get; set; }

    /// <summary>The validation issues</summary>
    public List<CaseValidationIssue> Issues { get; set; }

    /// <summary>Returns a string that represents this instance</summary>
    public override string ToString() =>
        $"Valid={Valid} ({Issues.Count} issues)";
}