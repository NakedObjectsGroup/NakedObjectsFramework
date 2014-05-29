// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using NakedObjects.Surface;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class CollectionValueRepresentation : Representation {
        protected CollectionValueRepresentation(PropertyContextSurface propertyContext, HttpRequestMessage req, RestControlFlags flags)
            : base(flags) {
            SetScalars(propertyContext);
            SetValue(propertyContext, req, flags);
            SelfRelType = new CollectionValueRelType(RelValues.Self, new UriMtHelper(req, propertyContext));
            SetLinks(req, propertyContext, new ObjectRelType(RelValues.Up, new UriMtHelper(req, propertyContext.Target)));
            SetExtensions();
            SetHeader(propertyContext.Target);
        }

        [DataMember(Name = JsonPropertyNames.Id)]
        public string Id { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        [DataMember(Name = JsonPropertyNames.Value)]
        public LinkRepresentation[] Value { get; set; }

        private void SetValue(PropertyContextSurface propertyContext, HttpRequestMessage req, RestControlFlags flags) {
            IEnumerable<INakedObjectSurface> collectionItems = propertyContext.Property.GetNakedObject(propertyContext.Target).ToEnumerable();
            Value = collectionItems.Select(i => LinkRepresentation.Create(new ValueRelType(propertyContext.Property, new UriMtHelper(req, i)), flags, new OptionalProperty(JsonPropertyNames.Title, RestUtils.SafeGetTitle(i)))).ToArray();
        }

        private void SetScalars(PropertyContextSurface propertyContext) {
            Id = propertyContext.Property.Id;
        }

        private void SetExtensions() {
            Extensions = new MapRepresentation();
        }

        private void SetLinks(HttpRequestMessage req, PropertyContextSurface propertyContext, RelType parentRelType) {
            var tempLinks = new List<LinkRepresentation> {
                LinkRepresentation.Create(parentRelType, Flags),
                LinkRepresentation.Create(SelfRelType, Flags)
            };

            if (Flags.FormalDomainModel) {
                tempLinks.Add(LinkRepresentation.Create(new DomainTypeRelType(RelValues.ReturnType, new UriMtHelper(req, propertyContext.Property)), Flags));
                tempLinks.Add(LinkRepresentation.Create(new DomainTypeRelType(RelValues.ElementType, new UriMtHelper(req, propertyContext.Property.Specification)), Flags));
            }

            Links = tempLinks.ToArray();
        }


        private void SetHeader(INakedObjectSurface target) {
            caching = CacheType.Transactional;
            SetEtag(target);
        }

        public static CollectionValueRepresentation Create(PropertyContextSurface propertyContext, HttpRequestMessage req, RestControlFlags flags) {
            return new CollectionValueRepresentation(propertyContext, req, flags);
        }
    }
}