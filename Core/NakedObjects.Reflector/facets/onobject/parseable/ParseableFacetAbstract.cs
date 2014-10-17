// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Spec;
using NakedObjects.Capabilities;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Parseable {
    public abstract class ParseableFacetAbstract<T> : FacetAbstract, IParseableFacet {
        // to delegate to
        private readonly ParseableFacetUsingParser<T> parseableFacetUsingParser;
        private readonly Type parserClass;

        protected ParseableFacetAbstract(string candidateParserName, Type candidateParserClass, ISpecification holder)
            : base(typeof (IParseableFacet), holder) {
            parserClass = ParserUtils.ParserOrNull<T>(candidateParserClass, candidateParserName);
            parseableFacetUsingParser = IsValid ? new ParseableFacetUsingParser<T>((IParser<T>) TypeUtils.NewInstance(parserClass), holder) : null;
        }

        /// <summary>
        ///     Discover whether either of the candidate parser name or class is valid.
        /// </summary>
        public bool IsValid {
            get { return parserClass != null; }
        }

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