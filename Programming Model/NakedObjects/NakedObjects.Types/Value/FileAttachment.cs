// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.IO;

namespace NakedObjects.Value {
    /// <summary>
    /// Type that may be used to present a File Attachment;  contains the file content (as a byte[]), plus, optionally
    /// the file name and the MIME type.
    /// </summary>
    [Serializable]
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

        public string Name { get; set; }

        #region IStreamResource Members

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