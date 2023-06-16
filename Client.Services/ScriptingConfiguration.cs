using System;

namespace PayrollEngine.Client;

/// <summary>The Payroll scripting configuration</summary>
public class ScriptingConfiguration
{
    /// <summary>The culture name</summary>
    public string CultureName { get; set; }

    /// <summary>The calendar name</summary>
    public string CalendarName { get; set; }

    /// <summary>The evaluation date</summary>
    public DateTime EvaluationDate { get; set; }

    /// <summary>The period date</summary>
    public DateTime PeriodDate { get; set; }

    /// <summary>The regulation date</summary>
    public DateTime RegulationDate { get; set; }

    /// <summary>Returns a <see cref="string" /> that represents this instance</summary>
    public override string ToString() =>
        $"Evaluation: {EvaluationDate}, Period: {PeriodDate}, Regulation: {RegulationDate}";
}