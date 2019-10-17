// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using NakedObjects.Rest.Snapshot.Constants;
using NakedObjects.Rest.Snapshot.Representations;
using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest.Snapshot.Strategies {
    [DataContract]
    public abstract class MemberRepresentationStrategy : AbstractStrategy {
        private readonly UriMtHelper objectUri;
        private readonly RelType self;

        protected MemberRepresentationStrategy(IOidStrategy oidStrategy, HttpRequest req, PropertyContextFacade propertyContext, RestControlFlags flags)
            : base(oidStrategy, flags) {
            Req = req;
            PropertyContext = propertyContext;
            objectUri = new UriMtHelper(oidStrategy, req, propertyContext);
            self = new MemberRelType(RelValues.Self, new UriMtHelper(oidStrategy, req, propertyContext));
        }

        protected HttpRequest Req { get; }

        protected PropertyContextFacade PropertyContext { get; }

        public IObjectFacade GetTarget() {
            return PropertyContext.Target;
        }

        public string GetId() {
            return PropertyContext.Property.Id;
        }

        protected UriMtHelper GetHelper() {
            return new UriMtHelper(OidStrategy, Req, PropertyContext);
        }

        protected string GetAttachmentFileName(PropertyContextFacade context) {
            IObjectFacade no = context.Property.GetValue(context.Target);
            return no != null ? no.GetAttachment().FileName : "UnknownFile";
        }

        private LinkRepresentation CreateAttachmentLink() {
            string title = GetAttachmentFileName(PropertyContext);
            return LinkRepresentation.Create(OidStrategy, new AttachmentRelType(GetHelper()), Flags, new OptionalProperty(JsonPropertyNames.Title, title));
        }

        private LinkRepresentation CreateSelfLink() {
            return LinkRepresentation.Create(OidStrategy, self, Flags);
        }

        private LinkRepresentation CreateUpLink() {
            return LinkRepresentation.Create(OidStrategy, new ObjectRelType(RelValues.Up, objectUri), Flags);
        }

        public virtual LinkRepresentation[] GetLinks(bool inline) {
            var tempLinks = new List<LinkRepresentation>();

            if (!inline) {
                tempLinks.Add(CreateUpLink());
                tempLinks.Add(CreateSelfLink());
            }
            else if (!PropertyContext.Target.IsTransient) {
                if (PropertyContext.Property.IsCollection && !PropertyContext.Property.IsEager(PropertyContext.Target)) {
                    tempLinks.Add(CreateCollectionValueLink());
                }
                tempLinks.Add(CreateDetailsLink());
            }

            if (!PropertyContext.Target.IsTransient && RestUtils.IsAttachment(PropertyContext.Specification)) {
                tempLinks.Add(CreateAttachmentLink());
            }

            return tempLinks.ToArray();
        }

        private LinkRepresentation CreateDetailsLink() {
            var opts = new List<OptionalProperty>();

            if (PropertyContext.Property.IsEager(PropertyContext.Target)) {
                opts.Add(new OptionalProperty(JsonPropertyNames.Value, MemberAbstractRepresentation.Create(OidStrategy, Req, PropertyContext, Flags)));
            }

            return LinkRepresentation.Create(OidStrategy, new MemberRelType(GetHelper()), Flags, opts.ToArray());
        }

        private LinkRepresentation CreateCollectionValueLink() {
            return LinkRepresentation.Create(OidStrategy, new CollectionValueRelType(GetHelper()), Flags);
        }

        public RelType GetSelf() {
            return new MemberRelType(RelValues.Self, GetHelper());
        }

        public RestControlFlags GetFlags() {
            return Flags;
        }
    }
}