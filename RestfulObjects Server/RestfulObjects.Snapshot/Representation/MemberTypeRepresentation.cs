// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using NakedObjects.Surface;
using NakedObjects.Surface.Utility;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public abstract class MemberTypeRepresentation : Representation {
        protected MemberTypeRepresentation(HttpRequestMessage req, PropertyTypeContextSurface propertyContext, RestControlFlags flags) : base(flags) {
            SetScalars(propertyContext);
            SelfRelType = new TypeMemberRelType(RelValues.Self, new UriMtHelper(req, propertyContext));
            SetExtensions();
            SetHeader();
        }

        [DataMember(Name = JsonPropertyNames.Id)]
        public string Id { get; set; }

        [DataMember(Name = JsonPropertyNames.FriendlyName)]
        public string FriendlyName { get; set; }

        [DataMember(Name = JsonPropertyNames.Description)]
        public string Description { get; set; }

        [DataMember(Name = JsonPropertyNames.MemberOrder)]
        public int MemberOrder { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        private void SetScalars(PropertyTypeContextSurface propertyContext) {
            Id = propertyContext.Property.Id;
            FriendlyName = propertyContext.Property.Name();
            Description = propertyContext.Property.Description();
            MemberOrder = propertyContext.Property.MemberOrder();
        }

        private void SetHeader() {
            caching = CacheType.NonExpiring;
        }

        private void SetExtensions() {
            Extensions = MapRepresentation.Create();
        }

        protected IList<LinkRepresentation> CreateLinks(HttpRequestMessage req, PropertyTypeContextSurface propertyContext) {
            var domainTypeUri = new UriMtHelper(req, propertyContext);
            return new List<LinkRepresentation> {
                LinkRepresentation.Create(SelfRelType, Flags),
                LinkRepresentation.Create(new DomainTypeRelType(RelValues.Up, domainTypeUri), Flags)
            };
        }


        public static MemberTypeRepresentation Create(HttpRequestMessage req, PropertyTypeContextSurface propertyContext, RestControlFlags flags) {
            if (propertyContext.Property.IsCollection()) {
                return CollectionTypeRepresentation.Create(req, propertyContext, flags);
            }

            Tuple<string, string> typeAndFormat = RestUtils.SpecToTypeAndFormatString(propertyContext.Property.Specification);

            if (typeAndFormat.Item1 == PredefinedType.String.ToRoString()) {
                var exts = new Dictionary<string, object>();

                AddStringProperties(propertyContext.Property.Specification, propertyContext.Property.MaxLength(), propertyContext.Property.Pattern(), exts);

                OptionalProperty[] parms = exts.Select(e => new OptionalProperty(e.Key, e.Value)).ToArray();

                return CreateWithOptionals<PropertyTypeRepresentation>(new object[] {req, propertyContext, flags}, parms);
            }


            return PropertyTypeRepresentation.Create(req, propertyContext, flags);
        }
    }
}