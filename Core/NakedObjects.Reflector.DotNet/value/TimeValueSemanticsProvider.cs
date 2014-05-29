// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Adapter.Value;
using NakedObjects.Architecture.Facets;
using NakedObjects.Core.Persist;

namespace NakedObjects.Reflector.DotNet.Value {
    public class TimeValueSemanticsProvider : ValueSemanticsProviderAbstract<TimeSpan>, ITimeValueFacet {
        private const bool equalByContent = false;
        private const bool immutable = false;
        private const int typicalLength = 6;
        private static readonly TimeSpan defaultValue = new TimeSpan();

        public TimeValueSemanticsProvider(IFacetHolder holder)
            : base(Type, holder, AdaptedType, typicalLength, immutable, equalByContent, defaultValue) {}

        public static Type Type {
            get { return typeof (ITimeValueFacet); }
        }

        // inject for testing 
        public static DateTime? TestDateTime { get; set; }

        public static Type AdaptedType {
            get { return typeof (TimeSpan); }
        }

        public int Level {
            get { throw new NotImplementedException(); }
        }

        #region ITimeValueFacet Members

        public INakedObject CreateValue(TimeSpan time) {
            return PersistorUtils.CreateAdapter(SetTime(time));
        }

        public TimeSpan TimeValue(INakedObject nakedObject) {
            return nakedObject.GetDomainObject<TimeSpan>();
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == typeof (TimeSpan);
        }

        protected override string DoEncode(TimeSpan time) {
            return (time).ToString();
        }

        protected override TimeSpan DoParse(TimeSpan original, string entry) {
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
            return ParseTime(entry.Trim());
        }

        private static TimeSpan RelativeDate(TimeSpan original, string str, bool add) {
            if (str.Equals("")) {
                return Now();
            }

            TimeSpan time = original;
            string[] st = str.Substring(1).Split();
            foreach (string token in st) {
                time = RelativeDate2(time, token, add);
            }
            return time;
        }

        private static int ParseIncrement(string str) {
            return int.Parse(str.Substring(0, str.Length - 1));
        }

        private static TimeSpan RelativeDate2(TimeSpan original, string str, bool addDate) {
            int hours = 0;
            int minutes = 0;


            if (str.EndsWith("H")) {
                hours = ParseIncrement(str);
            }
            else if (str.EndsWith("M")) {
                minutes = ParseIncrement(str);
            }
            else {
                return original;
            }

            if (addDate) {
                return Add(original, hours, minutes);
            }
            return Add(original, -hours, -minutes);
        }


        private static TimeSpan ParseTime(string dateString) {
            try {
                return SetTime(DateTime.Parse(dateString).TimeOfDay);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(dateString));
            }
        }


        protected override TimeSpan DoRestore(string data) {
            return SetTime(TimeSpan.Parse(data));
        }

        protected override string TitleString(TimeSpan obj) {
            TimeSpan? time = obj;
            return time == null ? "" : DateTime.Today.Add((TimeSpan) time).ToShortTimeString();
        }

        protected override string TitleStringWithMask(string mask, TimeSpan obj) {
            TimeSpan? time = (obj);
            return time == null ? "" : DateTime.Today.Add((TimeSpan) time).ToString(mask);
        }

        protected static TimeSpan SetTime(TimeSpan time) {
            return time;
        }

        protected static TimeSpan Now() {
            return TestDateTime.HasValue ? TestDateTime.Value.TimeOfDay : DateTime.Now.TimeOfDay;
        }

        protected static TimeSpan Add(TimeSpan original, int hours, int minutes) {
            // new to lose any day wrapping 
            TimeSpan newTime = original.Add(new TimeSpan(hours, minutes, 0));
            return new TimeSpan(0, newTime.Hours, newTime.Minutes, newTime.Seconds);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}