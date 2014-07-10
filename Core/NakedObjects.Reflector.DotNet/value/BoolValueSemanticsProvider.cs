// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Globalization;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Adapter.Value;
using NakedObjects.Architecture.Facets;
using NakedObjects.Capabilities;
using NakedObjects.Core.Util;

namespace NakedObjects.Reflector.DotNet.Value {
    public class BooleanValueSemanticsProvider : ValueSemanticsProviderAbstract<bool>, IBooleanValueFacet {
        private const bool defaultValue = false;
        private const bool equalByContent = true;
        private const bool immutable = true;
        private const int typicalLength = 5;

        public BooleanValueSemanticsProvider(IFacetHolder holder)
            : base(Type, holder, AdaptedType, typicalLength, immutable, equalByContent, defaultValue) {}

        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public BooleanValueSemanticsProvider()
            : this(null) {}

        private static Type Type {
            get { return typeof (IBooleanValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (bool); }
        }

        #region IBooleanValueFacet Members

        public bool IsSet(INakedObject nakedObject) {
            if (!nakedObject.Exists()) {
                return false;
            }
            return nakedObject.GetDomainObject<bool>();
        }


        public void Reset(INakedObject nakedObject) {
            nakedObject.ReplacePoco(false);
        }

        public void Set(INakedObject nakedObject) {
            nakedObject.ReplacePoco(true);
        }

        public void Toggle(INakedObject nakedObject) {
            bool newValue = !(bool) nakedObject.Object;
            nakedObject.ReplacePoco(newValue);
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == AdaptedType;
        }


        protected override bool DoParse(string entry) {
            if ("true".StartsWith(entry.ToLower())) {
                return true;
            }
            if ("false".StartsWith(entry.ToLower())) {
                return false;
            }
            throw new InvalidEntryException(string.Format(Resources.NakedObjects.NotALogical, entry));
        }

        protected override bool DoParseInvariant(string entry) {
            return bool.Parse(entry);
        }

        protected override string DoEncode(bool obj) {
            return obj ? "T" : "F";
        }

        protected override bool DoRestore(string data) {
            if (data.Length != 1) {
                throw new InvalidDataException(string.Format(Resources.NakedObjects.InvalidLogicalLength, data.Length));
            }
            switch (data[0]) {
                case 'T':
                    return true;
                case 'F':
                    return false;
                default:
                    throw new InvalidDataException(string.Format(Resources.NakedObjects.InvalidLogicalType, data[0]));
            }
        }
    }
}