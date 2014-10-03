// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Globalization;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Adapter.Value;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Context;
using NakedObjects.Core.Persist;
using NakedObjects.Reflector.Spec;

namespace NakedObjects.Reflector.DotNet.Value {
    public class TimeValueSemanticsProvider : ValueSemanticsProviderAbstract<TimeSpan>, ITimeValueFacet {
        private const bool EqualByContent = false;
        private const bool Immutable = false;
        private const int typicalLength = 6;
        private static readonly TimeSpan defaultValue = new TimeSpan();

        public TimeValueSemanticsProvider(IIntrospectableSpecification spec, IFacetHolder holder)
            : base(Type, holder, AdaptedType, typicalLength, Immutable, EqualByContent, defaultValue, spec) { }

        public static Type Type {
            get { return typeof (ITimeValueFacet); }
        }

        // inject for testing 
        public static DateTime? TestDateTime { get; set; }

        public static Type AdaptedType {
            get { return typeof (TimeSpan); }
        }

        #region ITimeValueFacet Members

    

        public TimeSpan TimeValue(INakedObject nakedObject) {
            return nakedObject.GetDomainObject<TimeSpan>();
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == typeof (TimeSpan);
        }

        protected override string DoEncode(TimeSpan time) {
            return time.ToString();
        }

        protected override TimeSpan DoParse(string entry) {
            string dateString = entry.Trim();
            try {
                return DateTime.Parse(dateString).TimeOfDay;
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(dateString));
            }
        }

        protected override TimeSpan DoParseInvariant(string entry) {
            return TimeSpan.Parse(entry, CultureInfo.InvariantCulture);
        }

        protected override string GetInvariantString(TimeSpan obj) {
            return obj.ToString(null, CultureInfo.InvariantCulture);
        }

        protected override TimeSpan DoRestore(string data) {
            return TimeSpan.Parse(data);
        }

        protected override string TitleString(TimeSpan obj) {
            return DateTime.Today.Add(obj).ToShortTimeString();
        }

        protected override string TitleStringWithMask(string mask, TimeSpan obj) {
            return DateTime.Today.Add(obj).ToString(mask);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}