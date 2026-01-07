using System;
using System.Text;
using System.Globalization;

namespace PayrollEngine.Client.Scripting;

/// <summary>Script calendar</summary>
public class ScriptCalendar
{
    /// <summary>The scripting configuration</summary>
    public ScriptConfiguration Configuration { get; }

    /// <summary>The payroll calendar</summary>
    public string CalendarName { get; }

    /// <summary>The culture</summary>
    public string Culture { get; }

    /// <summary>The culture name</summary>
    public string CultureName => Configuration.CultureName;

    /// <summary>The regulation date</summary>
    public DateTime PeriodDate => Configuration.PeriodDate;

    /// <summary>The evaluation date</summary>
    public DateTime EvaluationDate => Configuration.EvaluationDate;

    /// <summary>The regulation date</summary>
    public DateTime RegulationDate => Configuration.RegulationDate;

    /// <summary>Initializes a new instance of the <see cref="ScriptCalendar"/> class</summary>
    /// <param name="configuration">The scripting configuration</param>
    /// <param name="tenantCalendar">The payroll calendar</param>
    /// <param name="culture">The culture</param>
    public ScriptCalendar(ScriptConfiguration configuration, string tenantCalendar,
        string culture = null)
    {
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        // fallback to tenant calendar
        CalendarName = Configuration.CalendarName ?? tenantCalendar;
        Culture = culture ?? CultureInfo.CurrentCulture.Name;
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