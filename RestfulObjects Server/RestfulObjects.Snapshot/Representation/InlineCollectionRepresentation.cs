// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Strategies;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class InlineCollectionRepresentation : InlineMemberAbstractRepresentation {
        protected InlineCollectionRepresentation(IOidStrategy oidStrategy, AbstractCollectionRepresentationStrategy strategy)
            : base(oidStrategy, strategy.GetFlags()) {
            MemberType = MemberTypes.Collection;
            Id = strategy.GetId();
            Links = strategy.GetLinks(true);
            Extensions = strategy.GetExtensions();
            SetHeader(strategy.GetTarget());
        }

        public static InlineCollectionRepresentation Create(IOidStrategy oidStrategy, HttpRequestMessage req, PropertyContextFacade propertyContext, IList<OptionalProperty> optionals, RestControlFlags flags) {
            var collectionRepresentationStrategy = AbstractCollectionRepresentationStrategy.GetStrategy(true, oidStrategy, req, propertyContext, flags);

            int? size = collectionRepresentationStrategy.GetSize();

            if (size != null) {
                optionals.Add(new OptionalProperty(JsonPropertyNames.Size, size));
            }

            var value = collectionRepresentationStrategy.GetValue();

            if (value != null) {
                optionals.Add(new OptionalProperty(JsonPropertyNames.Value, value));
            }

            if (optionals.Count == 0) {
                return new InlineCollectionRepresentation(oidStrategy, collectionRepresentationStrategy);
            }
            return CreateWithOptionals<InlineCollectionRepresentation>(new object[] {oidStrategy, collectionRepresentationStrategy}, optionals);
        }
    }
}