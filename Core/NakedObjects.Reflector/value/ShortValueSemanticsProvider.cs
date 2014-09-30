// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Globalization;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Adapter.Value;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Properties.Defaults;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Capabilities;
using NakedObjects.Core.Context;
using NakedObjects.Core.Persist;

namespace NakedObjects.Reflector.DotNet.Value {
    public class ShortValueSemanticsProvider : ValueSemanticsProviderAbstract<short>, IPropertyDefaultFacet {
        private const short defaultValue = 0;
        private const bool equalByContent = true;
        private const bool immutable = true;
        private const int typicalLength = 6;

        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public ShortValueSemanticsProvider(INakedObjectReflector reflector)
            : this(reflector, null) {}

        public ShortValueSemanticsProvider(INakedObjectReflector reflector, IFacetHolder holder)
            : base(Type, holder, AdaptedType, typicalLength, immutable, equalByContent, defaultValue, reflector) { }

        public static Type Type {
            get { return typeof (IShortValueFacet); }
        }


        public static Type AdaptedType {
            get { return typeof (short); }
        }

        #region IPropertyDefaultFacet Members

        public object GetDefault(INakedObject inObject) {
            return defaultValue;
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == typeof (short);
        }


        protected override short DoParse(string entry) {
            try {
                return short.Parse(entry, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (OverflowException) {
                throw new InvalidEntryException(OutOfRangeMessage(entry, short.MinValue, short.MaxValue));
            }
        }

        protected override short DoParseInvariant(string entry) {
            return short.Parse(entry, CultureInfo.InvariantCulture);
        }

        protected override string GetInvariantString(short obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        protected override string TitleStringWithMask(string mask, short value) {
            return value.ToString(mask);
        }


        protected override string DoEncode(short obj) {
            return obj.ToString("G");
        }

        protected override short DoRestore(string data) {
            return short.Parse(data);
        }


   

        public short ShortValue(INakedObject nakedObject) {
            return nakedObject.GetDomainObject<short>();
        }


        public override string ToString() {
            return "ShortAdapter: ";
        }
    }
}