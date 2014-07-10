// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Adapter.Value;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Properties.Defaults;
using NakedObjects.Capabilities;
using NakedObjects.Core.Persist;

namespace NakedObjects.Reflector.DotNet.Value {
    public class CharValueSemanticsProvider : ValueSemanticsProviderAbstract<char>, IPropertyDefaultFacet {
        private const char defaultValue = ' ';
        private const bool equalByContent = true;
        private const bool immutable = true;
        private const int typicalLength = 2;


        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public CharValueSemanticsProvider()
            : this(null) {}

        public CharValueSemanticsProvider(IFacetHolder holder)
            : base(Type, holder, AdaptedType, typicalLength, immutable, equalByContent, defaultValue) {}

        public static Type Type {
            get { return typeof (ICharValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (char); }
        }

        #region IPropertyDefaultFacet Members

        public object GetDefault(INakedObject inObject) {
            return defaultValue;
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == AdaptedType;
        }

        public override object ParseTextEntry(string entry) {
            if (entry == null) {
                throw new ArgumentException();
            }
            if (entry.Equals("")) {
                return null;
            }
            return DoParse(entry);
        }

        protected override char DoParse(string entry) {
            try {
                return char.Parse(entry);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
        }

        protected override string TitleStringWithMask(string mask, char value) {
            return value.ToString();
        }


        protected override string DoEncode(char obj) {
            return obj.ToString();
        }

        protected override char DoRestore(string data) {
            return char.Parse(data);
        }


        public char CharValue(INakedObject nakedObject) {
            return nakedObject.GetDomainObject<char>();
        }

        public INakedObject CreateValue(char value) {
            return PersistorUtils.CreateAdapter(value);
        }


        public override string ToString() {
            return "CharAdapter: ";
        }
    }
}