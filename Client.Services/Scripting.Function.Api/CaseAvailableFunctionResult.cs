using PayrollEngine.Client.Model;

namespace PayrollEngine.Client.Scripting.Function.Api;

/// <summary>Result of the case available function</summary>
public class CaseAvailableFunctionResult : FunctionResultBase
{
    /// <summary>The available result</summary>
    public bool? Available { get; set; }

    /// <summary>The case</summary>
    public Case Case { get; set; }

    /// <summary>Returns a string that represents this instance</summary>
    public override string ToString() =>
        $"Available={Available}";
}