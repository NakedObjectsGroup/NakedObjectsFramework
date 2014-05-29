// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.IO;

namespace NakedObjects.Value {

    /// <summary>
    /// Type that may be used to present a File Attachment;  contains the file content (as a byte[]), plus, optionally
    /// the file name and the MIME type.
    /// </summary>
    public class FileAttachment : IStreamResource {
        private byte[] buffer;


        public FileAttachment(byte[] resource, string name = null, string mimeType = null) : this(name, mimeType) {
            LoadResourceFromByteArray(resource);
        }

        public FileAttachment(Stream resource, string name = null, string mimeType = null) : this(name, mimeType) {
            LoadResourceFromStream(resource);
        }

        protected FileAttachment(string name = null, string mimeType = null) {
            Name = name;
            MimeType = mimeType;
        }

        /// <summary>
        /// Allows developer to specify an intention of whether the FileAttachment should be rendered in-line or not.  (It is up to the
        /// presentation layer to interpret and make use of this, though).
        /// </summary>
        public string DispositionType { get; set; }

        public Func<FileAttachment, bool> Open { get; set; }

        #region IStreamResource Members

        public string Name { get; set; }
        public string MimeType { get; set; }

        public Stream GetResourceAsStream() {
            if (buffer != null) {
                return new MemoryStream(buffer);
            }
            return null;
        }

        #endregion

        public void LoadResourceFromStream(Stream stream) {
            if (stream != null) {
                var len = (int) stream.Length;
                stream.Position = 0;
                buffer = new byte[len];
                stream.Read(buffer, 0, len);
            }
            else {
                buffer = null;
            }
        }

        public byte[] GetResourceAsByteArray() {
            return buffer;
        }

        public void LoadResourceFromByteArray(byte[] byteArray) {
            buffer = byteArray;
        }

        public override string ToString() {
            return "FileAttachment";
        }
    }
}