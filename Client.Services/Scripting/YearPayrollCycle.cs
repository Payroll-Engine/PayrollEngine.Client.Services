using System;
using PayrollEngine.Client.Model;

namespace PayrollEngine.Client.Scripting;

/// <summary>Payroll year cycle, implements the <see cref="PayrollEngine.IPayrollPeriod" /></summary>
public class YearPayrollCycle : IPayrollPeriod
{
    private DatePeriod Period { get; }

    /// <summary>The culture</summary>
    public System.Globalization.CultureInfo Culture { get; }

    /// <summary>The date calendar</summary>
    public Calendar Calendar { get; }

    /// <inheritdoc />
    public YearPayrollCycle(System.Globalization.CultureInfo culture, Calendar calendar, DateTime moment) :
        this(culture, calendar, moment.Year, moment.Month)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="YearPayrollCycle"/> class</summary>
    /// <param name="culture">The culture</param>
    /// <param name="calendar">The calendar</param>
    /// <param name="year">The year</param>
    /// <param name="month">The month</param>
    public YearPayrollCycle(System.Globalization.CultureInfo culture, Calendar calendar, int year, int month)
    {
        Culture = culture ?? throw new ArgumentNullException(nameof(culture));
        Calendar = calendar ?? throw new ArgumentNullException(nameof(calendar));

        // period
        var periodStart = new DateTime(year, month, 1, 0, 0, 0, 0, culture.Calendar, DateTimeKind.Utc);

        // month offset
        var firstMonth = (int)Month.January;
        if (calendar.FirstMonthOfYear.HasValue)
        {
            firstMonth = (int)calendar.FirstMonthOfYear.Value;
        }
        // first moment of the year
        DateTime cycleStart;
        if (month == firstMonth)
        {
            cycleStart = periodStart;
        }
        else if (month > firstMonth)
        {
            cycleStart = periodStart.AddMonths((month - firstMonth) * -1);
        }
        else
        {
            cycleStart = periodStart.AddMonths((month + PayrollEngine.Date.MonthsInYear - firstMonth) * -1);
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
        Period.Start.ToString("yyyy", Culture);

    /// <inheritdoc />
    public virtual IPayrollPeriod GetPayrollPeriod(DateTime moment, int offset = 0) =>
        offset == 0 ? new(Culture, Calendar, moment) :
            new YearPayrollCycle(Culture, Calendar, moment.AddYears(offset));

    #endregion

    /// <summary>Returns a <see cref="string" /> that represents this instance</summary>
    /// <returns>A <see cref="string" /> that represents this instance</returns>
    public override string ToString() => Name;
}