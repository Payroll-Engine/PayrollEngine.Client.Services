using System;
using PayrollEngine.Client.Model;
using CultureInfo = System.Globalization.CultureInfo;

namespace PayrollEngine.Client.Scripting;

/// <summary>Payroll week period, implements the <see cref="PayrollEngine.IPayrollPeriod" /></summary>
public class WeekPayrollPeriod : IPayrollPeriod
{
    private DatePeriod Period { get; }
    /// <summary>
    /// The culture
    /// </summary>
    public CultureInfo Culture { get; }

    /// <summary>
    /// The date calendar
    /// </summary>
    public Calendar Calendar { get; }

    /// <summary>Gets the week of year</summary>
    public int WeekOfYear =>
        Calendar.GetWeekOfYear(Culture, Period.Start);

    /// <inheritdoc />
    public WeekPayrollPeriod(CultureInfo culture, Calendar calendar, int year, int month, int day) :
        this(culture, calendar, new(year, month, day, 0, 0, 0, DateTimeKind.Utc))
    {
    }

    /// <summary>Initializes a new instance of the <see cref="WeekPayrollPeriod"/> class</summary>
    /// <param name="culture">The culture</param>
    /// <param name="calendar">The calendar</param>
    /// <param name="moment">The moment</param>
    public WeekPayrollPeriod(CultureInfo culture, Calendar calendar, DateTime moment)
    {
        Culture = culture ?? throw new ArgumentNullException(nameof(culture));
        Calendar = calendar ?? throw new ArgumentNullException(nameof(calendar));
        var firstDayOfWeek = calendar.FirstDayOfWeek ?? (DayOfWeek)culture.DateTimeFormat.FirstDayOfWeek;
        var startOfWeek = moment.GetPreviousWeekDay(firstDayOfWeek);
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
        $"{Period.Start.ToString("yyyy", Culture)} {WeekOfYear}";

    /// <inheritdoc />
    public IPayrollPeriod GetPayrollPeriod(DateTime moment, int offset = 0) =>
        offset == 0 ? new(Culture, Calendar, moment) :
            new WeekPayrollPeriod(Culture, Calendar, moment.AddDays(offset * Date.DaysInWeek));

    #endregion

    /// <summary>Returns a <see cref="string" /> that represents this instance</summary>
    /// <returns>A <see cref="string" /> that represents this instance</returns>
    public override string ToString() => Name;
}