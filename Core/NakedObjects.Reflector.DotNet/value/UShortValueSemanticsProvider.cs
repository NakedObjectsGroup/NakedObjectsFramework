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
using NakedObjects.Core.Context;
using NakedObjects.Core.Persist;

namespace NakedObjects.Reflector.DotNet.Value {
    public class UShortValueSemanticsProvider : ValueSemanticsProviderAbstract<ushort>, IPropertyDefaultFacet {
        private const ushort defaultValue = 0;
        private const bool equalByContent = true;
        private const bool immutable = true;
        private const int typicalLength = 5;

        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public UShortValueSemanticsProvider()
            : this(null) {}

        public UShortValueSemanticsProvider(IFacetHolder holder)
            : base(Type, holder, AdaptedType, typicalLength, immutable, equalByContent, defaultValue) {}

        public static Type Type {
            get { return typeof (IShortValueFacet); }
        }


        public static Type AdaptedType {
            get { return typeof (ushort); }
        }

        #region IPropertyDefaultFacet Members

        public object GetDefault(INakedObject inObject) {
            return defaultValue;
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == typeof (ushort);
        }


        protected override ushort DoParse(string entry) {
            try {
                return ushort.Parse(entry, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (OverflowException) {
                throw new InvalidEntryException(OutOfRangeMessage(entry, ushort.MinValue, ushort.MaxValue));
            }
        }

        protected override ushort DoParseInvariant(string entry) {
            return ushort.Parse(entry, CultureInfo.InvariantCulture);
        }

        protected override string GetInvariantString(ushort obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        protected override string TitleStringWithMask(string mask, ushort value) {
            return value.ToString(mask);
        }


        protected override string DoEncode(ushort obj) {
            return obj.ToString();
        }

        protected override ushort DoRestore(string data) {
            return ushort.Parse(data);
        }


        public INakedObject CreateValue(ushort value) {
            return NakedObjectsContext.ObjectPersistor.CreateAdapter(value, null, null);
        }

        public ushort UnsignedShortValue(INakedObject nakedObject) {
            return nakedObject.GetDomainObject<ushort>();
        }


        public override string ToString() {
            return "UShortAdapter: ";
        }
    }
}