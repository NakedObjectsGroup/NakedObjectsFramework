// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

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
    public class ActionRepresentation : Representation {
        protected ActionRepresentation(IOidStrategy oidStrategy, AbstractActionRepresentationStrategy strategy)
            : base(oidStrategy, strategy.GetFlags()) {
            SelfRelType = strategy.GetSelf();
            Id = strategy.GetId();
            Parameters = strategy.GetParameters();
            Links = strategy.GetLinks();
            Extensions = strategy.GetExtensions();
            SetHeader(strategy.GetTarget());
        }

        [DataMember(Name = JsonPropertyNames.Id)]
        public string Id { get; set; }

        [DataMember(Name = JsonPropertyNames.Parameters)]
        public MapRepresentation Parameters { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        private void SetHeader(IObjectFacade target) {
            Caching = CacheType.Transactional;
            SetEtag(target);
        }

        public static ActionRepresentation Create(IOidStrategy oidStrategy, HttpRequest req, ActionContextFacade actionContext, RestControlFlags flags) {
            var actionRepresentationStrategy = AbstractActionRepresentationStrategy.GetStrategy(false, oidStrategy, req, actionContext, flags);
            return new ActionRepresentation(oidStrategy, actionRepresentationStrategy);
        }
    }
}