// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Adapter.Value;
using NakedObjects.Architecture.Facets;
using NakedObjects.Capabilities;
using NakedObjects.Core.Persist;

namespace NakedObjects.Reflector.DotNet.Value {
    public class StringValueSemanticsProvider : ValueSemanticsProviderAbstract<string>, IStringValueFacet {
        private const string defaultValue = null;
        private const bool equalByContent = true;
        private const bool immutable = true;
        private const int typicalLength = 25;

        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public StringValueSemanticsProvider()
            : this(null) {}

        public StringValueSemanticsProvider(IFacetHolder holder)
            : base(Type, holder, AdaptedType, typicalLength, immutable, equalByContent, defaultValue) {}

        public static Type Type {
            get { return typeof (IStringValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (string); }
        }

        #region IStringValueFacet Members

        public INakedObject CreateValue(string value) {
            return PersistorUtils.CreateAdapter(value);
        }

        public string StringValue(INakedObject nakedObject) {
            return nakedObject.GetDomainObject<string>();
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == typeof (string);
        }


        protected override string DoParse(string original, string entry) {
            if (entry.Trim().Equals("")) {
                return null;
            }
            return entry;
        }


        protected override string DoEncode(string obj) {
            string text = obj;
            if (text.Equals("NULL") || IsEscaped(text)) {
                return EscapeText(text);
            }
            return text;
        }

        protected override string DoRestore(string data) {
            if (IsEscaped(data)) {
                return data.Substring(1);
            }
            return data;
        }

        private static bool IsEscaped(string text) {
            return text.StartsWith("/");
        }

        private static string EscapeText(string text) {
            return "/" + text;
        }
    }
}