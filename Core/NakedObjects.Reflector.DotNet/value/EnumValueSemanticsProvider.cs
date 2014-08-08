// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Adapter.Value;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Properties.Defaults;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Capabilities;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Value {
    public class EnumValueSemanticsProvider<T> : ValueSemanticsProviderAbstract<T>, IPropertyDefaultFacet, IEnumValueFacet {
        private const bool EqualBycontent = true;
        private const bool Immutable = true;
        private const int TypicalLengthConst = 11;

        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public EnumValueSemanticsProvider(INakedObjectReflector reflector)
            : this(reflector, null) { }

        public EnumValueSemanticsProvider(INakedObjectReflector reflector, IFacetHolder holder)
            : base(Type, holder, AdaptedType, TypicalLengthConst, Immutable, EqualBycontent, default(T), reflector) { }

        public static Type Type {
            get { return typeof (IEnumValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (T); }
        }

        #region IEnumValueFacet Members

        public string IntegralValue(INakedObject nakedObject) {
            if (nakedObject.Object is T || TypeUtils.IsIntegralValueForEnum(nakedObject.Object)) {
                return Convert.ChangeType(nakedObject.Object, Enum.GetUnderlyingType(typeof (T))).ToString();
            }
            return null;
        }

        #endregion

        #region IPropertyDefaultFacet Members

        public object GetDefault(INakedObject inObject) {
            return default(T);
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == AdaptedType;
        }

        protected override T DoParse(string entry) {
            try {
                return (T) Enum.Parse(typeof (T), entry);
            }
            catch (ArgumentException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (OverflowException oe) {
                throw new InvalidEntryException(oe.Message);
            }
        }

        protected override T DoParseInvariant(string entry) {
            return (T)Enum.Parse(typeof(T), entry);
        }

        protected override string GetInvariantString(T obj) {
            return obj.ToString();
        }

        protected override string TitleString(T obj) {
            return NameUtils.NaturalName(obj.ToString());
        }

        protected override string TitleStringWithMask(string mask, T value) {
            return TitleString(value);
        }

        protected override string DoEncode(T obj) {
            return obj.GetType().FullName + ":" + obj;
        }

        protected override T DoRestore(string data) {
            string[] typeAndValue = data.Split(':');
            return (T) Enum.Parse(TypeUtils.GetType(typeAndValue[0]), typeAndValue[1]);
        }


        public override string ToString() {
            return "EnumAdapter: ";
        }
    }
}