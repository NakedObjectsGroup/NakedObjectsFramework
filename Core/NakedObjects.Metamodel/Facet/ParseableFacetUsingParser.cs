// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;
using NakedObjects.Meta.SemanticsProvider;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class ParseableFacetUsingParser<T> : FacetAbstract, IParseableFacet {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ParseableFacetUsingParser<T>));

        private readonly IValueSemanticsProvider<T> parser;

        public ParseableFacetUsingParser(IValueSemanticsProvider<T> parser, ISpecification holder)
            : base(typeof(IParseableFacet), holder) =>
            this.parser = parser;

        #region IParseableFacet Members

        public INakedObjectAdapter ParseTextEntry(string entry, INakedObjectManager manager) {
            if (entry == null) {
                throw new ArgumentException(Log.LogAndReturn(Resources.NakedObjects.MissingEntryError));
            }

            var parsed = parser.ParseTextEntry(entry);
            return manager.CreateAdapter(parsed, null, null);
        }

        public INakedObjectAdapter ParseInvariant(string text, INakedObjectManager manager) {
            var parsed = parser.ParseInvariant(text);
            return manager.CreateAdapter(parsed, null, null);
        }

        public string ParseableTitle(INakedObjectAdapter nakedObjectAdapter) {
            var context = nakedObjectAdapter.GetDomainObject<T>();
            return parser.EditableTitleOf(context);
        }

        public string InvariantString(INakedObjectAdapter nakedObjectAdapter) {
            var context = nakedObjectAdapter.GetDomainObject<T>();
            return parser.InvariantString(context);
        }

        #endregion

        protected override string ToStringValues() => parser.ToString();
    }
}