// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.IO;

namespace NakedObjects.Value {
    /// <summary>
    /// Specialised version of FileAttachment for handling images.
    /// </summary>
    public class Image : FileAttachment {
        public Image() {}

        public Image(byte[] resource, string name = null, string mimeType = null) : base(resource, name, mimeType) {}

        public Image(Stream resource, string name = null, string mimeType = null) : base(resource, name, mimeType) {}

        public override string ToString() {
            return "Image";
        }
    }
}