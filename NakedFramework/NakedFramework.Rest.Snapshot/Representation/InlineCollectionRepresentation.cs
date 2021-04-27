// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;
using NakedFramework.Facade.Contexts;
using NakedFramework.Facade.Interface;
using NakedFramework.Facade.Translation;
using NakedFramework.Rest.Snapshot.Constants;
using NakedFramework.Rest.Snapshot.Strategies;
using NakedFramework.Rest.Snapshot.Utility;

namespace NakedFramework.Rest.Snapshot.Representation {
    [DataContract]
    public class InlineCollectionRepresentation : InlineMemberAbstractRepresentation {
        protected InlineCollectionRepresentation(IFrameworkFacade frameworkFacade, AbstractCollectionRepresentationStrategy strategy)
            : base(frameworkFacade.OidStrategy, strategy.GetFlags()) {
            MemberType = MemberTypes.Collection;
            Id = strategy.GetId();
            Links = strategy.GetLinks(true);
            Extensions = strategy.GetExtensions();
            SetHeader(strategy.GetTarget());
        }

        public static InlineCollectionRepresentation Create(IFrameworkFacade frameworkFacade, HttpRequest req, PropertyContextFacade propertyContext, IList<OptionalProperty> optionals, RestControlFlags flags, bool asTableColumn) {
            var collectionRepresentationStrategy = AbstractCollectionRepresentationStrategy.GetStrategy(asTableColumn, true, frameworkFacade, req, propertyContext, flags);

            var size = collectionRepresentationStrategy.GetSize();

            if (size != null) {
                optionals.Add(new OptionalProperty(JsonPropertyNames.Size, size));
            }

            var value = collectionRepresentationStrategy.GetValue();

            if (value != null) {
                optionals.Add(new OptionalProperty(JsonPropertyNames.Value, value));
            }

            var actions = collectionRepresentationStrategy.GetActions();

            if (actions.Any()) {
                var members = RestUtils.CreateMap(actions.ToDictionary(m => m.Id, m => (object) m));
                optionals.Add(new OptionalProperty(JsonPropertyNames.Members, members));
            }

            return optionals.Any()
                ? CreateWithOptionals<InlineCollectionRepresentation>(new object[] {frameworkFacade, collectionRepresentationStrategy}, optionals)
                : new InlineCollectionRepresentation(frameworkFacade, collectionRepresentationStrategy);
        }
    }
}