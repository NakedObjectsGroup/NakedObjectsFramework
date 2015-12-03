// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using NakedObjects.Surface;
using NakedObjects.Surface.Utility;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    public class ArgumentsRepresentation : MapRepresentation {
        #region Format enum

        public enum Format {
            Full,
            MembersOnly
        };

        #endregion

        public ArgumentsRepresentation(IOidStrategy oidStrategy) : base(oidStrategy) {}

        private static MapRepresentation GetMap(HttpRequestMessage req, ContextSurface context, RestControlFlags flags) {
            MapRepresentation value;

            // All reasons why we cannot create a linkrep
            if (context.Specification.IsParseable() ||
                context.Specification.IsCollection() ||
                context.ProposedValue == null ||
                context.ProposedNakedObject == null ||
                context.ProposedNakedObject.Specification.IsParseable()) {
                value = CreateMap(context, context.ProposedValue, flags.OidStrategy);
            }
            else {
                value = CreateMap(context, RefValueRepresentation.Create(new ObjectRelType(RelValues.Self, new UriMtHelper(req, context.ProposedNakedObject, flags.OidStrategy)), flags), flags.OidStrategy);
            }
            return value;
        }

        private static MapRepresentation CreateMap(ContextSurface context, object obj, IOidStrategy oidStrategy) {
            var opts = new List<OptionalProperty> {new OptionalProperty(JsonPropertyNames.Value, obj)};
            if (!string.IsNullOrEmpty(context.Reason)) {
                opts.Add(new OptionalProperty(JsonPropertyNames.InvalidReason, context.Reason));
            }
            return Create(oidStrategy, opts.ToArray());
        }

        public static MapRepresentation Create(HttpRequestMessage req, ContextSurface context, Format format, RestControlFlags flags, MediaTypeHeaderValue mt) {
            var objectContextSurface = context as ObjectContextSurface;
            var actionResultContextSurface = context as ActionResultContextSurface;
            MapRepresentation mapRepresentation;


            if (objectContextSurface != null) {
                List<OptionalProperty> optionalProperties = objectContextSurface.VisibleProperties.Where(p => p.Reason != null || p.ProposedValue != null).Select(c => new OptionalProperty(c.Id, GetMap(req, c, flags))).ToList();
                if (!string.IsNullOrEmpty(objectContextSurface.Reason)) {
                    optionalProperties.Add(new OptionalProperty(JsonPropertyNames.XRoInvalidReason, objectContextSurface.Reason));
                }
                mapRepresentation = Create(flags.OidStrategy, optionalProperties.ToArray());
            }
            else if (actionResultContextSurface != null) {
                List<OptionalProperty> optionalProperties = actionResultContextSurface.ActionContext.VisibleParameters.Select(c => new OptionalProperty(c.Id, GetMap(req, c, flags))).ToList();

                if (!string.IsNullOrEmpty(actionResultContextSurface.Reason)) {
                    optionalProperties.Add(new OptionalProperty(JsonPropertyNames.XRoInvalidReason, actionResultContextSurface.Reason));
                }
                mapRepresentation = Create(flags.OidStrategy, optionalProperties.ToArray());
            }
            else {
                mapRepresentation = GetMap(req, context, flags);
            }


            mapRepresentation.SetContentType(mt);


            return mapRepresentation;
        }

        public static MapRepresentation Create(HttpRequestMessage req, IList<ContextSurface> contexts, Format format, RestControlFlags flags, MediaTypeHeaderValue mt) {
            OptionalProperty[] memberValues = contexts.Select(c => new OptionalProperty(c.Id, GetMap(req, c, flags))).ToArray();
            INakedObjectSurface target = contexts.First().Target;
            MapRepresentation mapRepresentation;

            if (format == Format.Full) {
                var tempProperties = new List<OptionalProperty>();

                if (flags.SimpleDomainModel) {
                    var dt = new OptionalProperty(JsonPropertyNames.DomainType, target.Specification.DomainTypeName(flags.OidStrategy));
                    tempProperties.Add(dt);
                }

                if (flags.FormalDomainModel) {
                    var links = new OptionalProperty(JsonPropertyNames.Links, new[] {
                        Create(flags.OidStrategy, new OptionalProperty(JsonPropertyNames.Rel, RelValues.DescribedBy),
                            new OptionalProperty(JsonPropertyNames.Href, new UriMtHelper(req, target.Specification, flags.OidStrategy).GetDomainTypeUri()))
                    });
                    tempProperties.Add(links);
                }

                var members = new OptionalProperty(JsonPropertyNames.Members, Create(flags.OidStrategy, memberValues));
                tempProperties.Add(members);
                mapRepresentation = Create(flags.OidStrategy, tempProperties.ToArray());
            }
            else {
                mapRepresentation = Create(flags.OidStrategy, memberValues);
            }

            mapRepresentation.SetContentType(mt);

            return mapRepresentation;
        }
    }
}