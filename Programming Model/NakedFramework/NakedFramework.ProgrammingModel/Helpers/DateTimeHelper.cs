// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Globalization;

namespace NakedFramework; 

/// <summary>
///     Helper class with DateTime extension methods for common date operations.
/// </summary>
/// <seealso cref="Calendar" />
public static class DateTimeHelper {
    #region StartOfPeriod

    /// <summary>
    ///     First day of the week of the passed in date
    /// </summary>
    /// <remarks>
    ///     Uses <see cref="CultureInfo.CurrentCulture" /> to determine first day of week
    /// </remarks>
    public static DateTime StartOfWeek(DateTime referenceDate) {
        var currentDOW = CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(referenceDate.Date);
        return referenceDate.Date.Subtract(new TimeSpan((int)currentDOW, 0, 0, 0));
    }

    /// <summary>
    ///     First day of the month of the passed in date
    /// </summary>
    public static DateTime StartOfMonth(DateTime referenceDate) => new(referenceDate.Year, referenceDate.Month, 1);

    /// <summary>
    ///     First day of the year of the passed in date
    /// </summary>
    public static DateTime StartOfYear(DateTime referenceDate) => new(referenceDate.Year, 1, 1);

    #endregion

    #region EndOfPeriod

    /// <summary>
    ///     Last day of the week of the passed in date
    /// </summary>
    /// <remarks>
    ///     Uses <see cref="CultureInfo.CurrentCulture" /> to determine <i>first</i> day of week
    ///     and then assumes 7 day week
    /// </remarks>
    public static DateTime EndOfWeek(DateTime referenceDate) => StartOfWeek(referenceDate).AddDays(6);

    /// <summary>
    ///     Last day of the month of the passed in date
    /// </summary>
    public static DateTime EndOfMonth(DateTime referenceDate) => StartOfMonth(referenceDate.Date.AddMonths(1)).AddDays(-1);

    /// <summary>
    ///     Last day of the year of the passed in date
    /// </summary>
    public static DateTime EndOfYear(DateTime referenceDate) => StartOfYear(referenceDate.Date.AddYears(1)).AddDays(-1);

    #endregion

    #region CompareToToday

    /// <summary>
    ///     Check if date is after today ignoring time
    /// </summary>
    public static bool IsAfterToday(DateTime referenceDate) => referenceDate.Date > DateTime.Now.Date;

    /// <summary>
    ///     Check if date is before today ignoring time
    /// </summary>
    public static bool IsBeforeToday(DateTime referenceDate) => referenceDate.Date < DateTime.Now.Date;

    /// <summary>
    ///     Check if date is today ignoring time
    /// </summary>
    public static bool IsToday(DateTime referenceDate) => referenceDate.Date == DateTime.Now.Date;

    /// <summary>
    ///     Check if date is after today ignoring time
    /// </summary>
    public static bool IsAfterToday(DateTime? referenceDate) => referenceDate.HasValue && IsAfterToday(referenceDate.Value);

    /// <summary>
    ///     Check if date is before today ignoring time
    /// </summary>
    public static bool IsBeforeToday(DateTime? referenceDate) => referenceDate.HasValue && IsBeforeToday(referenceDate.Value);

    /// <summary>
    ///     Check if date is today ignoring time
    /// </summary>
    public static bool IsToday(DateTime? referenceDate) => referenceDate.HasValue && IsToday(referenceDate.Value);

    #endregion

    #region IsSameDayAs

    /// <summary>
    ///     Check if two dates are the same ignoring time
    /// </summary>
    public static bool IsSameDayAs(DateTime referenceDate, DateTime? otherDate) => otherDate.HasValue && referenceDate.Date == otherDate.Value.Date;

    /// <summary>
    ///     Check if two dates are the same ignoring time
    /// </summary>
    public static bool IsSameDayAs(DateTime? referenceDate, DateTime? otherDate) => referenceDate.HasValue && IsSameDayAs(referenceDate.Value, otherDate);

    #endregion

    #region IsSameWeekAs

    /// <summary>
    ///     Check if two dates are in the same week ignoring time
    /// </summary>
    /// <remarks>
    ///     Uses <see cref="CultureInfo.CurrentCulture" /> to determine week
    /// </remarks>
    public static bool IsSameWeekAs(DateTime referenceDate, DateTime? otherDate) => otherDate.HasValue && StartOfWeek(referenceDate) == StartOfWeek(otherDate.Value);

    /// <summary>
    ///     Check if two dates are in the same week ignoring time
    /// </summary>
    /// <remarks>
    ///     Uses <see cref="CultureInfo.CurrentCulture" /> to determine week
    /// </remarks>
    public static bool IsSameWeekAs(DateTime? referenceDate, DateTime? otherDate) => referenceDate.HasValue && IsSameWeekAs(referenceDate.Value, otherDate);

    #endregion

    #region IsSameMonthAs

    /// <summary>
    ///     Check if two dates are in the same month ignoring time
    /// </summary>
    public static bool IsSameMonthAs(DateTime referenceDate, DateTime? otherDate) => otherDate.HasValue && StartOfMonth(referenceDate) == StartOfMonth(otherDate.Value);

    /// <summary>
    ///     Check if two dates are in the same month ignoring time
    /// </summary>
    public static bool IsSameMonthAs(DateTime? referenceDate, DateTime? otherDate) => referenceDate.HasValue && IsSameMonthAs(referenceDate.Value, otherDate);

    #endregion

    #region IsSameYearAs

    /// <summary>
    ///     Check if two dates are in the same year ignoring time
    /// </summary>
    public static bool IsSameYearAs(DateTime referenceDate, DateTime? otherDate) => otherDate.HasValue && referenceDate.Date.Year == otherDate.Value.Date.Year;

    /// <summary>
    ///     Check if two dates are in the same year ignoring time
    /// </summary>
    public static bool IsSameYearAs(DateTime? referenceDate, DateTime? otherDate) => referenceDate.HasValue && IsSameYearAs(referenceDate.Value, otherDate);

    #endregion

    #region IsAtLeastADayBefore

    /// <summary>
    ///     Check if referenceDate is at least the day before the otherDate  ignoring time
    /// </summary>
    public static bool IsAtLeastOneDayBefore(DateTime referenceDate, DateTime? otherDate) => otherDate.HasValue && referenceDate.Date < otherDate.Value.Date;

    /// <summary>
    ///     Check if referenceDate is at least the day before the otherDate  ignoring time
    /// </summary>
    public static bool IsAtLeastOneDayBefore(DateTime? referenceDate, DateTime? otherDate) => referenceDate.HasValue && IsAtLeastOneDayBefore(referenceDate.Value, otherDate);

    #endregion

    #region IsAtLeastADayAfter

    /// <summary>
    ///     Check if referanceDate is at least one day after the otherDate ignoring time
    /// </summary>
    public static bool IsAtLeastOneDayAfter(DateTime referenceDate, DateTime? otherDate) => otherDate.HasValue && referenceDate.Date > otherDate.Value.Date;

    /// <summary>
    ///     Check if referenceDate is at least one day after the otherDate ignoring time
    /// </summary>
    public static bool IsAtLeastOneDayAfter(DateTime? referenceDate, DateTime? otherDate) => referenceDate.HasValue && IsAtLeastOneDayAfter(referenceDate.Value, otherDate);

    #endregion
}