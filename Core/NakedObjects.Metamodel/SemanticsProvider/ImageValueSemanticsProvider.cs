// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.IO;
using Common.Logging;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Capabilities;
using NakedObjects.Value;

namespace NakedObjects.Meta.SemanticsProvider {
    public class ImageValueSemanticsProvider : ValueSemanticsProviderAbstract<Image>, IFromStream {
        private const bool equalByContent = true;
        private const bool immutable = true;
        private const int typicalLength = 0;
        private static readonly ILog Log = LogManager.GetLogger(typeof (ImageValueSemanticsProvider));

        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public ImageValueSemanticsProvider(IObjectSpecImmutable spec)
            : this(spec, null) {}


        public ImageValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, typicalLength, immutable, equalByContent, null, spec) {}

        public static Type Type {
            get { return typeof (IImageValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (Image); }
        }


        // TODO return null so they are ignored
        /*
        public IParser<Image> Parser {
            get { throw new System.NotImplementedException(); }
        }

        public IDefaultsProvider<Image> DefaultsProvider {
            get { throw new System.NotImplementedException(); }
        }
        */

        public override IFromStream FromStream {
            get { return this; }
        }

        #region IFromStream Members

        public object ParseFromStream(Stream stream, string mimeType = null, string name = null) {
            return new Image(stream, name, mimeType);
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == AdaptedType;
        }


        protected override Image DoParse(string entry) {
            throw new NotImplementedException();
        }


        protected override Image DoParseInvariant(string entry) {
            throw new NotImplementedException();
        }

        protected override string GetInvariantString(Image obj) {
            throw new NotImplementedException();
        }

        protected override string DoEncode(Image image) {
            Stream stream = image.GetResourceAsStream();
            long len = stream.Length;

            var buffer = new byte[len];
            // TODO check size
            stream.Read(buffer, 0, (int) len);
            string encoded = Convert.ToBase64String(buffer);
            return image.MimeType + " " + encoded;
        }

        protected override Image DoRestore(string data) {
            int offset = data.IndexOf(' ');
            string mime = data.Substring(0, offset);
            Log.Debug("decode " + data.Length + " bytes of image data, type " + mime);
            byte[] buffer = Convert.FromBase64String(data.Substring(offset));
            var stream = new MemoryStream(buffer);
            return new Image(stream);
        }

        public object ParseFromStream(Stream stream) {
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