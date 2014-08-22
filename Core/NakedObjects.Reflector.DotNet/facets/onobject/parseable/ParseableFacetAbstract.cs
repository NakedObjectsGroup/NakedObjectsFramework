// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Parseable;
using NakedObjects.Architecture.Persist;
using NakedObjects.Capabilities;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Parseable {
    public abstract class ParseableFacetAbstract<T> : FacetAbstract, IParseableFacet {
        // to delegate to
        private readonly ParseableFacetUsingParser<T> parseableFacetUsingParser;
        private readonly Type parserClass;

        protected ParseableFacetAbstract(string candidateParserName, Type candidateParserClass, IFacetHolder holder)
            : base(typeof (IParseableFacet), holder) {
            parserClass = ParserUtils.ParserOrNull<T>(candidateParserClass, candidateParserName);
            parseableFacetUsingParser = IsValid ? new ParseableFacetUsingParser<T>((IParser<T>) TypeUtils.NewInstance(parserClass), holder) : null;
        }

        #region IParseableFacet Members

        public INakedObject ParseTextEntry(string entryText, INakedObjectManager manager) {
            return parseableFacetUsingParser.ParseTextEntry(entryText, manager);
        }

        public INakedObject ParseInvariant(string text, INakedObjectManager manager) {
            return parseableFacetUsingParser.ParseInvariant(text, manager);
        }

        public string ParseableTitle(INakedObject nakedObject) {
            return parseableFacetUsingParser.ParseableTitle(nakedObject);
        }

        public string InvariantString(INakedObject nakedObject) {
            return parseableFacetUsingParser.InvariantString(nakedObject);
        }

        /// <summary>
        ///     Discover whether either of the candidate parser name or class is valid.
        /// </summary>
        public bool IsValid {
            get { return parserClass != null; }
        }

        #endregion

        /// <summary>
        ///     Guaranteed to implement the <see cref="IParser{T}" /> class, thanks to generics in the applib.
        /// </summary>
        public Type GetParserClass() {
            return parserClass;
        }

        protected override string ToStringValues() {
            return parserClass.FullName;
        }
    }
}