// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using NakedObjects.Facade.Contexts;
using NakedObjects.Surface;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Strategies;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class PropertyRepresentation : MemberAbstractRepresentation {
        protected PropertyRepresentation(IOidStrategy oidStrategy, PropertyRepresentationStrategy strategy)
            : base(oidStrategy ,strategy) {
            HasChoices = strategy.GetHasChoices();
            Links = strategy.GetLinks(false);
            Extensions = strategy.GetExtensions();
        }

        [DataMember(Name = JsonPropertyNames.HasChoices)]
        public bool HasChoices { get; set; }


        public static PropertyRepresentation Create(IOidStrategy oidStrategy, HttpRequestMessage req, PropertyContextFacade propertyContext, IList<OptionalProperty> optionals, RestControlFlags flags) {
            if (!RestUtils.IsBlobOrClob(propertyContext.Specification) && !RestUtils.IsAttachment(propertyContext.Specification)) {
                optionals.Add(new OptionalProperty(JsonPropertyNames.Value, GetPropertyValue(oidStrategy ,req, propertyContext.Property, propertyContext.Target, flags)));
            }

            RestUtils.AddChoices(oidStrategy ,req, propertyContext, optionals, flags);
            return CreateWithOptionals<PropertyRepresentation>(new object[] {oidStrategy, new PropertyRepresentationStrategy(oidStrategy ,req, propertyContext, flags)}, optionals);
        }
    }
}