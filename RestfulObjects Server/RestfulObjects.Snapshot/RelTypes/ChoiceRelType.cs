// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Surface;
using RestfulObjects.Snapshot.Constants;

namespace RestfulObjects.Snapshot.Utility {
    public class ChoiceRelType : ObjectRelType {
        private readonly IMemberFacade member;
        private readonly INakedObjectActionParameterSurface parameter;
        private ChoiceRelType(UriMtHelper helper) : base(RelValues.Choice, helper) {}

        public ChoiceRelType(INakedObjectAssociationSurface property, UriMtHelper helper) : this(helper) {
            member = property;
        }

        public ChoiceRelType(INakedObjectActionParameterSurface parameter, UriMtHelper helper)
            : this(helper) {
            this.parameter = parameter;
        }

        public override string Name {
            get { return base.Name + (parameter == null ? helper.GetRelParametersFor(member) : helper.GetRelParametersFor(parameter)); }
        }
    }
}