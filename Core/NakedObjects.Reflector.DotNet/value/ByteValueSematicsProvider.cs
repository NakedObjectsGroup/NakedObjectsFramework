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
    public class ByteValueSemanticsProvider : ValueSemanticsProviderAbstract<byte>, IPropertyDefaultFacet, IByteValueFacet {
        private const byte defaultValue = 0;
        private const bool equalByContent = true;
        private const bool immutable = true;
        private const int typicalLength = 3;

        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public ByteValueSemanticsProvider()
            : this(null) {}

        public ByteValueSemanticsProvider(IFacetHolder holder)
            : base(Type, holder, AdaptedType, typicalLength, immutable, equalByContent, defaultValue) {}

        public static Type Type {
            get { return typeof (IByteValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (byte); }
        }

        #region IPropertyDefaultFacet Members

        public object GetDefault(INakedObject inObject) {
            return defaultValue;
        }

        #endregion

        public byte ByteValue(INakedObject nakedObject) {
            return nakedObject.GetDomainObject<byte>();
        }

        public INakedObject CreateValue(byte value) {
            return PersistorUtils.CreateAdapter(value);
        }

        public static bool IsAdaptedType(Type type) {
            return type == typeof (byte);
        }


        protected override byte DoParse(string entry) {
            try {
                return byte.Parse(entry);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (OverflowException) {
                throw new InvalidEntryException(OutOfRangeMessage(entry, byte.MinValue, byte.MaxValue));
            }
        }

        protected override byte DoParseInvariant(string entry) {
            return byte.Parse(entry, CultureInfo.InvariantCulture);
        }

        protected override string GetInvariantString(byte obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        protected override string TitleStringWithMask(string mask, byte value) {
            return value.ToString(mask);
        }

        protected override string DoEncode(byte obj) {
            return obj.ToString();
        }

        protected override byte DoRestore(string data) {
            return byte.Parse(data);
        }

        public override string ToString() {
            return "ByteAdapter: ";
        }
    }
}