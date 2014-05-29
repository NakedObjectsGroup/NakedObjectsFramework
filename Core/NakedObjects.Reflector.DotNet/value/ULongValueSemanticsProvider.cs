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
    public class ULongValueSemanticsProvider : ValueSemanticsProviderAbstract<ulong>, IPropertyDefaultFacet {
        private const ulong defaultValue = 0;
        private const bool equalByContent = true;
        private const bool immutable = true;
        private const int typicalLength = 20;

        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public ULongValueSemanticsProvider()
            : this(null) {}

        public ULongValueSemanticsProvider(IFacetHolder holder)
            : base(Type, holder, AdaptedType, typicalLength, immutable, equalByContent, defaultValue) {}

        public static Type Type {
            get { return typeof (IUnsignedLongValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (ulong); }
        }

        #region IPropertyDefaultFacet Members

        public object GetDefault(INakedObject inObject) {
            return defaultValue;
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == typeof (ulong);
        }


        protected override ulong DoParse(ulong original, string entry) {
            try {
                return ulong.Parse(entry, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (OverflowException) {
                throw new InvalidEntryException(OutOfRangeMessage(entry, ulong.MinValue, ulong.MaxValue));
            }
        }

        protected override string TitleStringWithMask(string mask, ulong value) {
            return value.ToString(mask);
        }


        protected override string DoEncode(ulong obj) {
            return obj.ToString();
        }

        protected override ulong DoRestore(string data) {
            return ulong.Parse(data);
        }


        public ulong UnsignedLongValue(INakedObject nakedObject) {
            return nakedObject.GetDomainObject<ulong>();
        }

        public INakedObject CreateValue(ulong value) {
            return PersistorUtils.CreateAdapter(value);
        }


        public override string ToString() {
            return "ULongAdapter: ";
        }
    }
}