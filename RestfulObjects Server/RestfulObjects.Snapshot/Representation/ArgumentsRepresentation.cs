// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

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

        private static MapRepresentation GetMap(HttpRequestMessage req, ContextSurface context, RestControlFlags flags) {
            MapRepresentation value;

            // All reasons why we cannot create a linkrep
            if (context.Specification.IsParseable() ||
                context.Specification.IsCollection() ||
                context.ProposedValue == null ||
                context.ProposedNakedObject == null ||
                context.ProposedNakedObject.Specification.IsParseable()) {
                value = CreateMap(context, context.ProposedValue);
            }
            else {
                value = CreateMap(context, RefValueRepresentation.Create(new ObjectRelType(RelValues.Self, new UriMtHelper(req, context.ProposedNakedObject)), flags));
            }
            return value;
        }

        private static MapRepresentation CreateMap(ContextSurface context, object obj) {
            var opts = new List<OptionalProperty> {new OptionalProperty(JsonPropertyNames.Value, obj)};
            if (!string.IsNullOrEmpty(context.Reason)) {
                opts.Add(new OptionalProperty(JsonPropertyNames.InvalidReason, context.Reason));
            }
            return Create(opts.ToArray());
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
                mapRepresentation = Create(optionalProperties.ToArray());
            }
            else if (actionResultContextSurface != null) {
                List<OptionalProperty> optionalProperties = actionResultContextSurface.ActionContext.VisibleParameters.Select(c => new OptionalProperty(c.Id, GetMap(req, c, flags))).ToList();

                if (!string.IsNullOrEmpty(actionResultContextSurface.Reason)) {
                    optionalProperties.Add(new OptionalProperty(JsonPropertyNames.XRoInvalidReason, actionResultContextSurface.Reason));
                }
                mapRepresentation = Create(optionalProperties.ToArray());
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
                    var dt = new OptionalProperty(JsonPropertyNames.DomainType, target.Specification.DomainTypeName());
                    tempProperties.Add(dt);
                }

                if (flags.FormalDomainModel) {
                    var links = new OptionalProperty(JsonPropertyNames.Links, new[] {
                        Create(new OptionalProperty(JsonPropertyNames.Rel, RelValues.DescribedBy),
                               new OptionalProperty(JsonPropertyNames.Href, new UriMtHelper(req, target.Specification).GetDomainTypeUri()))
                    });
                    tempProperties.Add(links);
                }

                var members = new OptionalProperty(JsonPropertyNames.Members, Create(memberValues));
                tempProperties.Add(members);
                mapRepresentation = Create(tempProperties.ToArray());
            }
            else {
                mapRepresentation = Create(memberValues);
            }

            mapRepresentation.SetContentType(mt);

            return mapRepresentation;
        }
    }
}