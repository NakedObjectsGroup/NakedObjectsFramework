// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Representations;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Strategies {
    [DataContract]
    public abstract class MemberRepresentationStrategy : AbstractStrategy {
        private readonly UriMtHelper objectUri;
        protected readonly PropertyContextFacade propertyContext;
        protected readonly HttpRequestMessage req;
        private readonly RelType self;

        protected MemberRepresentationStrategy(IOidStrategy oidStrategy, HttpRequestMessage req, PropertyContextFacade propertyContext, RestControlFlags flags)
            : base(oidStrategy ,flags) {
            this.req = req;
            this.propertyContext = propertyContext;
            objectUri = new UriMtHelper(oidStrategy ,req, propertyContext);
            self = new MemberRelType(RelValues.Self, new UriMtHelper(oidStrategy , req, propertyContext));
        }

        public IObjectFacade GetTarget() {
            return propertyContext.Target;
        }

        public string GetId() {
            return propertyContext.Property.Id;
        }

        protected string GetAttachmentFileName(PropertyContextFacade context) {
            IObjectFacade no = context.Property.GetValue(context.Target);
            return no != null ? no.GetAttachment().FileName : "UnknownFile";
        }

        private LinkRepresentation CreateDescribedByLink() {
            return LinkRepresentation.Create(OidStrategy ,new TypeMemberRelType(RelValues.DescribedBy, new UriMtHelper(OidStrategy, req, new PropertyTypeContextFacade {
                Property = propertyContext.Property,
                OwningSpecification = propertyContext.Target.Specification
            })), Flags);
        }

        private LinkRepresentation CreateAttachmentLink() {
            string title = GetAttachmentFileName(propertyContext);
            return LinkRepresentation.Create(OidStrategy ,new AttachmentRelType(new UriMtHelper(OidStrategy ,req, propertyContext)), Flags, new OptionalProperty(JsonPropertyNames.Title, title));
        }

        private LinkRepresentation CreateSelfLink() {
            return LinkRepresentation.Create(OidStrategy, self, Flags);
        }

        private LinkRepresentation CreateUpLink() {
            return LinkRepresentation.Create(OidStrategy ,new ObjectRelType(RelValues.Up, objectUri), Flags);
        }

        public virtual LinkRepresentation[] GetLinks(bool inline) {
            var tempLinks = new List<LinkRepresentation>();

            if (!inline) {
                tempLinks.Add(CreateUpLink());
                tempLinks.Add(CreateSelfLink());
            }
            else if (!propertyContext.Target.IsTransient) {
                if (propertyContext.Property.IsCollection && !propertyContext.Property.IsEager(propertyContext.Target)) {
                    tempLinks.Add(CreateCollectionValueLink());
                }
                tempLinks.Add(CreateDetailsLink());
            }

            if (RestUtils.IsAttachment(propertyContext.Specification)) {
                tempLinks.Add(CreateAttachmentLink());
            }

            if (Flags.FormalDomainModel) {
                tempLinks.Add(CreateDescribedByLink());
            }

            return tempLinks.ToArray();
        }


        private LinkRepresentation CreateDetailsLink() {
            var opts = new List<OptionalProperty>();

            if (propertyContext.Property.IsEager(propertyContext.Target)) {
                opts.Add(new OptionalProperty(JsonPropertyNames.Value, MemberAbstractRepresentation.Create(OidStrategy ,req, propertyContext, Flags)));
            }

            return LinkRepresentation.Create(OidStrategy ,new MemberRelType(new UriMtHelper(OidStrategy ,req, propertyContext)), Flags, opts.ToArray());
        }

        private LinkRepresentation CreateCollectionValueLink() {
            return LinkRepresentation.Create(OidStrategy ,new CollectionValueRelType(new UriMtHelper(OidStrategy ,req, propertyContext)), Flags);
        }


        public RelType GetSelf() {
            return new MemberRelType(RelValues.Self, new UriMtHelper(OidStrategy ,req, propertyContext));
        }

        public RestControlFlags GetFlags() {
            return Flags;
        }
    }
}