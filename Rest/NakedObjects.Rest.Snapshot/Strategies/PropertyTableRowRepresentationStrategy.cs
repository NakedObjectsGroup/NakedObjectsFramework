// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Net.Http;
using System.Runtime.Serialization;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using NakedObjects.Rest.Snapshot.Representations;
using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest.Snapshot.Strategies {
    [DataContract]
    public class PropertyTableRowRepresentationStrategy : AbstractPropertyRepresentationStrategy {
        public PropertyTableRowRepresentationStrategy(IOidStrategy oidStrategy, HttpRequestMessage req, PropertyContextFacade propertyContext, RestControlFlags flags) :
            base(oidStrategy, req, propertyContext, flags) {}

        public override bool ShowChoices() {
            return false;
        }

        public override LinkRepresentation[] GetLinks() {
            return new LinkRepresentation[] {};
        }

        protected override bool AddChoices() {
            return propertyContext.Property.IsChoicesEnabled != Choices.NotEnabled &&
                   (propertyContext.Property.Specification.IsEnum ||
                    propertyContext.Property.Specification.IsParseable);
        }

        public override object GetPropertyValue(IOidStrategy oidStrategy, HttpRequestMessage req, IAssociationFacade property, IObjectFacade target, RestControlFlags flags, bool valueOnly, bool useDateOverDateTime) {
            //IObjectFacade valueNakedObject = property.GetValue(target);
            //string title = RestUtils.SafeGetTitle(property, valueNakedObject);

            //if (valueNakedObject == null) {
            //    return null;
            //}
            //if (property.Specification.IsParseable || property.Specification.IsCollection) {
            //    return RestUtils.ObjectToPredefinedType(valueNakedObject.GetDomainObject(), useDateOverDateTime);
            //}

            //if (valueOnly) {
            //    return RefValueRepresentation.Create(oidStrategy, new ValueRelType(property, new UriMtHelper(oidStrategy, req, valueNakedObject)), flags);
            //}

            //var titleProperty =  new OptionalProperty(JsonPropertyNames.Title, title);

            //return MapRepresentation.Create(titleProperty);

            // todo minimise this 
            return Representation.GetPropertyValue(oidStrategy, req, property, target, flags, valueOnly, useDateOverDateTime);
        }
    }
}