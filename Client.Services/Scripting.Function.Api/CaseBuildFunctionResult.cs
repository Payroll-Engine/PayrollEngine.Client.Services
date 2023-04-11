using PayrollEngine.Client.Model;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Result of the case build function</summary>
public class CaseBuildFunctionResult : FunctionResultBase
{
    /// <summary>The build result</summary>
    public bool? Build { get; set; }

    /// <summary>The case set</summary>
    public CaseSet CaseSet { get; set; }

    /// <summary>Returns a string that represents this instance</summary>
    public override string ToString() =>
        $"Build={Build}";
}