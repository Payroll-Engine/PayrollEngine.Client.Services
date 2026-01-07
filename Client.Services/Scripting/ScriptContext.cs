namespace PayrollEngine.Client.Scripting;

/// <summary>
/// Script context
/// </summary>
public class ScriptContext
{
    /// <summary>
    /// Script calendar
    /// </summary>
    public ScriptCalendar Calendar { get; set; }

    /// <summary>
    /// Script namespace
    /// </summary>
    public string Namespace { get; set; }
}