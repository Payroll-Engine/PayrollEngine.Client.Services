using System;

namespace PayrollEngine.Client.Scripting;

/// <summary>Payroll week period, implements the <see cref="PayrollEngine.IPayrollPeriod" /></summary>
public class WeekPayrollPeriod : IPayrollPeriod
{
    private DatePeriod Period { get; }

    /// <summary>The payroll calendar</summary>
    public IPayrollCalendar Calendar { get; }

    /// <summary>Gets the week of year</summary>
    public int WeekOfYear =>
        Calendar.GetWeekOfYear(Period.Start);

    /// <inheritdoc />
    public WeekPayrollPeriod(IPayrollCalendar calendar, int year, int month, int day) :
        this(calendar, new(year, month, day, 0, 0, 0, DateTimeKind.Utc))
    {
    }

    /// <summary>Initializes a new instance of the <see cref="WeekPayrollPeriod"/> class</summary>
    /// <param name="calendar">The calendar</param>
    /// <param name="moment">The moment</param>
    public WeekPayrollPeriod(IPayrollCalendar calendar, DateTime moment)
    {
        Calendar = calendar ?? throw new ArgumentNullException(nameof(calendar));
        var startOfWeek = moment.GetPreviousWeekDay(calendar.Configuration.FirstDayOfWeek);
        Period = new(
            startOfWeek,
            startOfWeek.AddDays(PayrollEngine.Date.DaysInWeek - 1).LastMomentOfDay());
    }

    #region IPayrollPeriod

    /// <inheritdoc />
    public virtual DateTime Start => Period.Start;

    /// <inheritdoc />
    public virtual DateTime End => Period.End;

    /// <inheritdoc />
    public virtual string Name =>
        $"{Period.Start.ToString("yyyy", Calendar.Culture)} {WeekOfYear}";

    /// <inheritdoc />
    public virtual IPayrollPeriod GetPayrollPeriod(DateTime moment, int offset = 0) =>
        offset == 0 ? new(Calendar, moment) :
            new WeekPayrollCycle(Calendar, moment.AddYears(offset));

    #endregion

    /// <summary>Returns a <see cref="string" /> that represents this instance</summary>
    /// <returns>A <see cref="string" /> that represents this instance</returns>
    public override string ToString() => Name;
}