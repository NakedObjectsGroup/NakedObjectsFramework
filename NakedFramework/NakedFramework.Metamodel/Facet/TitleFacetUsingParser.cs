// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Metamodel.SemanticsProvider;

namespace NakedFramework.Metamodel.Facet;

[Serializable]
public sealed class TitleFacetUsingParser<T> : TitleFacetAbstract, ITitleFacet {
    private readonly IValueSemanticsProvider<T> parser;

    public TitleFacetUsingParser(IValueSemanticsProvider<T> parser) =>
        this.parser = parser;

    #region ITitleFacet Members

    public override string GetTitle(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) => nakedObjectAdapter?.Object == null ? null : parser.DisplayTitleOf((T)nakedObjectAdapter.Object);

    public override string GetTitleWithMask(string mask, INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) => nakedObjectAdapter?.Object == null ? null : parser.TitleWithMaskOf(mask, (T)nakedObjectAdapter.Object);

    #endregion
}