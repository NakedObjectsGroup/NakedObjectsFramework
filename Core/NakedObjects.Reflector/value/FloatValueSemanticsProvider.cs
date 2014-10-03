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
    public class FloatValueSemanticsProvider : ValueSemanticsProviderAbstract<float>, IPropertyDefaultFacet {
        private const float defaultValue = 0;
        private const bool equalByContent = true;
        private const bool immutable = true;
        private const int typicalLenth = 12;

        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public FloatValueSemanticsProvider(IIntrospectableSpecification spec)
            : this(spec, null) { }

        public FloatValueSemanticsProvider(IIntrospectableSpecification spec, IFacetHolder holder)
            : base(Type, holder, AdaptedType, typicalLenth, immutable, equalByContent, defaultValue, spec) { }

        public static Type Type {
            get { return typeof (IFloatingPointValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (float); }
        }

        #region IPropertyDefaultFacet Members

        public object GetDefault(INakedObject inObject) {
            return defaultValue;
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == typeof (float);
        }


        protected override float DoParse(string entry) {
            try {
                return float.Parse(entry);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (OverflowException) {
                throw new InvalidEntryException(OutOfRangeMessage(entry, float.MinValue, float.MaxValue));
            }
        }

        protected override float DoParseInvariant(string entry) {
            return float.Parse(entry, CultureInfo.InvariantCulture);
        }

        protected override string GetInvariantString(float obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        protected override string TitleStringWithMask(string mask, float value) {
            return value.ToString(mask);
        }


        protected override string DoEncode(float obj) {
            return obj.ToString("G");
        }

        protected override float DoRestore(string data) {
            return float.Parse(data);
        }


        public float FloatValue(INakedObject nakedObject) {
            return nakedObject.GetDomainObject<float>();
        }

   

        public override string ToString() {
            return "FloatAdapter: ";
        }
    }
}