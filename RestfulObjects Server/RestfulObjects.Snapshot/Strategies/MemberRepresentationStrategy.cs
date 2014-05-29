// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using NakedObjects.Surface;
using NakedObjects.Surface.Utility;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Representations;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Strategies {
    [DataContract]
    public abstract class MemberRepresentationStrategy : AbstractStrategy {
        private readonly UriMtHelper objectUri;
        protected readonly PropertyContextSurface propertyContext;
        protected readonly HttpRequestMessage req;
        private readonly RelType self;

        protected MemberRepresentationStrategy(HttpRequestMessage req, PropertyContextSurface propertyContext, RestControlFlags flags) : base(flags) {
            this.req = req;
            this.propertyContext = propertyContext;
            objectUri = new UriMtHelper(req, propertyContext);
            self = new MemberRelType(RelValues.Self, new UriMtHelper(req, propertyContext));
        }

        public INakedObjectSurface GetTarget() {
            return propertyContext.Target;
        }

        public string GetId() {
            return propertyContext.Property.Id;
        }

        protected string GetAttachmentFileName(PropertyContextSurface context) {
            INakedObjectSurface no = context.Property.GetNakedObject(context.Target);
            return no != null ? no.GetAttachment().FileName : "UnknownFile";
        }

        private LinkRepresentation CreateDescribedByLink() {
            return LinkRepresentation.Create(new TypeMemberRelType(RelValues.DescribedBy, new UriMtHelper(req, new PropertyTypeContextSurface {
                Property = propertyContext.Property,
                OwningSpecification = propertyContext.Target.Specification
            })), Flags);
        }

        private LinkRepresentation CreateAttachmentLink() {
            string title = GetAttachmentFileName(propertyContext);
            return LinkRepresentation.Create(new AttachmentRelType(new UriMtHelper(req, propertyContext)), Flags, new OptionalProperty(JsonPropertyNames.Title, title));
        }

        private LinkRepresentation CreateSelfLink() {
            return LinkRepresentation.Create(self, Flags);
        }

        private LinkRepresentation CreateUpLink() {
            return LinkRepresentation.Create(new ObjectRelType(RelValues.Up, objectUri), Flags);
        }

        public virtual LinkRepresentation[] GetLinks(bool inline) {
            var tempLinks = new List<LinkRepresentation>();

            if (!inline) {
                tempLinks.Add(CreateUpLink());
                tempLinks.Add(CreateSelfLink());
            }
            else if (!propertyContext.Target.IsTransient()) {
                if (propertyContext.Property.IsCollection() && !propertyContext.Property.IsEager(propertyContext.Target)) {
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
                opts.Add(new OptionalProperty(JsonPropertyNames.Value, MemberAbstractRepresentation.Create(req, propertyContext, Flags)));
            }

            return LinkRepresentation.Create(new MemberRelType(new UriMtHelper(req, propertyContext)), Flags, opts.ToArray());
        }

        private LinkRepresentation CreateCollectionValueLink() {
            return LinkRepresentation.Create(new CollectionValueRelType(new UriMtHelper(req, propertyContext)), Flags);
        }


        public RelType GetSelf() {
            return new MemberRelType(RelValues.Self, new UriMtHelper(req, propertyContext));
        }

        public RestControlFlags GetFlags() {
            return Flags;
        }
    }
}