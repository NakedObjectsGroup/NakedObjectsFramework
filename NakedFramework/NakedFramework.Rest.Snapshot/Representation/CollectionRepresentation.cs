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
using NakedFramework.Rest.Snapshot.Constants;
using NakedFramework.Rest.Snapshot.Strategies;
using NakedFramework.Rest.Snapshot.Utility;

namespace NakedFramework.Rest.Snapshot.Representation;

[DataContract]
public class CollectionRepresentation : MemberAbstractRepresentation {
    protected CollectionRepresentation(IFrameworkFacade frameworkFacade, AbstractCollectionRepresentationStrategy strategy) : base(frameworkFacade, strategy) => Extensions = strategy.GetExtensions(strategy.GetTarget());

    public static CollectionRepresentation Create(IFrameworkFacade frameworkFacade, HttpRequest req, PropertyContextFacade propertyContext, IList<OptionalProperty> optionals, RestControlFlags flags) {
        var collectionRepresentationStrategy = AbstractCollectionRepresentationStrategy.GetStrategy(false, false, frameworkFacade, req, propertyContext, flags);

        var value = collectionRepresentationStrategy.GetValue();

        if (value != null) {
            optionals.Add(new OptionalProperty(JsonPropertyNames.Value, value));
        }

        var actions = collectionRepresentationStrategy.GetActions();

        if (actions.Any()) {
            var members = RestUtils.CreateMap(actions.ToDictionary(m => m.Id, m => (object)m));
            optionals.Add(new OptionalProperty(JsonPropertyNames.Members, members));
        }

        return optionals.Any()
            ? CreateWithOptionals<CollectionRepresentation>(new object[] { frameworkFacade, collectionRepresentationStrategy }, optionals)
            : new CollectionRepresentation(frameworkFacade, collectionRepresentationStrategy);
    }
}