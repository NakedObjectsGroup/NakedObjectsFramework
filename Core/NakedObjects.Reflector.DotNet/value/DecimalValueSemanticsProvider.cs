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
    public class DecimalValueSemanticsProvider : ValueSemanticsProviderAbstract<decimal>, IPropertyDefaultFacet {
        private const decimal defaultValue = 0;
        private const bool equalByContent = true;
        private const bool immutable = true;
        private const int typicalLength = 18;

        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public DecimalValueSemanticsProvider(INakedObjectReflector reflector)
            : this(reflector, null) {}

        public DecimalValueSemanticsProvider(INakedObjectReflector reflector, IFacetHolder holder)
            : base(Type, holder, AdaptedType, typicalLength, immutable, equalByContent, defaultValue, reflector) { }

        public static Type Type {
            get { return typeof (IDecimalValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (decimal); }
        }

        #region IPropertyDefaultFacet Members

        public object GetDefault(INakedObject inObject) {
            return defaultValue;
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == typeof (decimal);
        }


        protected override decimal DoParse(string entry) {
            try {
                return decimal.Parse(entry, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (OverflowException) {
                throw new InvalidEntryException(OutOfRangeMessage(entry, decimal.MinValue, decimal.MaxValue));
            }
        }

        protected override decimal DoParseInvariant(string entry) {
            return decimal.Parse(entry, CultureInfo.InvariantCulture);
        }

        protected override string GetInvariantString(decimal obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        protected override string TitleStringWithMask(string mask, decimal value) {
            return value.ToString(mask);
        }


        protected override string DoEncode(decimal obj) {
            return obj.ToString();
        }

        protected override decimal DoRestore(string data) {
            return decimal.Parse(data);
        }


        public decimal DecimalValue(INakedObject nakedObject) {
            return nakedObject.GetDomainObject<decimal>();
        }

  

        public override string ToString() {
            return "DecimalAdapter: ";
        }
    }
}