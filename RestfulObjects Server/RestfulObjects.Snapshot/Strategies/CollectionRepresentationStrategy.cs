// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using System.Net.Http;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using NakedObjects.Surface;
using NakedObjects.Surface.Utility;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Representations;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Strategies {
    public class CollectionRepresentationStrategy : MemberRepresentationStrategy {
        private readonly IObjectFacade collection;


        public CollectionRepresentationStrategy(IOidStrategy oidStrategy, HttpRequestMessage req, PropertyContextSurface propertyContext, RestControlFlags flags)
            : base(oidStrategy ,req, propertyContext, flags) {
            collection = propertyContext.Property.GetNakedObject(propertyContext.Target);
        }

        protected override MapRepresentation GetExtensionsForSimple() {
            return RestUtils.GetExtensions(friendlyname: propertyContext.Property.Name,
                description: propertyContext.Property.Description,
                pluralName: null,
                domainType: null,
                isService: null,
                hasParams: null,
                optional: null,
                maxLength: null,
                pattern: null,
                memberOrder: propertyContext.Property.MemberOrder,
                customExtensions: propertyContext.Property.ExtensionData,
                returnType: collection.Specification,
                elementType: propertyContext.ElementSpecification,
                oidStrategy: OidStrategy);
        }

        public LinkRepresentation[] GetValue() {
            return collection.ToEnumerable().Select(CreateValueLink).ToArray();
        }

        private LinkRepresentation CreateValueLink(IObjectFacade no) {
            return LinkRepresentation.Create(OidStrategy ,new ValueRelType(propertyContext.Property, new UriMtHelper(OidStrategy, req, no)), Flags,
                new OptionalProperty(JsonPropertyNames.Title, RestUtils.SafeGetTitle(no)));
        }

        public int GetSize() {
            return propertyContext.Property.Count(propertyContext.Target);
        }
    }
}