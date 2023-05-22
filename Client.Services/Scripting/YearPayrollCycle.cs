using System;

namespace PayrollEngine.Client.Scripting;

/// <summary>Payroll year cycle, implements the <see cref="PayrollEngine.IPayrollPeriod" /></summary>
public class YearPayrollCycle : IPayrollPeriod
{
    private DatePeriod Period { get; }

    /// <summary>The payroll calendar</summary>
    public IPayrollCalendar Calendar { get; }

    /// <inheritdoc />
    public YearPayrollCycle(IPayrollCalendar calendar, DateTime moment) :
        this(calendar, moment.Year, moment.Month)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="YearPayrollCycle"/> class</summary>
    /// <param name="calendar">The calendar</param>
    /// <param name="year">The year</param>
    /// <param name="month">The month</param>
    public YearPayrollCycle(IPayrollCalendar calendar, int year, int month)
    {
        Calendar = calendar ?? throw new ArgumentNullException(nameof(calendar));

        // period
        var periodStart = new DateTime(year, month, 1, 0, 0, 0, 0, calendar.Calendar, DateTimeKind.Utc);

        // first moment of the year
        DateTime cycleStart;
        if (month == (int)calendar.Configuration.FirstMonthOfYear)
        {
            cycleStart = periodStart;
        }
        else if (month > (int)calendar.Configuration.FirstMonthOfYear)
        {
            cycleStart = periodStart.AddMonths((month - (int)calendar.Configuration.FirstMonthOfYear) * -1);
        }
        else
        {
            cycleStart = periodStart.AddMonths((month + PayrollEngine.Date.MonthsInYear - (int)calendar.Configuration.FirstMonthOfYear) * -1);
        }

        // last moment of the year
        var cycleEnd = cycleStart.AddMonths(PayrollEngine.Date.MonthsInYear).AddDays(-1).LastMomentOfDay();
        Period = new(cycleStart, cycleEnd);
    }

    #region IPayrollPeriod

    /// <inheritdoc />
    public virtual DateTime Start => Period.Start;

    /// <inheritdoc />
    public virtual DateTime End => Period.End;

    /// <inheritdoc />
    public virtual string Name =>
        Period.Start.ToString("yyyy", Calendar.Culture);

    /// <inheritdoc />
    public virtual IPayrollPeriod GetPayrollPeriod(DateTime moment, int offset = 0) =>
        offset == 0 ? new(Calendar, moment) :
            new YearPayrollCycle(Calendar, moment.AddYears(offset));

    #endregion

    /// <summary>Returns a <see cref="string" /> that represents this instance</summary>
    /// <returns>A <see cref="string" /> that represents this instance</returns>
    public override string ToString() => Name;
}