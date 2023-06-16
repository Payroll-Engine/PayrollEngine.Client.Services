using System;
using System.Globalization;
using Calendar = PayrollEngine.Client.Model.Calendar;

namespace PayrollEngine.Client.Scripting;

/// <summary>Payroll month period, implements the <see cref="PayrollEngine.IPayrollPeriod" /></summary>
public class MonthPayrollPeriod : IPayrollPeriod
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

    /// <inheritdoc />
    public MonthPayrollPeriod(CultureInfo culture, Calendar calendar, DateTime moment) :
        this(culture, calendar, moment.Year, moment.Month)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="MonthPayrollPeriod"/> class</summary>
    /// <param name="culture">The culture</param>
    /// <param name="calendar">The calendar</param>
    /// <param name="year">The year</param>
    /// <param name="month">The month</param>
    public MonthPayrollPeriod(CultureInfo culture, Calendar calendar, int year, int month)
    {
        Culture = culture ?? throw new ArgumentNullException(nameof(culture));
        Calendar = calendar ?? throw new ArgumentNullException(nameof(calendar));

        // period
        Period = new(
            new(year, month, 1, 0, 0, 0, 0, Culture.Calendar, DateTimeKind.Utc),
            new DateTime(year, month, PayrollEngine.Date.DaysInMonth(year, month), 0, 0, 0, 0, Culture.Calendar, DateTimeKind.Utc).LastMomentOfDay());
    }

    #region IPayrollPeriod

    /// <inheritdoc />
    public virtual DateTime Start => Period.Start;

    /// <inheritdoc />
    public virtual DateTime End => Period.End;

    /// <inheritdoc />
    public virtual string Name =>
        // ReSharper disable once StringLiteralTypo
        Period.Start.ToString("yyyy-MM", Culture);

    /// <inheritdoc />
    public virtual IPayrollPeriod GetPayrollPeriod(DateTime moment, int offset = 0) =>
        offset == 0 ? new(Culture, Calendar, moment) :
            new MonthPayrollPeriod(Culture, Calendar, moment.AddMonths(offset));

    #endregion

    /// <summary>Returns a <see cref="string" /> that represents this instance</summary>
    /// <returns>A <see cref="string" /> that represents this instance</returns>
    public override string ToString() => Name;
}