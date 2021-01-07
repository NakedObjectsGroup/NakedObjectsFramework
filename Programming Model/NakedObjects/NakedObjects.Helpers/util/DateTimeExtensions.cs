// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Globalization;
using NakedFramework;

namespace NakedObjects {
    /// <summary>
    ///     Helper class with DateTime extension methods for common date operations.
    /// </summary>
    /// <seealso cref="Calendar" />
    public static class DateTimeExtensions {
        #region StartOfPeriod

        /// <summary>
        ///     First day of the week of the passed in date
        /// </summary>
        /// <remarks>
        ///     Uses <see cref="CultureInfo.CurrentCulture" /> to determine first day of week
        /// </remarks>
        public static DateTime StartOfWeek(this DateTime referenceDate) => DateTimeHelper.StartOfWeek(referenceDate);

        /// <summary>
        ///     First day of the month of the passed in date
        /// </summary>
        public static DateTime StartOfMonth(this DateTime referenceDate) => DateTimeHelper.StartOfMonth(referenceDate);

        /// <summary>
        ///     First day of the year of the passed in date
        /// </summary>
        public static DateTime StartOfYear(this DateTime referenceDate) => DateTimeHelper.StartOfYear(referenceDate);

        #endregion

        #region EndOfPeriod

        /// <summary>
        ///     Last day of the week of the passed in date
        /// </summary>
        /// <remarks>
        ///     Uses <see cref="CultureInfo.CurrentCulture" /> to determine <i>first</i> day of week
        ///     and then assumes 7 day week
        /// </remarks>
        public static DateTime EndOfWeek(this DateTime referenceDate) => DateTimeHelper.EndOfWeek(referenceDate);

        /// <summary>
        ///     Last day of the month of the passed in date
        /// </summary>
        public static DateTime EndOfMonth(this DateTime referenceDate) => DateTimeHelper.EndOfMonth(referenceDate);

        /// <summary>
        ///     Last day of the year of the passed in date
        /// </summary>
        public static DateTime EndOfYear(this DateTime referenceDate) => DateTimeHelper.EndOfYear(referenceDate);

        #endregion

        #region CompareToToday

        /// <summary>
        ///     Check if date is after today ignoring time
        /// </summary>
        public static bool IsAfterToday(this DateTime referenceDate) => DateTimeHelper.IsAfterToday(referenceDate);

        /// <summary>
        ///     Check if date is before today ignoring time
        /// </summary>
        public static bool IsBeforeToday(this DateTime referenceDate) => DateTimeHelper.IsBeforeToday(referenceDate);

        /// <summary>
        ///     Check if date is today ignoring time
        /// </summary>
        public static bool IsToday(this DateTime referenceDate) => DateTimeHelper.IsToday(referenceDate);

        /// <summary>
        ///     Check if date is after today ignoring time
        /// </summary>
        public static bool IsAfterToday(this DateTime? referenceDate) => DateTimeHelper.IsAfterToday(referenceDate);

        /// <summary>
        ///     Check if date is before today ignoring time
        /// </summary>
        public static bool IsBeforeToday(this DateTime? referenceDate) => DateTimeHelper.IsBeforeToday(referenceDate);

        /// <summary>
        ///     Check if date is today ignoring time
        /// </summary>
        public static bool IsToday(this DateTime? referenceDate) => DateTimeHelper.IsToday(referenceDate);

        #endregion

        #region IsSameDayAs

        /// <summary>
        ///     Check if two dates are the same ignoring time
        /// </summary>
        public static bool IsSameDayAs(this DateTime referenceDate, DateTime? otherDate) => DateTimeHelper.IsSameDayAs(referenceDate, otherDate);

        /// <summary>
        ///     Check if two dates are the same ignoring time
        /// </summary>
        public static bool IsSameDayAs(this DateTime? referenceDate, DateTime? otherDate) => DateTimeHelper.IsSameDayAs(referenceDate, otherDate);

        #endregion

        #region IsSameWeekAs

        /// <summary>
        ///     Check if two dates are in the same week ignoring time
        /// </summary>
        /// <remarks>
        ///     Uses <see cref="CultureInfo.CurrentCulture" /> to determine week
        /// </remarks>
        public static bool IsSameWeekAs(this DateTime referenceDate, DateTime? otherDate) => DateTimeHelper.IsSameWeekAs(referenceDate, otherDate);

        /// <summary>
        ///     Check if two dates are in the same week ignoring time
        /// </summary>
        /// <remarks>
        ///     Uses <see cref="CultureInfo.CurrentCulture" /> to determine week
        /// </remarks>
        public static bool IsSameWeekAs(this DateTime? referenceDate, DateTime? otherDate) => DateTimeHelper.IsSameWeekAs(referenceDate, otherDate);

        #endregion

        #region IsSameMonthAs

        /// <summary>
        ///     Check if two dates are in the same month ignoring time
        /// </summary>
        public static bool IsSameMonthAs(this DateTime referenceDate, DateTime? otherDate) => DateTimeHelper.IsSameMonthAs(referenceDate, otherDate);

        /// <summary>
        ///     Check if two dates are in the same month ignoring time
        /// </summary>
        public static bool IsSameMonthAs(this DateTime? referenceDate, DateTime? otherDate) => DateTimeHelper.IsSameMonthAs(referenceDate, otherDate);

        #endregion

        #region IsSameYearAs

        /// <summary>
        ///     Check if two dates are in the same year ignoring time
        /// </summary>
        public static bool IsSameYearAs(this DateTime referenceDate, DateTime? otherDate) => DateTimeHelper.IsSameYearAs(referenceDate, otherDate);

        /// <summary>
        ///     Check if two dates are in the same year ignoring time
        /// </summary>
        public static bool IsSameYearAs(this DateTime? referenceDate, DateTime? otherDate) => DateTimeHelper.IsSameYearAs(referenceDate, otherDate);

        #endregion

        #region IsAtLeastADayBefore

        /// <summary>
        ///     Check if referenceDate is at least the day before the otherDate  ignoring time
        /// </summary>
        public static bool IsAtLeastOneDayBefore(this DateTime referenceDate, DateTime? otherDate) => DateTimeHelper.IsAtLeastOneDayBefore(referenceDate, otherDate);

        /// <summary>
        ///     Check if referenceDate is at least the day before the otherDate  ignoring time
        /// </summary>
        public static bool IsAtLeastOneDayBefore(this DateTime? referenceDate, DateTime? otherDate) => DateTimeHelper.IsAtLeastOneDayBefore(referenceDate, otherDate);

        #endregion

        #region IsAtLeastADayAfter

        /// <summary>
        ///     Check if referanceDate is at least one day after the otherDate ignoring time
        /// </summary>
        public static bool IsAtLeastOneDayAfter(this DateTime referenceDate, DateTime? otherDate) => DateTimeHelper.IsAtLeastOneDayAfter(referenceDate, otherDate);

        /// <summary>
        ///     Check if referenceDate is at least one day after the otherDate ignoring time
        /// </summary>
        public static bool IsAtLeastOneDayAfter(this DateTime? referenceDate, DateTime? otherDate) => DateTimeHelper.IsAtLeastOneDayAfter(referenceDate, otherDate);

        #endregion
    }
}