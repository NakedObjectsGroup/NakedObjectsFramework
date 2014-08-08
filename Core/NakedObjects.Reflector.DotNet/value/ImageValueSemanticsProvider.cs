// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.IO;
using Common.Logging;
using NakedObjects.Architecture.Adapter.Value;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Capabilities;
using NakedObjects.Value;

namespace NakedObjects.Reflector.DotNet.Value {
    public class ImageValueSemanticsProvider : ValueSemanticsProviderAbstract<Image>, IFromStream {
        private const bool equalByContent = true;
        private const bool immutable = true;
        private const int typicalLength = 0;
        private static readonly ILog Log = LogManager.GetLogger(typeof (ImageValueSemanticsProvider));

        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public ImageValueSemanticsProvider(INakedObjectReflector reflector)
            : this(reflector, null) {}


        public ImageValueSemanticsProvider(INakedObjectReflector reflector, IFacetHolder holder)
            : base(Type, holder, AdaptedType, typicalLength, immutable, equalByContent, null, reflector) { }

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

        public object ParseFromStream(Stream stream, string mimeType = null, string name = null) {
            return new Image(stream, name, mimeType);
        }

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