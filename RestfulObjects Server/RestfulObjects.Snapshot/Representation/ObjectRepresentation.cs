// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using NakedObjects.Facade.Translation;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class ObjectRepresentation : Representation {
        protected ObjectRepresentation(IOidStrategy oidStrategy, HttpRequestMessage req, ObjectContextFacade objectContext, RestControlFlags flags)
            : base(oidStrategy, flags) {
            var objectUri = new UriMtHelper(oidStrategy, req, objectContext.Target);
            SetScalars(objectContext);
            SelfRelType = objectContext.Specification.IsService ? new ServiceRelType(RelValues.Self, objectUri) : new ObjectRelType(RelValues.Self, objectUri);
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

        private static bool IsProtoPersistent(IObjectFacade objectFacade) {
            return objectFacade.IsTransient && RestControlFlags.ProtoPersistentObjects;
        }

        private void SetScalars(ObjectContextFacade objectContext) {
            Title = objectContext.Target.TitleString;
        }

        private void SetHeader(ObjectContextFacade objectContext) {
            caching = objectContext.Specification.IsService ? CacheType.NonExpiring : CacheType.Transactional;
            SetEtag(objectContext.Target);
        }

        private void SetLinksAndMembers(HttpRequestMessage req, ObjectContextFacade objectContext) {
            var tempLinks = new List<LinkRepresentation>();
            if (!objectContext.Mutated && !IsProtoPersistent(objectContext.Target)) {
                tempLinks.Add(LinkRepresentation.Create(OidStrategy, SelfRelType, Flags));
            }

            if (Flags.FormalDomainModel) {
                tempLinks.Add(LinkRepresentation.Create(OidStrategy, new DomainTypeRelType(RelValues.DescribedBy, new UriMtHelper(OidStrategy, req, objectContext.Specification)), Flags));
            }

            // temp disable icons 
            //tempLinks.Add(LinkRepresentation.Create(new IconRelType(objectUri), Flags));
            SetMembers(objectContext, req, tempLinks);
            Links = tempLinks.ToArray();
        }

        private void SetMembers(ObjectContextFacade objectContext, HttpRequestMessage req, List<LinkRepresentation> tempLinks) {
            PropertyContextFacade[] visiblePropertiesAndCollections = objectContext.VisibleProperties;

            if (!Flags.BlobsClobs) {
                // filter any blobs and clobs 
                visiblePropertiesAndCollections = visiblePropertiesAndCollections.Where(vp => !RestUtils.IsBlobOrClob(vp.Specification)).ToArray();
            }

            PropertyContextFacade[] visibleProperties = visiblePropertiesAndCollections.Where(p => !p.Property.IsCollection).ToArray();

            if (!IsProtoPersistent(objectContext.Target) && visibleProperties.Any(p => p.Property.IsUsable(objectContext.Target).IsAllowed)) {
                string[] ids = visibleProperties.Where(p => p.Property.IsUsable(objectContext.Target).IsAllowed && !p.Property.IsInline).Select(p => p.Id).ToArray();
                OptionalProperty[] props = ids.Select(s => new OptionalProperty(s, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, null, typeof (object))))).ToArray();

                LinkRepresentation modifyLink = LinkRepresentation.Create(OidStrategy, new ObjectRelType(RelValues.Update, new UriMtHelper(OidStrategy, req, objectContext.Target)) {Method = RelMethod.Put}, Flags,
                    new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(props)));

                tempLinks.Add(modifyLink);
            }

            if (IsProtoPersistent(objectContext.Target)) {
                KeyValuePair<string, object>[] ids = objectContext.Target.Specification.Properties.Where(p => !p.IsCollection && !p.IsInline).ToDictionary(p => p.Id, p => GetPropertyValue(OidStrategy, req, p, objectContext.Target, Flags, true)).ToArray();
                OptionalProperty[] props = ids.Select(kvp => new OptionalProperty(kvp.Key, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, kvp.Value)))).ToArray();

                var argMembers = new OptionalProperty(JsonPropertyNames.Members, MapRepresentation.Create(props));
                var args = new List<OptionalProperty> {argMembers};

                LinkRepresentation persistLink = LinkRepresentation.Create(OidStrategy, new ObjectsRelType(RelValues.Persist, new UriMtHelper(OidStrategy, req, objectContext.Target.Specification)) {Method = RelMethod.Post}, Flags,
                    new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(args.ToArray())));

                tempLinks.Add(persistLink);
            }

            InlineMemberAbstractRepresentation[] properties = visiblePropertiesAndCollections.Select(p => InlineMemberAbstractRepresentation.Create(OidStrategy, req, p, Flags)).ToArray();

            InlineActionRepresentation[] actions = objectContext.Target.IsTransient ? new InlineActionRepresentation[] {}
                : objectContext.VisibleActions.Select(a => InlineActionRepresentation.Create(OidStrategy, req, a, Flags)).ToArray();

            IEnumerable<InlineMemberAbstractRepresentation> allMembers = properties.Union(actions);

            Members = RestUtils.CreateMap(allMembers.ToDictionary(m => m.Id, m => (object) m));
        }

        private IDictionary<string, object> GetCustomExtensions(IObjectFacade objectFacade) {
            return objectFacade.ExtensionData == null ? null : objectFacade.ExtensionData.ToDictionary(kvp => kvp.Key, kvp => (object) kvp.Value.ToString().ToLower());
        }

        private void SetExtensions(IObjectFacade objectFacade) {
            if (Flags.SimpleDomainModel) {
                Extensions = RestUtils.GetExtensions(objectFacade.Specification.SingularName, objectFacade.Specification.Description, objectFacade.Specification.PluralName, objectFacade.Specification.DomainTypeName(OidStrategy), objectFacade.Specification.IsService, null, null, null, null, null, GetCustomExtensions(objectFacade), null, null, OidStrategy);
            }
            else {
                Extensions = MapRepresentation.Create();
            }
        }

        public static ObjectRepresentation Create(IOidStrategy oidStrategy, IObjectFacade target, HttpRequestMessage req, RestControlFlags flags) {
            ObjectContextFacade oc = target.FrameworkFacade.GetObject(target);
            return Create(oidStrategy, oc, req, flags);
        }

        public static ObjectRepresentation Create(IOidStrategy oidStrategy, ObjectContextFacade objectContext, HttpRequestMessage req, RestControlFlags flags) {
            if (objectContext.Target != null && (objectContext.Specification.IsService || !IsProtoPersistent(objectContext.Target))) {
                return CreateObjectWithOptionals(oidStrategy, objectContext, req, flags);
            }

            return new ObjectRepresentation(oidStrategy, req, objectContext, flags);
        }

        private static ObjectRepresentation CreateObjectWithOptionals(IOidStrategy oidStrategy, ObjectContextFacade objectContext, HttpRequestMessage req, RestControlFlags flags) {
            IOidTranslation oid = oidStrategy.FrameworkFacade.OidTranslator.GetOidTranslation(objectContext.Target);

            var props = new List<OptionalProperty>();
            if (objectContext.Specification.IsService) {
                props.Add(new OptionalProperty(JsonPropertyNames.ServiceId, oid.DomainType));
            }
            else {
                // if get here and transient then must be non-protopersistent so add pseudo id;
                var id = objectContext.Target.IsTransient ? (object) objectContext.UniqueIdForTransient : oid.InstanceId;
                props.Add(new OptionalProperty(JsonPropertyNames.InstanceId, id));
                if (flags.SimpleDomainModel) {
                    props.Add(new OptionalProperty(JsonPropertyNames.DomainType, oid.DomainType));
                }
            }

            return CreateWithOptionals<ObjectRepresentation>(new object[] {oidStrategy, req, objectContext, flags}, props);
        }
    }
}