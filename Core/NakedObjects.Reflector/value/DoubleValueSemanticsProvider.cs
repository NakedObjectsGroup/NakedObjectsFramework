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
    public class DoubleValueSemanticsProvider : ValueSemanticsProviderAbstract<double>, IPropertyDefaultFacet {
        private const double defaultValue = 0;
        private const bool equalByContent = true;
        private const bool immutable = true;
        private const int typicalLenth = 22;

        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public DoubleValueSemanticsProvider(IObjectSpecImmutable spec)
            : this(spec, null) { }

        public DoubleValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, typicalLenth, immutable, equalByContent, defaultValue, spec) { }

        private static Type Type {
            get { return typeof (IDoubleFloatingPointValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (double); }
        }

        #region IPropertyDefaultFacet Members

        public object GetDefault(INakedObject inObject) {
            return defaultValue;
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == typeof (double);
        }


        protected override double DoParse(string entry) {
            try {
                return double.Parse(entry);
            }
            catch (FormatException) {
                throw new InvalidEntryException(Resources.NakedObjects.NotANumber);
            }
            catch (OverflowException) {
                throw new InvalidEntryException(OutOfRangeMessage(entry, double.MinValue, double.MaxValue));
            }
        }

        protected override double DoParseInvariant(string entry) {
            return double.Parse(entry, CultureInfo.InvariantCulture);
        }

        protected override string GetInvariantString(double obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        protected override string TitleStringWithMask(string mask, double value) {
            return value.ToString(mask);
        }

        protected override string DoEncode(double obj) {
            return obj.ToString("G");
        }

        protected override double DoRestore(string data) {
            return double.Parse(data);
        }

        public Double DoubleValue(INakedObject nakedObject) {
            return nakedObject.GetDomainObject<double>();
        }

 

        public override string ToString() {
            return "DoubleAdapter: ";
        }
    }
}