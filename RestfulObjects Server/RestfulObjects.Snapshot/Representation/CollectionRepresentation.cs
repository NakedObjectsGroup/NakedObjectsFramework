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
    public class CollectionRepresentation : MemberAbstractRepresentation {
        protected CollectionRepresentation(CollectionRepresentationStrategy strategy) : base(strategy) {
            Value = strategy.GetValue();
            Extensions = strategy.GetExtensions();
        }

        [DataMember(Name = JsonPropertyNames.Value)]
        public LinkRepresentation[] Value { get; set; }

        public static CollectionRepresentation Create(HttpRequestMessage req, PropertyContextSurface propertyContext, IList<OptionalProperty> optionals, RestControlFlags flags) {
            var collectionRepresentationStrategy = new CollectionRepresentationStrategy(req, propertyContext, flags);
            if (optionals.Count == 0) {
                return new CollectionRepresentation(collectionRepresentationStrategy);
            }
            return CreateWithOptionals<CollectionRepresentation>(new object[] {collectionRepresentationStrategy}, optionals);
        }
    }
}