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
    public class ActionTypeRepresentation : Representation {
        protected ActionTypeRepresentation(HttpRequestMessage req, ActionTypeContextSurface actionTypeContext, RestControlFlags flags) : base(flags) {
            SelfRelType = new TypeMemberRelType(RelValues.Self, new UriMtHelper(req, actionTypeContext));
            SetScalars(actionTypeContext);
            SetLinks(req, actionTypeContext);
            SetParameters(req, actionTypeContext);
            SetExtensions();
            SetHeader();
        }

        [DataMember(Name = JsonPropertyNames.Id)]
        public string Id { get; set; }

        [DataMember(Name = JsonPropertyNames.FriendlyName)]
        public string FriendlyName { get; set; }

        [DataMember(Name = JsonPropertyNames.Description)]
        public string Description { get; set; }

        [DataMember(Name = JsonPropertyNames.HasParams)]
        public bool HasParams { get; set; }

        [DataMember(Name = JsonPropertyNames.MemberOrder)]
        public int MemberOrder { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Parameters)]
        public LinkRepresentation[] Parameters { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        private void SetHeader() {
            caching = CacheType.NonExpiring;
        }

        private void SetExtensions() {
            Extensions = MapRepresentation.Create();
        }

        private void SetScalars(ActionTypeContextSurface actionTypeContext) {
            Id = actionTypeContext.ActionContext.Id;
            FriendlyName = actionTypeContext.ActionContext.Action.Name();
            Description = actionTypeContext.ActionContext.Action.Description();
            HasParams = actionTypeContext.ActionContext.VisibleParameters.Any();
            MemberOrder = actionTypeContext.ActionContext.Action.MemberOrder();
        }

        private void SetParameters(HttpRequestMessage req, ActionTypeContextSurface actionTypeContext) {
            IEnumerable<LinkRepresentation> parms = actionTypeContext.ActionContext.VisibleParameters.
                                                                      Select(p => LinkRepresentation.Create(new ParamTypeRelType(new UriMtHelper(req, new ParameterTypeContextSurface {
                                                                          Action = actionTypeContext.ActionContext.Action,
                                                                          OwningSpecification = actionTypeContext.OwningSpecification,
                                                                          Parameter = p.Parameter
                                                                      })), Flags));
            Parameters = parms.ToArray();
        }

        private void SetLinks(HttpRequestMessage req, ActionTypeContextSurface actionTypeContext) {
            var domainTypeUri = new UriMtHelper(req, actionTypeContext);
            var tempLinks = new List<LinkRepresentation> {
                LinkRepresentation.Create(SelfRelType, Flags),
                LinkRepresentation.Create(new DomainTypeRelType(RelValues.Up, domainTypeUri), Flags),
                LinkRepresentation.Create(new DomainTypeRelType(RelValues.ReturnType, new UriMtHelper(req, actionTypeContext.ActionContext.Action.ReturnType)), Flags)
            };

            if (actionTypeContext.ActionContext.Action.ReturnType.IsCollection()) {
                tempLinks.Add(LinkRepresentation.Create(new DomainTypeRelType(RelValues.ElementType, new UriMtHelper(req, actionTypeContext.ActionContext.Action.ReturnType.ElementType)), Flags));
            }

            Links = tempLinks.ToArray();
        }


        public static ActionTypeRepresentation Create(HttpRequestMessage req, ActionTypeContextSurface actionTypeContext, RestControlFlags flags) {
            return new ActionTypeRepresentation(req, actionTypeContext, flags);
        }
    }
}