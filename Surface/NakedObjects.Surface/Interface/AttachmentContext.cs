using System.IO;

namespace NakedObjects.Surface.Interface {
    public class AttachmentContext {
        public const string DefaultMimeType = "application/octet-stream";
        public const string DefaultContentDisposition = "attachment";
        public const string DefaultFileName = "unknown.txt";
        
        private Stream content;
        private string contentDisposition;
        private string fileName;
        private string mimeType;

        public Stream Content {
            get { return content ?? new MemoryStream(); }
            set { content = value; }
        }

        public string MimeType {
            get { return string.IsNullOrWhiteSpace(mimeType) ? DefaultMimeType : mimeType; }
            set { mimeType = value; }
        }

        public string ContentDisposition {
            get { return string.IsNullOrWhiteSpace(contentDisposition) ? DefaultContentDisposition : contentDisposition; }
            set { contentDisposition = value; }
        }

        public string FileName {
            get { return string.IsNullOrWhiteSpace(fileName) ? DefaultFileName : fileName; }
            set { fileName = value; }
        }
    }
}