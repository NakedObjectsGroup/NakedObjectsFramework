// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using NakedObjects.Facade.Translation;
using NakedObjects.Rest.Snapshot.Constants;
using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest.Snapshot.Representations {
    [DataContract]
    public class ObjectRepresentation : Representation {
        protected ObjectRepresentation(IOidStrategy oidStrategy, HttpRequestMessage req, ObjectContextFacade objectContext, RestControlFlags flags)
            : base(oidStrategy, flags) {
            var objectUri = GetHelper(oidStrategy, req, objectContext);
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

        private UriMtHelper GetHelper(IOidStrategy oidStrategy, HttpRequestMessage req, ObjectContextFacade objectContext) {
            return new UriMtHelper(oidStrategy, req, objectContext.Target);
        }

        private static bool IsProtoPersistent(IObjectFacade objectFacade) {
            return objectFacade.IsTransient;
        }

        private static bool IsForm(IObjectFacade objectFacade) {
            return objectFacade.IsViewModelEditView;
        }

        private void SetScalars(ObjectContextFacade objectContext) {
            Title = objectContext.Target.TitleString;
        }

        private void SetHeader(ObjectContextFacade objectContext) {
            Caching = objectContext.Specification.IsService ? CacheType.NonExpiring : CacheType.Transactional;
            SetEtag(objectContext.Target);
        }


        private LinkRepresentation[] CreateIsOfTypeLinks(HttpRequestMessage req, ObjectContextFacade objectContext) {
            var spec = objectContext.Target.Specification;

            return new[] {
                LinkRepresentation.Create(OidStrategy, new TypeActionRelType(new UriMtHelper(OidStrategy, req, spec), WellKnownIds.IsSubtypeOf), Flags,
                    new OptionalProperty(JsonPropertyNames.Id, WellKnownIds.IsSubtypeOf),
                    new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.SuperType, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, null, typeof (object))))))),
                LinkRepresentation.Create(OidStrategy, new TypeActionRelType(new UriMtHelper(OidStrategy, req, spec), WellKnownIds.IsSupertypeOf), Flags,
                    new OptionalProperty(JsonPropertyNames.Id, WellKnownIds.IsSupertypeOf),
                    new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.SubType, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, null, typeof (object)))))))
            };
        }


        private void SetLinksAndMembers(HttpRequestMessage req, ObjectContextFacade objectContext) {
            var tempLinks = new List<LinkRepresentation>();
            if (!objectContext.Mutated && !IsProtoPersistent(objectContext.Target)) {
                tempLinks.Add(LinkRepresentation.Create(OidStrategy, SelfRelType, Flags));
            }

            // custom isSub/SupertypeOf links 

            tempLinks.AddRange(CreateIsOfTypeLinks(req, objectContext));

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

            if (!IsProtoPersistent(objectContext.Target) && !IsForm(objectContext.Target) && visibleProperties.Any(p => p.Property.IsUsable(objectContext.Target).IsAllowed)) {
                string[] ids = visibleProperties.Where(p => p.Property.IsUsable(objectContext.Target).IsAllowed && !p.Property.IsInline).Select(p => p.Id).ToArray();
                OptionalProperty[] props = ids.Select(s => new OptionalProperty(s, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, null, typeof (object))))).ToArray();

                var helper = GetHelper(OidStrategy, req, objectContext);

                LinkRepresentation modifyLink = LinkRepresentation.Create(OidStrategy, new ObjectRelType(RelValues.Update, helper) {Method = RelMethod.Put }, Flags,
                    new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(props)));

                tempLinks.Add(modifyLink);
            }

            if (IsProtoPersistent(objectContext.Target)) {
                KeyValuePair<string, object>[] ids = objectContext.Target.Specification.Properties.Where(p => !p.IsCollection && !p.IsInline).ToDictionary(p => p.Id, p => {

                    var useDate = p.IsDateOnly;
                    return GetPropertyValue(OidStrategy, req, p, objectContext.Target, Flags, true, useDate);
                }).ToArray();
                OptionalProperty[] props = ids.Select(kvp => new OptionalProperty(kvp.Key, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, kvp.Value)))).ToArray();

                var argMembers = new OptionalProperty(JsonPropertyNames.Members, MapRepresentation.Create(props));
                var args = new List<OptionalProperty> {argMembers};

                LinkRepresentation persistLink = LinkRepresentation.Create(OidStrategy, new ObjectsRelType(RelValues.Persist, new UriMtHelper(OidStrategy, req, objectContext.Target.Specification)) {Method = RelMethod.Post}, Flags,
                    new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(args.ToArray())));

                tempLinks.Add(persistLink);
            }

            InlineMemberAbstractRepresentation[] properties = visiblePropertiesAndCollections.Select(p => InlineMemberAbstractRepresentation.Create(OidStrategy, req, p, Flags, false)).ToArray();

            ActionContextFacade[] visibleActions;

            if (IsProtoPersistent(objectContext.Target)) {
                visibleActions = new ActionContextFacade[] {};
            }
            else if (IsForm(objectContext.Target)) {
                visibleActions = objectContext.VisibleActions.Where(af => af.Action.ParameterCount == 0).ToArray();
            }
            else {
                visibleActions = FilterLocallyContributedActions(objectContext.VisibleActions, visiblePropertiesAndCollections.Where(p => p.Property.IsCollection).ToArray());
            }

            InlineActionRepresentation[] actions = visibleActions.Select(a => InlineActionRepresentation.Create(OidStrategy, req, a, Flags)).ToArray();
          
            IEnumerable<InlineMemberAbstractRepresentation> allMembers = properties.Union(actions);

            Members = RestUtils.CreateMap(allMembers.ToDictionary(m => m.Id, m => (object) m));
        }

        private ActionContextFacade[] FilterLocallyContributedActions(ActionContextFacade[] actions, PropertyContextFacade[] collections) {
            var lcas = collections.SelectMany(c => c.Target.Specification.GetLocallyContributedActions(c.Property.ElementSpecification, c.Property.Id));
            return actions.Where(a => !lcas.Select(lca => lca.Id).Contains(a.Id)).ToArray();
        }

        private IDictionary<string, object> GetCustomExtensions(IObjectFacade objectFacade) {

            InteractionMode mode;

            if (objectFacade.IsNotPersistent && !objectFacade.IsViewModel) {
                mode = InteractionMode.NotPersistent;
            }
            else if (objectFacade.IsTransient) {
                mode = InteractionMode.Transient;
            }
            else if (objectFacade.IsViewModelEditView) {
                mode = InteractionMode.Form;
            }
            else {
                mode = InteractionMode.Persistent;
            }

            return new Dictionary<string, object> {
                [JsonPropertyNames.InteractionMode] = mode.ToString().ToLower()
            };
        }

        private void SetExtensions(IObjectFacade objectFacade) {
            Extensions = GetExtensions(objectFacade);
        }

        private MapRepresentation GetExtensions(IObjectFacade objectFacade) {
            return RestUtils.GetExtensions(
                           friendlyname: objectFacade.Specification.SingularName,
                           description: objectFacade.Specification.Description,
                           pluralName: objectFacade.Specification.PluralName,
                           domainType: objectFacade.Specification.DomainTypeName(OidStrategy),
                           isService: objectFacade.Specification.IsService,
                           hasParams: null,
                           optional: null,
                           maxLength: null,
                           pattern: null,
                           memberOrder: null,
                           dataType: null,
                           presentationHint: objectFacade.PresentationHint,
                           customExtensions: GetCustomExtensions(objectFacade),
                           returnType: null,
                           elementType: null,
                           oidStrategy: OidStrategy,
                           useDateOverDateTime: false);
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
                props.Add(new OptionalProperty(JsonPropertyNames.InstanceId, oid.InstanceId));
                props.Add(new OptionalProperty(JsonPropertyNames.DomainType, oid.DomainType));
            }

            return CreateWithOptionals<ObjectRepresentation>(new object[] {oidStrategy, req, objectContext, flags}, props);
        }
    }
}