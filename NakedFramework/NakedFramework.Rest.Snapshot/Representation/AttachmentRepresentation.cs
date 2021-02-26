// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using NakedFramework.Facade.Contexts;
using NakedFramework.Facade.Interface;
using NakedFramework.Facade.Translation;
using NakedFramework.Rest.Snapshot.Utility;

namespace NakedFramework.Rest.Snapshot.Representation {
    public class AttachmentRepresentation : Representation {
        private MediaTypeHeaderValue contentType;

        public AttachmentRepresentation(IOidStrategy oidStrategy, HttpRequest req, PropertyContextFacade propertyContext, RestControlFlags flags)
            : base(oidStrategy, flags) {
            SetContentType(propertyContext);
            SetContentDisposition(propertyContext);
            SetStream(propertyContext);
            SetHeader(propertyContext.Target);
        }

        public ContentDispositionHeaderValue ContentDisposition { get; set; }

        public Stream AsStream { get; set; }

        public override MediaTypeHeaderValue GetContentType() => contentType;

        private void SetHeader(IObjectFacade target) {
            Caching = CacheType.Transactional;
            SetEtag(target);
        }

        private void SetContentType(PropertyContextFacade context) {
            var no = context.Property.GetValue(context.Target);
            string DefaultMimeType() => no == null ? AttachmentContextFacade.DefaultMimeType : no.GetAttachment().DefaultMimeType();
            var mtv = string.IsNullOrWhiteSpace(no?.GetAttachment().MimeType) ? DefaultMimeType() : no.GetAttachment().MimeType;
            contentType = new MediaTypeHeaderValue(mtv);
        }

        private void SetContentDisposition(PropertyContextFacade context) {
            var no = context.Property.GetValue(context.Target);
            var cd = string.IsNullOrWhiteSpace(no?.GetAttachment().ContentDisposition) ? AttachmentContextFacade.DefaultContentDisposition : no.GetAttachment().ContentDisposition;
            var fn = no?.GetAttachment().FileName ?? AttachmentContextFacade.DefaultFileName;
            ContentDisposition = new ContentDispositionHeaderValue(cd) {FileName = fn};
        }

        private void SetStream(PropertyContextFacade context) {
            var no = context.Property.GetValue(context.Target);
            AsStream = no != null ? no.GetAttachment().Content : new MemoryStream();
        }

        public static Representation Create(IOidStrategy oidStrategy, HttpRequest req, PropertyContextFacade propertyContext, RestControlFlags flags) => new AttachmentRepresentation(oidStrategy, req, propertyContext, flags);
    }
}