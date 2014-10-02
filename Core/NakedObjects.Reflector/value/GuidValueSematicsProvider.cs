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
    public class GuidValueSemanticsProvider : ValueSemanticsProviderAbstract<Guid>, IPropertyDefaultFacet {
        private const bool equalByContent = true;
        private const bool immutable = true;
        private const int typicalLength = 36;
        private static readonly Guid defaultValue = Guid.Empty;

        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public GuidValueSemanticsProvider(IMetadata metadata)
            : this(metadata, null) { }

        public GuidValueSemanticsProvider(IMetadata metadata, IFacetHolder holder)
            : base(Type, holder, AdaptedType, typicalLength, immutable, equalByContent, defaultValue, metadata) { }

        public static Type Type {
            get { return typeof (IGuidValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (Guid); }
        }

        #region IPropertyDefaultFacet Members

        public object GetDefault(INakedObject inObject) {
            return defaultValue;
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == typeof (Guid);
        }


        protected override Guid DoParse(string entry) {
            try {
                return new Guid(entry);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (OverflowException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
        }

        protected override Guid DoParseInvariant(string entry) {
            return Guid.Parse(entry);
        }

        protected override string GetInvariantString(Guid obj) {
            return obj.ToString();
        }

        protected override string TitleStringWithMask(string mask, Guid value) {
            return value.ToString(mask);
        }

        protected override string DoEncode(Guid obj) {
            return obj.ToString();
        }

        protected override Guid DoRestore(string data) {
            return new Guid(data);
        }

        public Guid GuidValue(INakedObject nakedObject) {
            return nakedObject.GetDomainObject<Guid>();
        }

  

        public override string ToString() {
            return "GuidAdapter: ";
        }
    }
}