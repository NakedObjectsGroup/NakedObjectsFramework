// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using NakedObjects.Rest.Snapshot.Representations;
using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest.Snapshot.Strategies {
    public class FormActionRepresentationStrategy : AbstractActionRepresentationStrategy {
        public FormActionRepresentationStrategy(IOidStrategy oidStrategy, HttpRequest req, ActionContextFacade actionContext, RestControlFlags flags)
            : base(oidStrategy, req, actionContext, flags) {}

        protected override IEnumerable<ParameterRepresentation> GetParameterList() {
            var visibleProperties = ActionContext.Target.Specification.Properties.Where(p => p.IsUsable(ActionContext.Target).IsAllowed && p.IsVisible(ActionContext.Target));
            return visibleProperties.Select(GetParameter);
        }

        public override LinkRepresentation[] GetLinks() =>
            new List<LinkRepresentation> {
                CreateSelfLink(),
                CreateUpLink(),
                CreateActionLink()
            }.ToArray();

        protected ParameterRepresentation GetParameter(IAssociationFacade assoc) {
            IObjectFacade objectFacade = ActionContext.Target;
            return ParameterRepresentation.Create(OidStrategy, Req, objectFacade, assoc, ActionContext, Flags);
        }

        protected override bool HasParams() => GetParameterList().Any();
    }
}