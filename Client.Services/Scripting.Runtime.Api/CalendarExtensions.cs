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
    /// <param name="calendar">The payroll calendar</param>
    extension(Calendar calendar)
    {
        /// <summary>Test for working days</summary>
        /// <param name="moment">Test day</param>
        /// <returns>Returns true for valid time units</returns>
        public bool IsWorkDay(DateTime moment)
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
        /// <param name="moment">The start moment (not included in results)</param>
        /// <param name="count">The number of days (default: 1)</param>
        /// <returns>Returns true for valid time units</returns>
        public List<DateTime> GetPreviousWorkDays(DateTime moment, int count = 1)
        {
            var days = new List<DateTime>();
            for (var i = 0; i < count; i++)
            {
                var day = moment.AddDays(-i).Date;
                if (calendar.IsWorkDay(day))
                {
                    days.Add(day);
                }
            }
            return days;
        }

        /// <summary>Get next working days</summary>
        /// <param name="moment">The start moment (not included in results)</param>
        /// <param name="count">The number of days (default: 1)</param>
        /// <returns>Returns true for valid time units</returns>
        public List<DateTime> GetNextWorkDays(DateTime moment, int count = 1)
        {
            var days = new List<DateTime>();
            for (var i = 0; i < count; i++)
            {
                var day = moment.AddDays(i).Date;
                if (calendar.IsWorkDay(day))
                {
                    days.Add(day);
                }
            }
            return days;
        }
    }
}