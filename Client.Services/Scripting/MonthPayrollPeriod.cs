using System;

namespace PayrollEngine.Client.Scripting;

/// <summary>Payroll month period, implements the <see cref="PayrollEngine.IPayrollPeriod" /></summary>
public class MonthPayrollPeriod : IPayrollPeriod
{
    private DatePeriod Period { get; }

    /// <summary>The payroll calendar</summary>
    public IPayrollCalendar Calendar { get; }

    /// <inheritdoc />
    public MonthPayrollPeriod(IPayrollCalendar calendar, DateTime moment) :
        this(calendar, moment.Year, moment.Month)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="MonthPayrollPeriod"/> class</summary>
    /// <param name="calendar">The calendar</param>
    /// <param name="year">The year</param>
    /// <param name="month">The month</param>
    public MonthPayrollPeriod(IPayrollCalendar calendar, int year, int month)
    {
        Calendar = calendar ?? throw new ArgumentNullException(nameof(calendar));
        Period = new(
            new(year, month, 1, 0, 0, 0, 0, calendar.Calendar, DateTimeKind.Utc),
            new DateTime(year, month, PayrollEngine.Date.DaysInMonth(year, month), 0, 0, 0, 0, calendar.Calendar, DateTimeKind.Utc).LastMomentOfDay());
    }

    #region IPayrollPeriod

    /// <inheritdoc />
    public virtual DateTime Start => Period.Start;

    /// <inheritdoc />
    public virtual DateTime End => Period.End;

    /// <inheritdoc />
    public virtual string Name =>
        // ReSharper disable once StringLiteralTypo
        Period.Start.ToString("yyyy-MM", Calendar.Culture);

    /// <inheritdoc />
    public virtual IPayrollPeriod GetPayrollPeriod(DateTime moment, int offset = 0) =>
        offset == 0 ? new(Calendar, moment) :
            new MonthPayrollPeriod(Calendar, moment.AddMonths(offset));

    #endregion

    /// <summary>Returns a <see cref="string" /> that represents this instance</summary>
    /// <returns>A <see cref="string" /> that represents this instance</returns>
    public override string ToString() => Name;
}