// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Metamodel.NonSerializedSemanticsProvider;
using NakedFramework.Metamodel.SemanticsProvider;

namespace NakedFramework.Metamodel.Facet;

[Serializable]
public sealed class ParseableFacetUsingParser<T> : FacetAbstract, IParseableFacet {
    private readonly IValueSemanticsProvider<T> parser;

    public ParseableFacetUsingParser(IValueSemanticsProvider<T> parser) =>
        this.parser = parser;

    public override Type FacetType => typeof(IParseableFacet);

    #region IParseableFacet Members

    public INakedObjectAdapter ParseTextEntry(string entry, INakedObjectManager manager) {
        if (entry == null) {
            throw new ArgumentException(NakedObjects.Resources.NakedObjects.MissingEntryError);
        }

        var parsed = parser.ParseTextEntry(entry);
        return manager.CreateAdapter(parsed, null, null);
    }

    #endregion
}