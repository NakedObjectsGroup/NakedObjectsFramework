// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

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
    public class ParameterTypeRepresentation : Representation {
        protected ParameterTypeRepresentation(HttpRequestMessage req, ParameterTypeContextSurface parameterTypeContext, RestControlFlags flags) : base(flags) {
            SetScalars(parameterTypeContext);
            SelfRelType = new ParamTypeRelType(RelValues.Self, new UriMtHelper(req, parameterTypeContext));
            SetLinks(req, parameterTypeContext);
            SetExtensions();
            SetHeader();
        }

        [DataMember(Name = JsonPropertyNames.Id)]
        public string Id { get; set; }

        [DataMember(Name = JsonPropertyNames.Number)]
        public int Number { get; set; }

        [DataMember(Name = JsonPropertyNames.Name)]
        public string Name { get; set; }

        [DataMember(Name = JsonPropertyNames.FriendlyName)]
        public string FriendlyName { get; set; }

        [DataMember(Name = JsonPropertyNames.Description)]
        public string Description { get; set; }

        [DataMember(Name = JsonPropertyNames.Optional)]
        public bool Optional { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        private void SetHeader() {
            caching = CacheType.NonExpiring;
        }

        private void SetExtensions() {
            Extensions = MapRepresentation.Create();
        }

        private void SetScalars(ParameterTypeContextSurface parameterTypeContext) {
            Id = parameterTypeContext.Parameter.Id;
            Number = parameterTypeContext.Parameter.Number();
            Name = parameterTypeContext.Parameter.Id;
            FriendlyName = parameterTypeContext.Parameter.Name();
            Description = parameterTypeContext.Parameter.Description();
            Optional = !parameterTypeContext.Parameter.IsMandatory();
        }

        private void SetLinks(HttpRequestMessage req, ParameterTypeContextSurface parameterTypeContext) {
            var domainTypeUri = new UriMtHelper(req, parameterTypeContext);

            var tempLinks = new List<LinkRepresentation> {
                LinkRepresentation.Create(SelfRelType, Flags),
                LinkRepresentation.Create(new TypeMemberRelType(RelValues.Up, domainTypeUri), Flags),
                LinkRepresentation.Create(new DomainTypeRelType(RelValues.ReturnType, new UriMtHelper(req, parameterTypeContext.Parameter.Specification)), Flags)
            };

            Links = tempLinks.ToArray();
        }


        public static ParameterTypeRepresentation Create(HttpRequestMessage req, ParameterTypeContextSurface parameterTypeContext, RestControlFlags flags) {
            if (!parameterTypeContext.Parameter.Specification.IsParseable()) {
                return new ParameterTypeRepresentation(req, parameterTypeContext, flags);
            }
            var exts = new Dictionary<string, object>();
            AddStringProperties(parameterTypeContext.Parameter.Specification, parameterTypeContext.Parameter.MaxLength(), parameterTypeContext.Parameter.Pattern(), exts);

            OptionalProperty[] parms = exts.Select(e => new OptionalProperty(e.Key, e.Value)).ToArray();

            return CreateWithOptionals<ParameterTypeRepresentation>(new object[] {req, parameterTypeContext, flags}, parms);
        }
    }
}