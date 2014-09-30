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
    public class IntValueSemanticsProvider : ValueSemanticsProviderAbstract<int>, IPropertyDefaultFacet {
        private const int defaultValue = 0;
        private const bool equalBycontent = true;
        private const bool immutable = true;
        private const int typicalLength = 11;


        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public IntValueSemanticsProvider(INakedObjectReflector reflector)
            : this(reflector, null) { }

        public IntValueSemanticsProvider(INakedObjectReflector reflector, IFacetHolder holder)
            : base(Type, holder, AdaptedType, typicalLength, immutable, equalBycontent, defaultValue, reflector) { }

        public static Type Type {
            get { return typeof (IIntegerValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (int); }
        }

        #region IPropertyDefaultFacet Members

        public object GetDefault(INakedObject inObject) {
            return defaultValue;
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == typeof (int);
        }

        protected override int DoParse(string entry) {
            try {
                return int.Parse(entry, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (OverflowException) {
                throw new InvalidEntryException(OutOfRangeMessage(entry, int.MinValue, int.MaxValue));
            }
        }

        protected override int DoParseInvariant(string entry) {
            return int.Parse(entry, CultureInfo.InvariantCulture);
        }

        protected override string GetInvariantString(int obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        protected override string TitleStringWithMask(string mask, int value) {
            return value.ToString(mask);
        }

        protected override string DoEncode(int obj) {
            return obj.ToString("G");
        }

        protected override int DoRestore(string data) {
            return int.Parse(data);
        }

        public int IntegerValue(INakedObject nakedObject) {
            return nakedObject.GetDomainObject<int>();
        }

    

        public override string ToString() {
            return "IntAdapter: ";
        }
    }
}