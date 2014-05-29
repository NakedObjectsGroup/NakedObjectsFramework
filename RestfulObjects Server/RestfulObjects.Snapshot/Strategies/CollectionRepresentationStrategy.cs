// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Linq;
using System.Net.Http;
using NakedObjects.Surface;
using NakedObjects.Surface.Utility;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Representations;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Strategies {
    public class CollectionRepresentationStrategy : MemberRepresentationStrategy {
        private readonly INakedObjectSurface collection;


        public CollectionRepresentationStrategy(HttpRequestMessage req, PropertyContextSurface propertyContext, RestControlFlags flags) : base(req, propertyContext, flags) {
            collection = propertyContext.Property.GetNakedObject(propertyContext.Target);
        }

        protected override MapRepresentation GetExtensionsForSimple() {
            return RestUtils.GetExtensions(friendlyname: propertyContext.Property.Name(),
                                           description: propertyContext.Property.Description(),
                                           pluralName: null,
                                           domainType: null,
                                           isService: null,
                                           hasParams: null,
                                           optional: null,
                                           maxLength: null,
                                           pattern: null,
                                           memberOrder: propertyContext.Property.MemberOrder(),
                                           customExtensions: propertyContext.Property.ExtensionData(),
                                           returnType: collection.Specification);
        }

        public LinkRepresentation[] GetValue() {
            return collection.ToEnumerable().Select(CreateValueLink).ToArray();
        }

        private LinkRepresentation CreateValueLink(INakedObjectSurface no) {
            return LinkRepresentation.Create(new ValueRelType(propertyContext.Property, new UriMtHelper(req, no)), Flags,
                                             new OptionalProperty(JsonPropertyNames.Title, RestUtils.SafeGetTitle(no)));
        }

        public int GetSize() {
            return propertyContext.Property.Count(propertyContext.Target);
        }
    }
}