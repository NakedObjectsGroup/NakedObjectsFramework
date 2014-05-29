// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Adapter.Value;
using NakedObjects.Architecture.Facets;
using NakedObjects.Capabilities;
using NakedObjects.Core.Persist;

namespace NakedObjects.Reflector.DotNet.Value {
    public class DateTimeValueSemanticsProvider : ValueSemanticsProviderAbstract<DateTime>, IDateValueFacet {
        private const bool equalByContent = false;
        private const bool immutable = false;
        private const int typicalLength = 18;
        private static readonly DateTime defaultValue = new DateTime();

        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public DateTimeValueSemanticsProvider()
            : this(null) {}

        public DateTimeValueSemanticsProvider(IFacetHolder holder)
            : base(Type, holder, AdaptedType, typicalLength, immutable, equalByContent, defaultValue) {}

        // inject for testing 
        public static DateTime? TestDateTime { get; set; }

        public static Type Type {
            get { return typeof (IDateValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (DateTime); }
        }

        #region IDateValueFacet Members

        public INakedObject CreateValue(DateTime date) {
            return PersistorUtils.CreateAdapter(SetDate(date));
        }

        public DateTime DateValue(INakedObject nakedObject) {
            return nakedObject == null ? Now() : DateValue((DateTime) nakedObject.Object);
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == typeof (DateTime);
        }

        public DateTime DateValue(DateTime date) {
            return date;
        }

        protected override string DoEncode(DateTime obj) {
            DateTime date = DateValue(obj);
            return date.ToString("s");
        }

        protected override DateTime DoParse(DateTime original, string entry) {
            string dateString = entry.Trim();
            string str = dateString.ToLower();
            if (str.Equals("today") || str.Equals("now")) {
                return Now();
            }
            if (dateString.StartsWith("+")) {
                return RelativeDate(original, dateString, true);
            }
            if (dateString.StartsWith("-")) {
                return RelativeDate(original, dateString, false);
            }
            return ParseDate(dateString);
        }

        protected override DateTime DoRestore(string data) {
            return SetDate(DateTime.Parse(data));
        }

        protected override string TitleStringWithMask(string mask, DateTime value) {
            return (value).ToString(mask);
        }

        private static DateTime ParseDate(string dateString) {
            try {
                return SetDate(DateTime.Parse(dateString));
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(dateString));
            }
        }

        private static DateTime RelativeDate(DateTime original, string str, bool add) {
            if (str.Equals("")) {
                return Now();
            }

            DateTime date = original;
            string[] st = str.Substring(1).Split();
            foreach (string token in st) {
                date = RelativeDate2(date, token, add);
            }
            return date;
        }

        private static int ParseIncrement(string str) {
            return int.Parse(str.Substring(0, str.Length - 1));
        }

        private static DateTime RelativeDate2(DateTime original, string str, bool addDate) {
            if (str.Length == 0) {
                throw new InvalidEntryException(Resources.NakedObjects.IncompleteEntry);
            }

            int hours = 0;
            int minutes = 0;
            int days = 0;
            int months = 0;
            int years = 0;

            if (str.EndsWith("H")) {
                hours = ParseIncrement(str);
            }
            else if (str.EndsWith("M")) {
                minutes = ParseIncrement(str);
            }
            else if (str.EndsWith("w")) {
                days = 7*ParseIncrement(str);
            }
            else if (str.EndsWith("y")) {
                years = ParseIncrement(str);
            }
            else if (str.EndsWith("m")) {
                months = ParseIncrement(str);
            }
            else if (str.EndsWith("d")) {
                days = ParseIncrement(str);
            }
            else {
                days = ParseIncrement(str + "d");
            }

            if (addDate) {
                return Add(original, years, months, days, hours, minutes);
            }
            else {
                return Add(original, -years, -months, -days, -hours, -minutes);
            }
        }

        protected static DateTime SetDate(DateTime date) {
            return new DateTime(date.Ticks);
        }

        protected static DateTime Now() {
            return TestDateTime.HasValue ? TestDateTime.Value : DateTime.Now;
        }

        protected static DateTime Add(DateTime original, int years, int months, int days, int hours, int minutes) {
            DateTime date = original;

            try {
                date = date.AddYears(years);
                date = date.AddMonths(months);
                date = date.AddDays(days);
                date = date.AddHours(hours);
                date = date.AddMinutes(minutes);

                return date;
            }
            catch (ArgumentOutOfRangeException) {
                throw new InvalidEntryException(Resources.NakedObjects.InvalidDifference);
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}