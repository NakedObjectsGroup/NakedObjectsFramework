// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

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
    public class ObjectRepresentation : Representation {
        protected ObjectRepresentation(HttpRequestMessage req, ObjectContextSurface objectContext, RestControlFlags flags) : base(flags) {
            var objectUri = new UriMtHelper(req, objectContext.Target);
            SetScalars(objectContext);
            SelfRelType = objectContext.Specification.IsService() ? new ServiceRelType(RelValues.Self, objectUri) : new ObjectRelType(RelValues.Self, objectUri);
            SetLinksAndMembers(req, objectContext);
            SetExtensions(objectContext.Target);
            SetHeader(objectContext);
        }

        [DataMember(Name = JsonPropertyNames.Title)]
        public string Title { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        [DataMember(Name = JsonPropertyNames.Members)]
        public MapRepresentation Members { get; set; }

        private void SetScalars(ObjectContextSurface objectContext) {
            Title = objectContext.Target.TitleString();
        }

        private void SetHeader(ObjectContextSurface objectContext) {
            caching = objectContext.Specification.IsService() ? CacheType.NonExpiring : CacheType.Transactional;
            SetEtag(objectContext.Target);
        }

        private void SetLinksAndMembers(HttpRequestMessage req, ObjectContextSurface objectContext) {
            var tempLinks = new List<LinkRepresentation>();
            if (!objectContext.Mutated && !objectContext.Target.IsTransient()) {
                tempLinks.Add(LinkRepresentation.Create(SelfRelType, Flags));
            }

            if (Flags.FormalDomainModel) {
                tempLinks.Add(LinkRepresentation.Create(new DomainTypeRelType(RelValues.DescribedBy, new UriMtHelper(req, objectContext.Specification)), Flags));
            }

            // temp disable icons 
            //tempLinks.Add(LinkRepresentation.Create(new IconRelType(objectUri), Flags));
            SetMembers(objectContext, req, tempLinks);
            Links = tempLinks.ToArray();
        }

        private void SetMembers(ObjectContextSurface objectContext, HttpRequestMessage req, List<LinkRepresentation> tempLinks) {
            PropertyContextSurface[] visiblePropertiesAndCollections = objectContext.VisibleProperties;

            if (!Flags.BlobsClobs) {
                // filter any blobs and clobs 
                visiblePropertiesAndCollections = visiblePropertiesAndCollections.Where(vp => !RestUtils.IsBlobOrClob(vp.Specification)).ToArray();
            }

            PropertyContextSurface[] visibleProperties = visiblePropertiesAndCollections.Where(p => !p.Property.IsCollection()).ToArray();

            if (!objectContext.Target.IsTransient() && visibleProperties.Any(p => p.Property.IsUsable(objectContext.Target).IsAllowed)) {
                string[] ids = visibleProperties.Where(p => p.Property.IsUsable(objectContext.Target).IsAllowed && !p.Property.IsInline()).Select(p => p.Id).ToArray();
                OptionalProperty[] props = ids.Select(s => new OptionalProperty(s, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, null, typeof (object))))).ToArray();

                LinkRepresentation modifyLink = LinkRepresentation.Create(new ObjectRelType(RelValues.Update, new UriMtHelper(req, objectContext.Target)) {Method = RelMethod.Put}, Flags,
                    new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(props)));

                tempLinks.Add(modifyLink);
            }

            if (objectContext.Target.IsTransient()) {
                KeyValuePair<string, object>[] ids = objectContext.Target.Specification.Properties.Where(p => !p.IsCollection() && !p.IsInline()).ToDictionary(p => p.Id, p => GetPropertyValue(req, p, objectContext.Target, Flags, true)).ToArray();
                OptionalProperty[] props = ids.Select(kvp => new OptionalProperty(kvp.Key, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, kvp.Value)))).ToArray();

                var argMembers = new OptionalProperty(JsonPropertyNames.Members, MapRepresentation.Create(props));
                var args = new List<OptionalProperty> {argMembers};

                LinkRepresentation persistLink = LinkRepresentation.Create(new ObjectsRelType(RelValues.Persist, new UriMtHelper(req, objectContext.Target.Specification)) {Method = RelMethod.Post}, Flags,
                    new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(args.ToArray())));

                tempLinks.Add(persistLink);
            }

            InlineMemberAbstractRepresentation[] properties = visiblePropertiesAndCollections.Select(p => InlineMemberAbstractRepresentation.Create(req, p, Flags)).ToArray();

            InlineActionRepresentation[] actions = objectContext.Target.IsTransient() ? new InlineActionRepresentation[] {}
                : objectContext.VisibleActions.Select(a => InlineActionRepresentation.Create(req, a, Flags)).ToArray();


            IEnumerable<InlineMemberAbstractRepresentation> allMembers = properties.Union(actions);

            Members = RestUtils.CreateMap(allMembers.ToDictionary(m => m.Id, m => (object) m));
        }

        private IDictionary<string, object> GetCustomExtensions(INakedObjectSurface nakedObject) {
            return nakedObject.ExtensionData() == null ? null : nakedObject.ExtensionData().ToDictionary(kvp => kvp.Key, kvp => (object) kvp.Value.ToString().ToLower());
        }

        private void SetExtensions(INakedObjectSurface nakedObject) {
            if (Flags.SimpleDomainModel) {
                Extensions = RestUtils.GetExtensions(friendlyname: nakedObject.Specification.SingularName(),
                    description: nakedObject.Specification.Description(),
                    pluralName: nakedObject.Specification.PluralName(),
                    domainType: nakedObject.Specification.DomainTypeName(),
                    isService: nakedObject.Specification.IsService(),
                    hasParams: null,
                    optional: null,
                    maxLength: null,
                    pattern: null,
                    memberOrder: null,
                    customExtensions: GetCustomExtensions(nakedObject),
                    returnType: null,
                    elementType: null);
            }
            else {
                Extensions = MapRepresentation.Create();
            }
        }

        public static ObjectRepresentation Create(INakedObjectSurface target, HttpRequestMessage req, RestControlFlags flags) {
            ObjectContextSurface oc = target.Surface.GetObject(target);
            return Create(oc, req, flags);
        }

        public static ObjectRepresentation Create(ObjectContextSurface objectContext, HttpRequestMessage req, RestControlFlags flags) {
            if (objectContext.Target != null && (objectContext.Specification.IsService() || !objectContext.Target.IsTransient())) {
                return CreateObjectWithOptionals(objectContext, req, flags);
            }

            return new ObjectRepresentation(req, objectContext, flags);
        }

        private static ObjectRepresentation CreateObjectWithOptionals(ObjectContextSurface objectContext, HttpRequestMessage req, RestControlFlags flags) {
            LinkObjectId oid = OidStrategyHolder.OidStrategy.GetOid(objectContext.Target);

            var props = new List<OptionalProperty>();
            if (objectContext.Specification.IsService()) {
                props.Add(new OptionalProperty(JsonPropertyNames.ServiceId, oid.DomainType));
            }
            else {
                props.Add(new OptionalProperty(JsonPropertyNames.InstanceId, oid.InstanceId));
                if (flags.SimpleDomainModel) {
                    props.Add(new OptionalProperty(JsonPropertyNames.DomainType, oid.DomainType));
                }
            }

            return CreateWithOptionals<ObjectRepresentation>(new object[] {req, objectContext, flags}, props);
        }
    }
}