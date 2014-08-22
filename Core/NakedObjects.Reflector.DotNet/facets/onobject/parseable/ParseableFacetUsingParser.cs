// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Parseable;
using NakedObjects.Architecture.Persist;
using NakedObjects.Capabilities;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Parseable {
    public class ParseableFacetUsingParser<T> : FacetAbstract, IParseableFacet {
        private readonly IParser<T> parser;

        public ParseableFacetUsingParser(IParser<T> parser, IFacetHolder holder)
            : base(typeof (IParseableFacet), holder) {
            this.parser = parser;
        }

        #region IParseableFacet Members

        public INakedObject ParseTextEntry(string entry, INakedObjectManager manager) {
            if (entry == null) {
                throw new ArgumentException(Resources.NakedObjects.MissingEntryError);
            }
            object parsed = parser.ParseTextEntry(entry);
            return manager.CreateAdapter(parsed, null, null);
        }

        public INakedObject ParseInvariant(string text, INakedObjectManager manager) {
            object parsed = parser.ParseInvariant(text);
            return manager.CreateAdapter(parsed, null, null);
        }

        public string ParseableTitle(INakedObject nakedObject) {
            var context = nakedObject.GetDomainObject<T>();
            return parser.EditableTitleOf(context);
        }

        public string InvariantString(INakedObject nakedObject) {
            var context = nakedObject.GetDomainObject<T>();
            return parser.InvariantString(context);
        }

        public bool IsValid {
            get { return parser != null; }
        }

        #endregion

        protected override string ToStringValues() {
            return parser.ToString();
        }
    }
}