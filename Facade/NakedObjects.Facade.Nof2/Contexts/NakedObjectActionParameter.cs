// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using NakedObjects.Facade.Nof2.Utility;
using org.nakedobjects.@object;

namespace NakedObjects.Facade.Nof2.Contexts {
    public class NakedObjectActionParameter {
        private readonly ActionWrapper action;
        private readonly object[] choices;
        private readonly object dflt;
        private readonly string id;
        private readonly int number;
        private readonly NakedObjectSpecification spec;

        public NakedObjectActionParameter(string id, int number, NakedObjectSpecification spec, ActionWrapper action, object[] choices, object dflt) {
            this.id = id;
            this.number = number;
            this.spec = spec;
            this.action = action;
            this.choices = choices;
            this.dflt = dflt;
        }

        public string getId() {
            return id;
        }

        public NakedObjectSpecification getSpecification() {
            return spec;
        }

        public string getName() {
            return FacadeUtils.MakeSpaced(FacadeUtils.Capitalize(id));
        }

        public string getDescription() {
            return "";
        }

        public bool isMandatory() {
            return true;
        }

        public int getNumber() {
            return number;
        }

        public ActionWrapper getAction() {
            return action;
        }

        public bool isChoicesEnabled() {
            return choices != null && choices.Any();
        }

        public Naked getDefault(Naked nakedObject) {
            return dflt == null ? null : org.nakedobjects.@object.NakedObjects.getObjectLoader().getAdapterFor(dflt);
        }

        public Naked[] getChoices(Naked nakedObject) {
            return choices.Select(o => org.nakedobjects.@object.NakedObjects.getObjectLoader().getAdapterFor(o)).Cast<Naked>().ToArray();
        }
    }
}