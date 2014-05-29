// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.IO;
using Common.Logging;
using NakedObjects.Architecture.Adapter.Value;
using NakedObjects.Architecture.Facets;
using NakedObjects.Capabilities;
using NakedObjects.Value;

namespace NakedObjects.Reflector.DotNet.Value {
    public class FileAttachmentValueSemanticsProvider : ValueSemanticsProviderAbstract<FileAttachment>, IFromStream {
        private const bool EqualByContent = true;
        private const bool Immutable = true;
        private const int TypicalLengthDefault = 0;
        private static readonly ILog Log = LogManager.GetLogger(typeof (FileAttachmentValueSemanticsProvider));

        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public FileAttachmentValueSemanticsProvider()
            : this(null) {}

        public FileAttachmentValueSemanticsProvider(IFacetHolder holder)
            : base(Type, holder, AdaptedType, TypicalLengthDefault, Immutable, EqualByContent, null) {}

        public static Type Type {
            get { return typeof (IFileAttachmentValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (FileAttachment); }
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
            return new FileAttachment(stream, name, mimeType);
        }

        public static bool IsAdaptedType(Type type) {
            return type == AdaptedType;
        }


        protected override FileAttachment DoParse(FileAttachment original, string entry) {
            throw new NotImplementedException();
        }

        protected override string DoEncode(FileAttachment fileAttachment) {
            Stream stream = fileAttachment.GetResourceAsStream();
            long len = stream.Length;

            var buffer = new byte[len];
            // TODO check size
            stream.Read(buffer, 0, (int) len);
            string encoded = Convert.ToBase64String(buffer);
            return fileAttachment.MimeType + " " + encoded;
        }

        protected override FileAttachment DoRestore(string data) {
            int offset = data.IndexOf(' ');
            string mime = data.Substring(0, offset);
            Log.Debug("decode " + data.Length + " bytes of image data, type " + mime);
            byte[] buffer = Convert.FromBase64String(data.Substring(offset));
            var stream = new MemoryStream(buffer);
            return new FileAttachment(stream);
        }

        protected override string TitleString(FileAttachment obj) {
            return obj.Name;
        }

        protected override string TitleStringWithMask(string mask, FileAttachment obj) {
            return obj.Name;
        }
    }
}