// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.IO;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Value;

namespace NakedFramework.Metamodel.SemanticsProvider;

[Serializable]
public sealed class FileAttachmentValueSemanticsProvider : ValueSemanticsProviderAbstract<FileAttachment>, IFileAttachmentValueFacet, IFromStream {
    private const bool EqualByContent = true;
    private const bool Immutable = true;
    private const int TypicalLengthDefault = 0;

    public FileAttachmentValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
        : base(Type, holder, AdaptedType, TypicalLengthDefault, Immutable, EqualByContent, null, spec) { }

    public static Type Type => typeof(IFileAttachmentValueFacet);

    public static Type AdaptedType => typeof(FileAttachment);

    #region IFromStream Members

    public object ParseFromStream(Stream stream, string mimeType = null, string name = null) => new FileAttachment(stream, name, mimeType);

    #endregion

    public static bool IsAdaptedType(Type type) => type == AdaptedType;

    protected override FileAttachment DoParse(string entry) => throw new NakedObjectSystemException($"FileAttachment cannot parse: {entry}");

    protected override FileAttachment DoParseInvariant(string entry) => throw new NakedObjectSystemException($"FileAttachment cannot parse invariant: {entry}");

    protected override string GetInvariantString(FileAttachment obj) => throw new NakedObjectSystemException("FileAttachment cannot get invariant string");

    protected override string DoEncode(FileAttachment fileAttachment) {
        var stream = fileAttachment.GetResourceAsStream();

        if (stream.Length > int.MaxValue) {
            throw new ModelException($"Attachment is too large size: {stream.Length} max: {int.MaxValue} name: {fileAttachment.Name}");
        }

        var len = Convert.ToInt32(stream.Length);
        var buffer = new byte[len];
        ReadWholeArray(stream, buffer);
        var encoded = Convert.ToBase64String(buffer);
        return $"{fileAttachment.MimeType} {encoded}";
    }

    protected override FileAttachment DoRestore(string data) {
        var offset = data.IndexOf(' ');
        var mime = data.Substring(0, offset);
        var buffer = Convert.FromBase64String(data[offset..]);
        var stream = new MemoryStream(buffer);
        return new FileAttachment(stream);
    }

    protected override string TitleString(FileAttachment obj) => obj.Name;

    protected override string TitleStringWithMask(string mask, FileAttachment obj) => obj.Name;
}