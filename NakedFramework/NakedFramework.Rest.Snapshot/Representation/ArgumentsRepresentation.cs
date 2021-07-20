// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using NakedFramework.Facade.Contexts;
using NakedFramework.Facade.Interface;
using NakedFramework.Facade.Translation;
using NakedFramework.Rest.Snapshot.Constants;
using NakedFramework.Rest.Snapshot.RelTypes;
using NakedFramework.Rest.Snapshot.Utility;

namespace NakedFramework.Rest.Snapshot.Representation {
    public class ArgumentsRepresentation : MapRepresentation {
        #region Format enum

        public enum Format {
            Full,
            MembersOnly
        }

        #endregion

        private static RefValueRepresentation CreateObjectRef(IOidStrategy oidStrategy, HttpRequest req, IObjectFacade no, RestControlFlags flags) {
            var helper = new UriMtHelper(oidStrategy, req, no);
            var rt = new ObjectRelType(RelValues.Element, helper);

            return RefValueRepresentation.Create(oidStrategy, rt, flags);
        }

        private static MapRepresentation GetMap(IOidStrategy oidStrategy, IFrameworkFacade frameworkFacade, HttpRequest req, ContextFacade context, RestControlFlags flags) {
            MapRepresentation value;

            // All reasons why we cannot create a link representation
            if (context.Specification.IsCollection && context.ElementSpecification is {IsParseable: false}) {
                var proposedObjectFacade = frameworkFacade.GetObject(context.ProposedValue);
                var coll = proposedObjectFacade.ToEnumerable().Select(no => CreateObjectRef(oidStrategy, req, no, flags)).ToArray();
                value = CreateMap(context, coll);
            }
            else if (context.Specification.IsParseable ||
                     context.ProposedValue == null ||
                     context.ProposedObjectFacade == null ||
                     context.ProposedObjectFacade.Specification.IsParseable) {
                value = CreateMap(context, context.ProposedValue);
            }
            else {
                value = CreateMap(context, RefValueRepresentation.Create(oidStrategy, new ObjectRelType(RelValues.Self, new UriMtHelper(oidStrategy, req, context.ProposedObjectFacade)), flags));
            }

            return value;
        }

        private static MapRepresentation CreateMap(ContextFacade context, object obj) {
            var opts = new List<OptionalProperty> {new(JsonPropertyNames.Value, obj)};
            if (!string.IsNullOrEmpty(context.Reason)) {
                opts.Add(new OptionalProperty(JsonPropertyNames.InvalidReason, context.Reason));
            }

            return Create(opts.ToArray());
        }

        public static MapRepresentation Create(IOidStrategy oidStrategy, IFrameworkFacade frameworkFacade, HttpRequest req, ContextFacade contextFacade, IList<ContextFacade> contexts, Format format, RestControlFlags flags, MediaTypeHeaderValue mt) {
            var memberValues = contexts.Select(c => new OptionalProperty(c.Id, GetMap(oidStrategy, frameworkFacade, req, c, flags))).ToList();
            var target = contexts.First().Target;
            MapRepresentation mapRepresentation;

            if (format == Format.Full) {
                var tempProperties = new List<OptionalProperty>();

                if (!string.IsNullOrEmpty(contextFacade?.Reason)) {
                    tempProperties.Add(new OptionalProperty(JsonPropertyNames.XRoInvalidReason, contextFacade.Reason));
                }

                var dt = new OptionalProperty(JsonPropertyNames.DomainType, target.Specification.DomainTypeName(oidStrategy));
                tempProperties.Add(dt);

                var members = new OptionalProperty(JsonPropertyNames.Members, Create(memberValues.ToArray()));
                tempProperties.Add(members);
                mapRepresentation = Create(tempProperties.ToArray());
            }
            else {
                mapRepresentation = Create(memberValues.ToArray());
            }

            mapRepresentation.SetContentType(mt);

            return mapRepresentation;
        }

        public static MapRepresentation Create(IOidStrategy oidStrategy, IFrameworkFacade frameworkFacade, HttpRequest req, ContextFacade context, Format format, RestControlFlags flags, MediaTypeHeaderValue mt) {
            MapRepresentation mapRepresentation;

            if (context is ObjectContextFacade objectContext) {
                var optionalProperties = objectContext.VisibleProperties.Where(p => p.Reason != null || p.ProposedValue != null).Select(c => new OptionalProperty(c.Id, GetMap(oidStrategy, frameworkFacade, req, c, flags))).ToList();
                if (!string.IsNullOrEmpty(objectContext.Reason)) {
                    optionalProperties.Add(new OptionalProperty(JsonPropertyNames.XRoInvalidReason, objectContext.Reason));
                }

                mapRepresentation = Create(optionalProperties.ToArray());
            }
            else if (context is ActionResultContextFacade actionResultContext) {
                var optionalProperties = actionResultContext.ActionContext.VisibleParameters.Select(c => new OptionalProperty(c.Id, GetMap(oidStrategy, frameworkFacade, req, c, flags))).ToList();

                if (!string.IsNullOrEmpty(actionResultContext.Reason)) {
                    optionalProperties.Add(new OptionalProperty(JsonPropertyNames.XRoInvalidReason, actionResultContext.Reason));
                }

                mapRepresentation = Create(optionalProperties.ToArray());
            }
            else {
                mapRepresentation = GetMap(oidStrategy, frameworkFacade, req, context, flags);
            }

            mapRepresentation.SetContentType(mt);

            return mapRepresentation;
        }

        public static MapRepresentation Create(IOidStrategy oidStrategy, IFrameworkFacade frameworkFacade, HttpRequest req, IList<ContextFacade> contexts, Format format, RestControlFlags flags, MediaTypeHeaderValue mt) => Create(oidStrategy, frameworkFacade, req, null, contexts, format, flags, mt);
    }
}