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
using NakedObjects.Capabilities;

namespace NakedObjects.Reflector.DotNet.Value {
    public class DateTimeValueSemanticsProvider : ValueSemanticsProviderAbstract<DateTime>, IDateValueFacet {
        private const bool EqualByContent = false;
        private const bool Immutable = false;
        private const int typicalLength = 18;
        private static readonly DateTime defaultValue = new DateTime();

        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public DateTimeValueSemanticsProvider(INakedObjectReflector reflector)
            : this(reflector, null) { }

        public DateTimeValueSemanticsProvider(INakedObjectReflector reflector, IFacetHolder holder)
            : base(Type, holder, AdaptedType, typicalLength, Immutable, EqualByContent, defaultValue, reflector) { }

        // inject for testing 
        public static DateTime? TestDateTime { get; set; }

        public static Type Type {
            get { return typeof (IDateValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (DateTime); }
        }

        #region IDateValueFacet Members

  

        public DateTime DateValue(INakedObject nakedObject) {
            return nakedObject == null ? Now() : (DateTime) nakedObject.Object;
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == typeof (DateTime);
        }

        protected override string DoEncode(DateTime obj) {
            DateTime date = obj;
            return date.ToString("s");
        }

        protected override DateTime DoParse(string entry) {
            string dateString = entry.Trim();
            try {
                return DateTime.Parse(dateString);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(dateString));
            }
        }

        protected override DateTime DoParseInvariant(string entry) {
            return DateTime.Parse(entry, CultureInfo.InvariantCulture);
        }

        protected override string GetInvariantString(DateTime obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        protected override DateTime DoRestore(string data) {
            return DateTime.Parse(data);
        }

        protected override string TitleStringWithMask(string mask, DateTime value) {
            return value.ToString(mask);
        }

        protected static DateTime Now() {
            return TestDateTime.HasValue ? TestDateTime.Value : DateTime.Now;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}