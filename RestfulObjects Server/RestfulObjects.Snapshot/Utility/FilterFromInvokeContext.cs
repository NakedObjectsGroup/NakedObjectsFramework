// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using NakedObjects.Facade;
using RestfulObjects.Snapshot.Constants;

namespace RestfulObjects.Snapshot.Utility {
    public class FilterFromInvokeContext {
        #region ActionType enum

        public enum ActionType {
            FilterSubtypesFrom,
            FilterSuperTypesFrom
        };

        #endregion

        private readonly ActionType aType;

        public FilterFromInvokeContext(string actionName, string typeName) {
            TypeName = typeName;
            if (actionName == WellKnownIds.FilterSubtypesFrom) {
                aType = ActionType.FilterSubtypesFrom;
            }
            else if (actionName == WellKnownIds.FilterSupertypesFrom) {
                aType = ActionType.FilterSuperTypesFrom;
            }
            else {
                throw new TypeActionResourceNotFoundException(actionName, typeName);
            }
        }

        public string TypeName { get; }

        public string Id => aType == ActionType.FilterSubtypesFrom ? WellKnownIds.FilterSubtypesFrom : WellKnownIds.FilterSupertypesFrom;

        public string ParameterId => aType == ActionType.FilterSubtypesFrom ? JsonPropertyNames.SubTypes : JsonPropertyNames.SuperTypes;

        public ITypeFacade[] Value => aType == ActionType.FilterSubtypesFrom ? Subtypes() : Supertypes();

        public ITypeFacade ThisSpecification { get; set; }
        public ITypeFacade[] OtherSpecifications { get; set; }

        private ITypeFacade[] Subtypes() {
            return OtherSpecifications.Where(os => os.IsOfType(ThisSpecification)).ToArray();
        }

        private ITypeFacade[] Supertypes() {
            return OtherSpecifications.Where(os => ThisSpecification.IsOfType(os)).ToArray();
        }
    }
}