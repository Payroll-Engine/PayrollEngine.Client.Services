using System;
using System.Text;

namespace PayrollEngine.Client.Scripting;

/// <summary>Script calendar</summary>
public class ScriptingCalendar
{
    /// <summary>The scripting configuration</summary>
    public ScriptingConfiguration Configuration { get; }

    /// <summary>The payroll calendar</summary>
    public PayrollCalendar PayrollCalendar { get; }

    /// <summary>The language</summary>
    public Language Language { get; }

    /// <summary>The regulation date</summary>
    public DateTime PeriodDate => Configuration.PeriodDate;

    /// <summary>The evaluation date</summary>
    public DateTime EvaluationDate => Configuration.EvaluationDate;

    /// <summary>The regulation date</summary>
    public DateTime RegulationDate => Configuration.RegulationDate;

    /// <summary>Initializes a new instance of the <see cref="ScriptingCalendar"/> class</summary>
    /// <param name="configuration">The scripting configuration</param>
    /// <param name="payrollCalendar">The payroll calendar</param>
    /// <param name="language">The language</param>
    public ScriptingCalendar(ScriptingConfiguration configuration, PayrollCalendar payrollCalendar,
        Language? language = null)
    {
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        PayrollCalendar = payrollCalendar ?? throw new ArgumentNullException(nameof(payrollCalendar));
        Language = language ?? default;
    }

    /// <summary>Returns a string that represents this instance</summary>
    public override string ToString()
    {
        var buffer = new StringBuilder();
        buffer.Append($"Period={PeriodDate.ToUtc().ToPeriodStartString()}");
        buffer.Append($", Evaluation={EvaluationDate.ToUtc().ToPeriodStartString()}");
        if (RegulationDate.IsDefined())
        {
            buffer.Append($", Regulation={RegulationDate.ToUtc().ToPeriodStartString()}");
        }
        return buffer.ToString();
    }
}