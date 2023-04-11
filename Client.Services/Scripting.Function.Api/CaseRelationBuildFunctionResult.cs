using PayrollEngine.Client.Model;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Result of the case relation build function</summary>
public class CaseRelationBuildFunctionResult : FunctionResultBase
{
    /// <summary>The build result</summary>
    public bool? Build { get; set; }

    /// <summary>The source case set</summary>
    public CaseSet SourceCaseSet { get; set; }

    /// <summary>The target case set</summary>
    public CaseSet TargetCaseSet { get; set; }

    /// <summary>Returns a string that represents this instance</summary>
    public override string ToString() =>
        $"Map={Build}";
}