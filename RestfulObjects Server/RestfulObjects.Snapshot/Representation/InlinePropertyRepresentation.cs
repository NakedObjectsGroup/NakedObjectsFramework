// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using NakedObjects.Surface;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Strategies;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class InlinePropertyRepresentation : InlineMemberAbstractRepresentation {
        protected InlinePropertyRepresentation(PropertyRepresentationStrategy strategy) : base(strategy.GetFlags()) {
            MemberType = MemberTypes.Property;
            Id = strategy.GetId();
            HasChoices = strategy.GetHasChoices();
            Links = strategy.GetLinks(true);
            Extensions = strategy.GetExtensions();
            SetHeader(strategy.GetTarget());
        }

        [DataMember(Name = JsonPropertyNames.HasChoices)]
        public bool HasChoices { get; set; }

        public static InlinePropertyRepresentation Create(HttpRequestMessage req, PropertyContextSurface propertyContext, IList<OptionalProperty> optionals, RestControlFlags flags) {
            if (!RestUtils.IsBlobOrClob(propertyContext.Specification) && !RestUtils.IsAttachment(propertyContext.Specification)) {
                optionals.Add(new OptionalProperty(JsonPropertyNames.Value, GetPropertyValue(req, propertyContext.Property, propertyContext.Target, flags)));
            }
            RestUtils.AddChoices(req, propertyContext, optionals, flags);
            return CreateWithOptionals<InlinePropertyRepresentation>(new object[] {new PropertyRepresentationStrategy(req, propertyContext, flags)}, optionals);
        }
    }
}