// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using NakedFramework.Architecture.Facet;
using NakedFramework.Core.Error;
using NakedFramework.Value;

namespace NakedFramework.Metamodel.SemanticsProvider;

[Serializable]
public sealed class ImageValueSemanticsProvider : ValueSemanticsProviderAbstract<Image>, IImageValueFacet, IFromStream {
    private const bool Immutable = true;
    private static ImageValueSemanticsProvider instance;

    private ImageValueSemanticsProvider() : base(Immutable, null) { }
    internal static ImageValueSemanticsProvider Instance => instance ??= new ImageValueSemanticsProvider();

    public static Type AdaptedType => typeof(Image);

    public static KeyValuePair<Type, IValueSemanticsProvider> Factory => new(AdaptedType, Instance);

    #region IFromStream Members

    public object ParseFromStream(Stream stream, string mimeType = null, string name = null) => new Image(stream, name, mimeType);

    #endregion

    public override Type FacetType => typeof(IImageValueFacet);

    protected override Image DoParse(string entry) => throw new NakedObjectSystemException($"Image cannot parse: {entry}");

    protected override string TitleString(Image obj) => obj.Name;

    protected override string TitleStringWithMask(string mask, Image obj) => obj.Name;
}