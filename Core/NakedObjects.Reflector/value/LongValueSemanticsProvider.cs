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
using NakedObjects.Reflector.Spec;

namespace NakedObjects.Reflector.DotNet.Value {
    public class LongValueSemanticsProvider : ValueSemanticsProviderAbstract<long>, IPropertyDefaultFacet {
        private const bool equalByContent = true;
        private const bool immutable = true;
        private const int typicalLength = 20;
        private const long defaultValue = 0;

        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public LongValueSemanticsProvider(IObjectSpecImmutable spec)
            : this(spec, null) { }

        public LongValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, typicalLength, immutable, equalByContent, defaultValue, spec) { }

        public static Type Type {
            get { return typeof (ILongValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (long); }
        }

        #region IPropertyDefaultFacet Members

        public object GetDefault(INakedObject inObject) {
            return defaultValue;
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == typeof (long);
        }


        protected override long DoParse(string entry) {
            try {
                return long.Parse(entry, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (OverflowException) {
                throw new InvalidEntryException(OutOfRangeMessage(entry, long.MinValue, long.MaxValue));
            }
        }

        protected override long DoParseInvariant(string entry) {
            return long.Parse(entry, CultureInfo.InvariantCulture);
        }

        protected override string GetInvariantString(long obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        protected override string TitleStringWithMask(string mask, long value) {
            return value.ToString(mask);
        }


        protected override string DoEncode(long obj) {
            return obj.ToString("G");
        }

        protected override long DoRestore(string data) {
            return long.Parse(data);
        }


        public long LongValue(INakedObject nakedObject) {
            return nakedObject.GetDomainObject<long>();
        }


        public override string ToString() {
            return "LongAdapter: ";
        }
    }
}