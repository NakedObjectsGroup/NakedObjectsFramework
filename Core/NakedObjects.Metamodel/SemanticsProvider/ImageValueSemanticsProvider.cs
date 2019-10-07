// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Diagnostics;
using System.IO;
using Common.Logging;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Value;

namespace NakedObjects.Meta.SemanticsProvider {
    [Serializable]
    public sealed class ImageValueSemanticsProvider : ValueSemanticsProviderAbstract<Image>, IImageValueFacet, IFromStream {
        private const bool EqualByContent = true;
        private const bool Immutable = true;
        private const int TypicalLengthConst = 0;
        private static readonly ILog Log = LogManager.GetLogger(typeof(ImageValueSemanticsProvider));

        public ImageValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, TypicalLengthConst, Immutable, EqualByContent, null, spec) { }

        private static Type Type => typeof(IImageValueFacet);

        public static Type AdaptedType => typeof(Image);

        #region IFromStream Members

        public object ParseFromStream(Stream stream, string mimeType = null, string name = null) {
            return new Image(stream, name, mimeType);
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == AdaptedType;
        }

        protected override Image DoParse(string entry) {
            Trace.Assert(false, "Image cannot parse: " + entry);
            return null;
        }

        protected override Image DoParseInvariant(string entry) {
            Trace.Assert(false, "Image cannot parse invariant: " + entry);
            return null;
        }

        protected override string GetInvariantString(Image obj) {
            Trace.Assert(false, "Image cannot get invraiant string");
            return null;
        }

        protected override string DoEncode(Image image) {
            Stream stream = image.GetResourceAsStream();

            if (stream.Length > int.MaxValue) {
                throw new ModelException($"Image is too large size: {stream.Length} max: {int.MaxValue} name: {image.Name}");
            }

            int len = Convert.ToInt32(stream.Length);
            var buffer = new byte[len];
            ReadWholeArray(stream, buffer);
            string encoded = Convert.ToBase64String(buffer);
            return image.MimeType + " " + encoded;
        }

        protected override Image DoRestore(string data) {
            int offset = data.IndexOf(' ');
            string mime = data.Substring(0, offset);
            byte[] buffer = Convert.FromBase64String(data.Substring(offset));
            var stream = new MemoryStream(buffer);
            return new Image(stream);
        }

        protected override string TitleString(Image obj) {
            return obj.Name;
        }

        protected override string TitleStringWithMask(string mask, Image obj) {
            return obj.Name;
        }
    }
}