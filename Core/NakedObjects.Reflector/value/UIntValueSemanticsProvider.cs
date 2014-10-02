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
    public class UIntValueSemanticsProvider : ValueSemanticsProviderAbstract<uint>, IPropertyDefaultFacet {
        private const uint defaultValue = 0;
        private const bool equalByContent = true;
        private const bool immutable = true;
        private const int typicalLength = 10;


        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public UIntValueSemanticsProvider(IMetadata metadata)
            : this(metadata, null) {}

        public UIntValueSemanticsProvider(IMetadata metadata, IFacetHolder holder)
            : base(Type, holder, AdaptedType, typicalLength, immutable, equalByContent, defaultValue, metadata) { }

        public static Type Type {
            get { return typeof (IUnsignedIntegerValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (uint); }
        }

        #region IPropertyDefaultFacet Members

        public object GetDefault(INakedObject inObject) {
            return defaultValue;
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == typeof (uint);
        }

        protected override uint DoParse(string entry) {
            try {
                return uint.Parse(entry, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (OverflowException) {
                throw new InvalidEntryException(OutOfRangeMessage(entry, uint.MinValue, uint.MaxValue));
            }
        }

        protected override uint DoParseInvariant(string entry) {
            return uint.Parse(entry, CultureInfo.InvariantCulture);
        }

        protected override string GetInvariantString(uint obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        protected override string TitleStringWithMask(string mask, uint value) {
            return value.ToString(mask);
        }

        protected override string DoEncode(uint obj) {
            return obj.ToString();
        }

        protected override uint DoRestore(string data) {
            return uint.Parse(data);
        }

        public uint UnsignedIntegerValue(INakedObject nakedObject) {
            return nakedObject.GetDomainObject<uint>();
        }

  

        public override string ToString() {
            return "UIntAdapter: ";
        }
    }
}