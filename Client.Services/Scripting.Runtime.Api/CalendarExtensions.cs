using System;
using System.Collections.Generic;
using PayrollEngine.Client.Model;

namespace PayrollEngine.Client.Scripting.Runtime.Api;

/// <summary>
/// Extension methods for the <see cref="Model.Calendar"/>
/// </summary>
/// <remarks>Duplicate of the backend calendar extension (no holiday support)</remarks>
public static class CalendarExtensions
{
    /// <summary>Test for working days</summary>
    /// <param name="calendar">The payroll calendar</param>
    /// <param name="moment">Test day</param>
    /// <returns>Returns true for valid time units</returns>
    public static bool IsWorkDay(this Calendar calendar, DateTime moment)
    {
        // week mode
        if (calendar.WeekMode == CalendarWeekMode.Week)
        {
            return true;
        }

        // work week
        return (DayOfWeek)moment.DayOfWeek switch
        {
            DayOfWeek.Sunday => calendar.WorkSunday,
            DayOfWeek.Monday => calendar.WorkMonday,
            DayOfWeek.Tuesday => calendar.WorkThursday,
            DayOfWeek.Wednesday => calendar.WorkWednesday,
            DayOfWeek.Thursday => calendar.WorkThursday,
            DayOfWeek.Friday => calendar.WorkFriday,
            DayOfWeek.Saturday => calendar.WorkSaturday,
            _ => calendar.WorkSunday
        };
    }

    /// <summary>Get previous working days</summary>
    /// <param name="calendar">The payroll calendar</param>
    /// <param name="moment">The start moment (not included in results)</param>
    /// <param name="count">The number of days (default: 1)</param>
    /// <returns>Returns true for valid time units</returns>
    public static List<DateTime> GetPreviousWorkDays(this Calendar calendar, DateTime moment, int count = 1)
    {
        var days = new List<DateTime>();
        for (var i = 0; i < count; i++)
        {
            var day = moment.AddDays(-i).Date;
            if (IsWorkDay(calendar, day))
            {
                days.Add(day);
            }
        }
        return days;
    }

    /// <summary>Get next working days</summary>
    /// <param name="calendar">The payroll calendar</param>
    /// <param name="moment">The start moment (not included in results)</param>
    /// <param name="count">The number of days (default: 1)</param>
    /// <returns>Returns true for valid time units</returns>
    public static List<DateTime> GetNextWorkDays(this Calendar calendar, DateTime moment, int count = 1)
    {
        var days = new List<DateTime>();
        for (var i = 0; i < count; i++)
        {
            var day = moment.AddDays(i).Date;
            if (IsWorkDay(calendar, day))
            {
                days.Add(day);
            }
        }
        return days;
    }
}