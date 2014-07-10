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
using NakedObjects.Capabilities;
using NakedObjects.Core.Persist;

namespace NakedObjects.Reflector.DotNet.Value {
    public class SbyteValueSemanticsProvider : ValueSemanticsProviderAbstract<sbyte>, IPropertyDefaultFacet {
        private const sbyte defaultValue = 0;
        private const bool equalByContent = true;
        private const bool immutable = true;
        private const int typicalLength = 3; // include sign 


        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public SbyteValueSemanticsProvider()
            : this(null) {}

        public SbyteValueSemanticsProvider(IFacetHolder holder)
            : base(Type, holder, AdaptedType, typicalLength, immutable, equalByContent, defaultValue) {}

        public static Type Type {
            get { return typeof (ISbyteValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (sbyte); }
        }

        #region IPropertyDefaultFacet Members

        public object GetDefault(INakedObject inObject) {
            return defaultValue;
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == typeof (sbyte);
        }


        protected override sbyte DoParse(string entry) {
            try {
                return sbyte.Parse(entry);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (OverflowException) {
                throw new InvalidEntryException(OutOfRangeMessage(entry, sbyte.MinValue, sbyte.MaxValue));
            }
        }

        protected override sbyte DoParseInvariant(string entry) {
            return sbyte.Parse(entry, CultureInfo.InvariantCulture);
        }

        protected override string GetInvariantString(sbyte obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        protected override string TitleStringWithMask(string mask, sbyte value) {
            return value.ToString(mask);
        }


        protected override string DoEncode(sbyte obj) {
            return obj.ToString("G");
        }

        protected override sbyte DoRestore(string data) {
            return sbyte.Parse(data);
        }


        public sbyte ByteValue(INakedObject nakedObject) {
            return nakedObject.GetDomainObject<sbyte>();
        }

        public INakedObject CreateValue(sbyte value) {
            return PersistorUtils.CreateAdapter(value);
        }


        public override string ToString() {
            return "SByteAdapter: ";
        }
    }
}