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
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Value;

namespace NakedFramework.Metamodel.SemanticsProvider;

[Serializable]
public sealed class FileAttachmentValueSemanticsProvider : ValueSemanticsProviderAbstract<FileAttachment>, IFileAttachmentValueFacet, IFromStream {
    private const bool Immutable = true;

    public FileAttachmentValueSemanticsProvider(IObjectSpecImmutable spec)
        : base(Type, AdaptedType, Immutable, null) { }

    public static Type Type => typeof(IFileAttachmentValueFacet);

    public static Type AdaptedType => typeof(FileAttachment);

    public static KeyValuePair<Type, Func<IObjectSpecImmutable, IValueSemanticsProvider>> Factory => new(AdaptedType, o => new FileAttachmentValueSemanticsProvider(o));

    #region IFromStream Members

    public object ParseFromStream(Stream stream, string mimeType = null, string name = null) => new FileAttachment(stream, name, mimeType);

    #endregion

    protected override FileAttachment DoParse(string entry) => throw new NakedObjectSystemException($"FileAttachment cannot parse: {entry}");

    protected override string TitleString(FileAttachment obj) => obj.Name;

    protected override string TitleStringWithMask(string mask, FileAttachment obj) => obj.Name;
}