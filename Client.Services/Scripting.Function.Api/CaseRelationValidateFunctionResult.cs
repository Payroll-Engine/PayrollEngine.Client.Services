using System.Collections.Generic;
using PayrollEngine.Client.Model;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Result of the case relation validate function</summary>
public class CaseRelationValidateFunctionResult : FunctionResultBase
{
    /// <summary>The validation result</summary>
    public bool? Valid { get; set; }

    /// <summary>The source case set</summary>
    public CaseSet SourceCaseSet { get; set; }

    /// <summary>The target case set</summary>
    public CaseSet TargetCaseSet { get; set; }
        
    /// <summary>The validation issues</summary>
    public List<CaseValidationIssue> Issues { get; set; }

    /// <summary>Returns a string that represents this instance</summary>
    public override string ToString() =>
        $"Validate={Valid} ({Issues.Count} issues)";
}