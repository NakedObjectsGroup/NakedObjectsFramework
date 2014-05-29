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
using RestfulObjects.Snapshot.Strategies;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class InlineCollectionRepresentation : InlineMemberAbstractRepresentation {
        protected InlineCollectionRepresentation(CollectionRepresentationStrategy strategy) : base(strategy.GetFlags()) {
            MemberType = MemberTypes.Collection;
            Id = strategy.GetId();
            Size = strategy.GetSize();
            Links = strategy.GetLinks(true);
            Extensions = strategy.GetExtensions();
            SetHeader(strategy.GetTarget());
        }

        [DataMember(Name = JsonPropertyNames.Size)]
        public int Size { get; set; }

        public static InlineCollectionRepresentation Create(HttpRequestMessage req, PropertyContextSurface propertyContext, IList<OptionalProperty> optionals, RestControlFlags flags) {
            if (propertyContext.Property.IsEager(propertyContext.Target) && !propertyContext.Target.IsTransient()) {
                IEnumerable<INakedObjectSurface> collectionItems = propertyContext.Property.GetNakedObject(propertyContext.Target).ToEnumerable();
                IEnumerable<LinkRepresentation> items = collectionItems.Select(i => LinkRepresentation.Create(new ValueRelType(propertyContext.Property, new UriMtHelper(req, i)), flags, new OptionalProperty(JsonPropertyNames.Title, RestUtils.SafeGetTitle(i))));
                optionals.Add(new OptionalProperty(JsonPropertyNames.Value, items.ToArray()));
            }

            var collectionRepresentationStrategy = new CollectionRepresentationStrategy(req, propertyContext, flags);
            if (optionals.Count == 0) {
                return new InlineCollectionRepresentation(collectionRepresentationStrategy);
            }
            return CreateWithOptionals<InlineCollectionRepresentation>(new object[] {collectionRepresentationStrategy}, optionals);
        }
    }
}