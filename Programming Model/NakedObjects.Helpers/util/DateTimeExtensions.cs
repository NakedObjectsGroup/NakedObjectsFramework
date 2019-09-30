// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Globalization;

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
        public static DateTime StartOfWeek(this DateTime referenceDate) {
            DayOfWeek currentDOW = CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(referenceDate.Date);
            return referenceDate.Date.Subtract(new TimeSpan((int) currentDOW, 0, 0, 0));
        }

        /// <summary>
        ///     First day of the month of the passed in date
        /// </summary>
        public static DateTime StartOfMonth(this DateTime referenceDate) {
            return new DateTime(referenceDate.Year, referenceDate.Month, 1);
        }

        /// <summary>
        ///     First day of the year of the passed in date
        /// </summary>
        public static DateTime StartOfYear(this DateTime referenceDate) {
            return new DateTime(referenceDate.Year, 1, 1);
        }

        #endregion

        #region EndOfPeriod

        /// <summary>
        ///     Last day of the week of the passed in date
        /// </summary>
        /// <remarks>
        ///     Uses <see cref="CultureInfo.CurrentCulture" /> to determine <i>first</i> day of week
        ///     and then assumes 7 day week
        /// </remarks>
        public static DateTime EndOfWeek(this DateTime referenceDate) {
            return StartOfWeek(referenceDate).AddDays(6);
        }

        /// <summary>
        ///     Last day of the month of the passed in date
        /// </summary>
        public static DateTime EndOfMonth(this DateTime referenceDate) {
            return StartOfMonth(referenceDate.Date.AddMonths(1)).AddDays(-1);
        }

        /// <summary>
        ///     Last day of the year of the passed in date
        /// </summary>
        public static DateTime EndOfYear(this DateTime referenceDate) {
            return StartOfYear(referenceDate.Date.AddYears(1)).AddDays(-1);
        }

        #endregion

        #region CompareToToday

        /// <summary>
        ///     Check if date is after today ignoring time
        /// </summary>
        public static bool IsAfterToday(this DateTime referenceDate) {
            return referenceDate.Date > DateTime.Now.Date;
        }

        /// <summary>
        ///     Check if date is before today ignoring time
        /// </summary>
        public static bool IsBeforeToday(this DateTime referenceDate) {
            return referenceDate.Date < DateTime.Now.Date;
        }

        /// <summary>
        ///     Check if date is today ignoring time
        /// </summary>
        public static bool IsToday(this DateTime referenceDate) {
            return referenceDate.Date == DateTime.Now.Date;
        }

        /// <summary>
        ///     Check if date is after today ignoring time
        /// </summary>
        public static bool IsAfterToday(this DateTime? referenceDate) {
            return referenceDate.HasValue && referenceDate.Value.IsAfterToday();
        }

        /// <summary>
        ///     Check if date is before today ignoring time
        /// </summary>
        public static bool IsBeforeToday(this DateTime? referenceDate) {
            return referenceDate.HasValue && referenceDate.Value.IsBeforeToday();
        }

        /// <summary>
        ///     Check if date is today ignoring time
        /// </summary>
        public static bool IsToday(this DateTime? referenceDate) {
            return referenceDate.HasValue && referenceDate.Value.IsToday();
        }

        #endregion

        #region IsSameDayAs

        /// <summary>
        ///     Check if two dates are the same ignoring time
        /// </summary>
        public static bool IsSameDayAs(this DateTime referenceDate, DateTime? otherDate) {
            return otherDate.HasValue && referenceDate.Date == otherDate.Value.Date;
        }

        /// <summary>
        ///     Check if two dates are the same ignoring time
        /// </summary>
        public static bool IsSameDayAs(this DateTime? referenceDate, DateTime? otherDate) {
            return referenceDate.HasValue && referenceDate.Value.IsSameDayAs(otherDate);
        }

        #endregion

        #region IsSameWeekAs

        /// <summary>
        ///     Check if two dates are in the same week ignoring time
        /// </summary>
        /// <remarks>
        ///     Uses <see cref="CultureInfo.CurrentCulture" /> to determine week
        /// </remarks>
        public static bool IsSameWeekAs(this DateTime referenceDate, DateTime? otherDate) {
            return otherDate.HasValue && StartOfWeek(referenceDate) == StartOfWeek(otherDate.Value);
        }

        /// <summary>
        ///     Check if two dates are in the same week ignoring time
        /// </summary>
        /// <remarks>
        ///     Uses <see cref="CultureInfo.CurrentCulture" /> to determine week
        /// </remarks>
        public static bool IsSameWeekAs(this DateTime? referenceDate, DateTime? otherDate) {
            return referenceDate.HasValue && referenceDate.Value.IsSameWeekAs(otherDate);
        }

        #endregion

        #region IsSameMonthAs

        /// <summary>
        ///     Check if two dates are in the same month ignoring time
        /// </summary>
        public static bool IsSameMonthAs(this DateTime referenceDate, DateTime? otherDate) {
            return otherDate.HasValue && StartOfMonth(referenceDate) == StartOfMonth(otherDate.Value);
        }

        /// <summary>
        ///     Check if two dates are in the same month ignoring time
        /// </summary>
        public static bool IsSameMonthAs(this DateTime? referenceDate, DateTime? otherDate) {
            return referenceDate.HasValue && referenceDate.Value.IsSameMonthAs(otherDate);
        }

        #endregion

        #region IsSameYearAs

        /// <summary>
        ///     Check if two dates are in the same year ignoring time
        /// </summary>
        public static bool IsSameYearAs(this DateTime referenceDate, DateTime? otherDate) {
            return otherDate.HasValue && referenceDate.Date.Year == otherDate.Value.Date.Year;
        }

        /// <summary>
        ///     Check if two dates are in the same year ignoring time
        /// </summary>
        public static bool IsSameYearAs(this DateTime? referenceDate, DateTime? otherDate) {
            return referenceDate.HasValue && referenceDate.Value.IsSameYearAs(otherDate);
        }

        #endregion

        #region IsAtLeastADayBefore

        /// <summary>
        ///    Check if referenceDate is at least the day before the otherDate  ignoring time
        /// </summary>
        public static bool IsAtLeastOneDayBefore(this DateTime referenceDate, DateTime? otherDate) {
            return otherDate.HasValue && referenceDate.Date < otherDate.Value.Date;
        }

        [Obsolete("FAULTY IMPLEMENTATION. Use IsAtLeastOneDayBefore, but note that behaviour is different.")]
        public static bool IsAtLeastADayBefore(this DateTime referenceDate, DateTime? otherDate) {
            return otherDate.HasValue && otherDate.Value.Date < referenceDate.Date;
        }

        /// <summary>
        ///     Check if referenceDate is at least the day before the otherDate  ignoring time
        /// </summary>
        public static bool IsAtLeastOneDayBefore(this DateTime? referenceDate, DateTime? otherDate) {
            return referenceDate.HasValue && referenceDate.Value.IsAtLeastOneDayBefore(otherDate);
        }

        [Obsolete("FAULTY IMPLEMENTATION. Use IsAtLeastOneDayBefore, but note that behaviour is different.")]
        public static bool IsAtLeastADayBefore(this DateTime? referenceDate, DateTime? otherDate) {
#pragma warning disable 612, 618
            return referenceDate.HasValue && referenceDate.Value.IsAtLeastADayBefore(otherDate);
#pragma warning restore 612, 618
        }

        #endregion

        #region IsAtLeastADayAfter

        /// <summary>
        ///     Check if referanceDate is at least one day after the otherDate ignoring time
        /// </summary>
        public static bool IsAtLeastOneDayAfter(this DateTime referenceDate, DateTime? otherDate) {
            return otherDate.HasValue && referenceDate.Date > otherDate.Value.Date;
        }

        [Obsolete("FAULTY IMPLEMENTATION. Use IsAtLeastOneDayAfter, but note that behaviour is different.")]
        public static bool IsAtLeastADayAfter(this DateTime referenceDate, DateTime? otherDate) {
            return otherDate.HasValue && otherDate.Value.Date > referenceDate.Date;
        }

        /// <summary>
        ///     Check if referenceDate is at least one day after the otherDate ignoring time
        /// </summary>
        public static bool IsAtLeastOneDayAfter(this DateTime? referenceDate, DateTime? otherDate) {
            return referenceDate.HasValue && referenceDate.Value.IsAtLeastOneDayAfter(otherDate);
        }

        [Obsolete("FAULTY IMPLEMENTATION. Use IsAtLeastOneDayAfter, but note that behaviour is different.")]
        public static bool IsAtLeastADayAfter(this DateTime? referenceDate, DateTime? otherDate) {
#pragma warning disable 612, 618
            return referenceDate.HasValue && referenceDate.Value.IsAtLeastADayAfter(otherDate);
#pragma warning restore 612, 618
        }

        #endregion
    }
}